using GCDTool.IO;
using GCDTool.Wram;
using System.Security.Cryptography;

namespace GCDTool;

/// <summary>
/// Class representing the header of a GCD rom file.
/// </summary>
sealed class GcdHeader
{
    /// <summary>
    /// The game code of the GCD rom.
    /// </summary>
    public uint GameCode { get; set; }

    /// <summary>
    /// The offset of the ARM 9 binary in the rom.
    /// </summary>
    public uint Arm9RomOffset { get; set; }

    /// <summary>
    /// The original size of the ARM 9 binary.
    /// </summary>
    public uint Arm9Size { get; set; }

    /// <summary>
    /// The memory address where the ARM 9 binary will be loaded.
    /// </summary>
    public uint Arm9LoadAddress { get; set; }

    /// <summary>
    /// The padded size of the ARM 9 binary.
    /// </summary>
    public uint Arm9PaddedSize { get; set; }

    /// <summary>
    /// The offset of the ARM 7 binary in the rom.
    /// </summary>
    public uint Arm7RomOffset { get; set; }

    /// <summary>
    /// The original size of the ARM 7 binary.
    /// </summary>
    public uint Arm7Size { get; set; }

    /// <summary>
    /// The memory address where the ARM 7 binary will be loaded.
    /// </summary>
    public uint Arm7LoadAddress { get; set; }

    /// <summary>
    /// The padded size of the ARM 7 binary.
    /// </summary>
    public uint Arm7PaddedSize { get; set; }

    /// <summary>
    /// The regular rom control settings.
    /// </summary>
    public uint RomControl { get; set; }

    /// <summary>
    /// The secure rom control settings.
    /// </summary>
    public uint RomControlSecure { get; set; }

    /// <summary>
    /// The secure area delay.
    /// </summary>
    public ushort SecureAreaDelay { get; set; }

    /// <summary>
    /// The end of the nitro region of the rom.
    /// </summary>
    public ushort NitroRomRegionEnd { get; set; }

    /// <summary>
    /// The start of the twl region of the rom.
    /// </summary>
    public ushort TwlRomRegionStart { get; set; }

    /// <summary>
    /// Flags controlling how the GCD firm is loaded.
    /// </summary>
    public GcdHeaderFlags Flags { get; set; }

    /// <summary>
    /// The RSA signed signature. See <see cref="GcdSignature"/> for the format before signing.
    /// </summary>
    public byte[] Signature { get; } = new byte[GcdSignature.SIGNATURE_SIGNED_LENGTH];

    /// <summary>
    /// The initial WRAM configuration used by the GCD firm.
    /// </summary>
    public WramConfig WramConfiguration { get; set; } = new();

    /// <summary>
    /// Serializes the header data as a byte array.
    /// </summary>
    /// <returns>The header data as a byte array.</returns>
    public byte[] ToByteArray()
    {
        var header = new byte[0x200];
        IOUtil.WriteU32Le(header, 0x0C, GameCode);
        IOUtil.WriteU32Le(header, 0x20, Arm9RomOffset);
        IOUtil.WriteU32Le(header, 0x24, Arm9Size);
        IOUtil.WriteU32Le(header, 0x28, Arm9LoadAddress);
        IOUtil.WriteU32Le(header, 0x2C, Arm9PaddedSize);
        IOUtil.WriteU32Le(header, 0x30, Arm7RomOffset);
        IOUtil.WriteU32Le(header, 0x34, Arm7Size);
        IOUtil.WriteU32Le(header, 0x38, Arm7LoadAddress);
        IOUtil.WriteU32Le(header, 0x3C, Arm7PaddedSize);
        IOUtil.WriteU32Le(header, 0x60, RomControl);
        IOUtil.WriteU32Le(header, 0x64, RomControlSecure);
        IOUtil.WriteU16Le(header, 0x6E, SecureAreaDelay);
        IOUtil.WriteU16Le(header, 0x90, NitroRomRegionEnd);
        IOUtil.WriteU16Le(header, 0x92, TwlRomRegionStart);
        header[0xFF] = (byte)Flags;
        Signature.CopyTo(header, 0x100);
        WramConfiguration.Write(header.AsSpan(0x180));
        return header;
    }

    /// <summary>
    /// Calculates the SHA-1 hash used for <see cref="GcdSignature.HeaderSha1"/>.
    /// </summary>
    /// <param name="destination">The byte span to write the SHA-1 hash to.</param>
    public void GetSha1Hash(Span<byte> destination)
    {
        var headerData = ToByteArray();
        SHA1.HashData([.. headerData.AsSpan(0, 0x100), .. headerData.AsSpan(0x180, 0x80)], destination);
    }
}
