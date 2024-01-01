using CommandLine;
using CommandLine.Text;
using GCDTool.Elf;
using GCDTool.IO;
using GCDTool.Wram;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Text;

namespace GCDTool;

static class Program
{
    private const string GCDTOOL_HEADING = "== GCDTool by Gericom ==";
    private const string AES_CIPHER_NAME = "AES/CTR/NoPadding";
    private const string ALGORITHM_AES = "AES";
    private const int AES_BLOCK_SIZE = 16;
    private const uint ROM_CONTROL = 0x00416657;
    private const uint ROM_CONTROL_SECURE = 0x081808F8;
    private const ushort SECURE_AREA_DELAY = 3454;

    private static ReadOnlySpan<byte> AesKeyX
        => [0x4E, 0x69, 0x6E, 0x74, 0x65, 0x6E, 0x64, 0x6F, 0x20, 0x44, 0x53, 0x00, 0x01, 0x23, 0x21, 0x00];

    private static void Main(string[] args)
    {
        var parser = new Parser(with => with.HelpWriter = null);
        var parserResult = parser.ParseArguments<CommandLineOptions>(args);
        parserResult
            .WithParsed(RunOptions)
            .WithNotParsed(_ =>
            {
                Console.WriteLine(HelpText.AutoBuild(parserResult, h =>
                {
                    h.AutoVersion = false;
                    h.Copyright = string.Empty;
                    h.Heading = GCDTOOL_HEADING;
                    return HelpText.DefaultParsingErrorsHandler(parserResult, h);
                }, e => e));
            });
    }

    private static void RunOptions(CommandLineOptions options)
    {
        if (options.GameCode.Length != 4)
        {
            Console.WriteLine("Invalid game code specified. Should be 4 characters.");
            return;
        }

        uint gameCode = IOUtil.ReadU32Le(Encoding.ASCII.GetBytes(options.GameCode));

        // This can be any key, used for encryption of the arm7 and arm9 binaries.
        // To have reproducable results, we'll simply use an all-zeros key.
        byte[] aesKeyY = new byte[16];

        var gcdRom = new GcdRom
        {
            Header = new GcdHeader
            {
                GameCode = gameCode,

                RomControl = ROM_CONTROL,
                RomControlSecure = ROM_CONTROL_SECURE,
                SecureAreaDelay = SECURE_AREA_DELAY,

                // it appears to be unnecessary to specify the rom area
                NitroRomRegionEnd = 0,
                TwlRomRegionStart = 0,

                Flags = options.Arm9Speed67MHz ? 0 : GcdHeaderFlags.Arm9Speed134MHz
            },
        };

        var signature = new GcdSignature();
        aesKeyY.CopyTo(signature.AesKeyY, 0);

        if (!TryLoadArm9(options.Arm9Path, gcdRom, signature) ||
            !TryLoadArm7(options.Arm7Path, gcdRom, signature) ||
            !TryLoadWramConfig(options.WramConfigJsonPath, gcdRom))
        {
            return;
        }

        gcdRom.Header.GetSha1Hash(signature.HeaderSha1);
        var signedSignature = signature.ToSignedByteArray(options.GcdKeyPath);
        signedSignature.CopyTo(gcdRom.Header.Signature, 0);

        using (var stream = File.Create(options.OutputPath))
        {
            gcdRom.Write(stream);
        }
    }

    private static bool TryLoadArm9(string arm9ElfPath, GcdRom gcdRom, GcdSignature signature)
    {
        var arm9Data = ParseElf(arm9ElfPath, out uint arm9LoadAddress);
        if (arm9LoadAddress < 0x03000000)
        {
            Console.WriteLine("Error: ARM9 binary cannot be loaded to main memory. Use TWL wram instead.");
            return false;
        }
        gcdRom.Header.Arm9LoadAddress = arm9LoadAddress;
        gcdRom.Header.Arm9Size = (uint)arm9Data.Length;
        SHA1.HashData(arm9Data, signature.Arm9Sha1);

        if ((arm9Data.Length % 512) != 0)
        {
            arm9Data = [.. arm9Data, .. new byte[512 - (arm9Data.Length % 512)]];
        }
        gcdRom.Header.Arm9PaddedSize = (uint)arm9Data.Length;
        EncryptAes(arm9Data, signature.AesKeyY);
        gcdRom.EncryptedArm9Binary = arm9Data;
        gcdRom.Header.Arm9RomOffset = 0x4000;
        return true;
    }

    private static bool TryLoadArm7(string arm7ElfPath, GcdRom gcdRom, GcdSignature signature)
    {
        var arm7Data = ParseElf(arm7ElfPath, out uint arm7LoadAddress);
        if (arm7LoadAddress < 0x03000000)
        {
            Console.WriteLine("Error: ARM7 binary cannot be loaded to main memory. Use TWL wram instead.");
            return false;
        }
        gcdRom.Header.Arm7LoadAddress = arm7LoadAddress;
        gcdRom.Header.Arm7Size = (uint)arm7Data.Length;
        SHA1.HashData(arm7Data, signature.Arm7Sha1);

        if ((arm7Data.Length % 512) != 0)
        {
            arm7Data = [.. arm7Data, .. new byte[512 - (arm7Data.Length % 512)]];
        }
        gcdRom.Header.Arm7PaddedSize = (uint)arm7Data.Length;
        EncryptAes(arm7Data, signature.AesKeyY);
        gcdRom.EncryptedArm7Binary = arm7Data;
        gcdRom.Header.Arm7RomOffset = (uint)(0x4000 + gcdRom.EncryptedArm9Binary.Length);
        return true;
    }

    private static byte[] ParseElf(string elfPath, out uint loadAddress)
    {
        loadAddress = 0;
        var elf = new ElfFile(File.ReadAllBytes(elfPath));
        var programData = new MemoryStream();
        bool loadAddressSet = false;
        foreach (var programHeader in elf.ProgramHeaderTable)
        {
            if (programHeader.SegmentType == ProgramHeaderTableEntry.ElfSegmentType.Load &&
                programHeader.SegmentData is not null &&
                programHeader.SegmentData.Length != 0)
            {
                if (!loadAddressSet)
                {
                    loadAddress = programHeader.PhysicalAddress;
                    loadAddressSet = true;
                }
                programData.Write(programHeader.SegmentData);
            }
        }

        return programData.ToArray();
    }

    private static void ByteSwapAesBlocks(Span<byte> data)
    {
        for (int i = 0; i < data.Length; i += AES_BLOCK_SIZE)
        {
            data.Slice(i, AES_BLOCK_SIZE).Reverse();
        }
    }

    private static byte[] CreateAesInitializationVector(uint dataLength)
    {
        var iv = new byte[16];
        IOUtil.WriteU32Le(iv, 0, dataLength);
        IOUtil.WriteU32Le(iv, 4, (uint)-dataLength);
        IOUtil.WriteU32Le(iv, 8, ~dataLength);
        return iv;
    }

    private static void EncryptAes(Span<byte> data, ReadOnlySpan<byte> keyY)
    {
        var scrambledKey = AesKeyScrambler.Scramble(AesKeyX, keyY);
        Array.Reverse(scrambledKey);
        ByteSwapAesBlocks(data);
        var iv = CreateAesInitializationVector((uint)data.Length);
        Array.Reverse(iv);
        var cipher = CipherUtilities.GetCipher(AES_CIPHER_NAME);
        cipher.Init(true, new ParametersWithIV(
            ParameterUtilities.CreateKeyParameter(ALGORITHM_AES, scrambledKey), iv));
        cipher.ProcessBytes(data, data);
        cipher.DoFinal(data);
        ByteSwapAesBlocks(data);
    }

    private static bool TryLoadWramConfig(string wramConfigJsonPath, GcdRom gcdRom)
    {
        try
        {
            string wramConfigJson = File.ReadAllText(wramConfigJsonPath);
            gcdRom.Header.WramConfiguration = new WramConfigJsonReader().ReadWramConfigFromJson(wramConfigJson);
            return true;
        }
        catch (Exception exception)
        {
            Console.WriteLine("Failed to parse wram configuration JSON: " + exception.Message);
            return false;
        }
    }
}
