using GCDTool.IO;

namespace GCDTool.Elf;

sealed class ElfHeader
{
    private const uint ElfHeaderMagic = 0x464C457F;

    public ElfHeader(EndianBinaryReader er)
    {
        Magic = er.Read<uint>();
        if (Magic != ElfHeaderMagic)
        {
            throw new InvalidDataException("Elf magic invalid!");
        }

        BitFormat = er.Read<byte>();
        Endianness = er.Read<byte>();
        Version = er.Read<byte>();
        Abi = er.Read<byte>();
        AbiVersion = er.Read<byte>();
        Padding = er.Read<byte>(7);
        ObjectType = er.Read<ushort>();
        Architecture = er.Read<ushort>();
        Version2 = er.Read<uint>();
        EntryPoint = er.Read<uint>();
        ProgramHeaderTableOffset = er.Read<uint>();
        SectionHeaderTableOffset = er.Read<uint>();
        Flags = er.Read<uint>();
        HeaderSize = er.Read<ushort>();
        ProgramHeaderTableEntrySize = er.Read<ushort>();
        ProgramHeaderTableEntryCount = er.Read<ushort>();
        SectionHeaderTableEntrySize = er.Read<ushort>();
        SectionHeaderTableEntryCount = er.Read<ushort>();
        SectionNamesIndex = er.Read<ushort>();
    }

    public uint Magic;
    public byte BitFormat;
    public byte Endianness;
    public byte Version;
    public byte Abi;
    public byte AbiVersion;
    public byte[] Padding;//7
    public ushort ObjectType;
    public ushort Architecture;
    public uint Version2;
    public uint EntryPoint;
    public uint ProgramHeaderTableOffset;
    public uint SectionHeaderTableOffset;
    public uint Flags;
    public ushort HeaderSize;
    public ushort ProgramHeaderTableEntrySize;
    public ushort ProgramHeaderTableEntryCount;
    public ushort SectionHeaderTableEntrySize;
    public ushort SectionHeaderTableEntryCount;
    public ushort SectionNamesIndex;
}