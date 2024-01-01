using GCDTool.IO;

namespace GCDTool.Elf;

sealed class ProgramHeaderTableEntry
{
    public enum ElfSegmentType : uint
    {
        Null = 0,
        Load = 1,
        Dynamic = 2,
        Interp = 3,
        Note = 4,
        Shlib = 5,
        Phdr = 6,
        Tls = 7,
        Num = 8,
        Loos = 0x60000000,
        Hios = 0x6fffffff,
        Loproc = 0x70000000,
        Hiproc = 0x7fffffff
    }

    public ProgramHeaderTableEntry(EndianBinaryReader er)
    {
        SegmentType = er.Read<ElfSegmentType>();
        FileImageOffset = er.Read<uint>();
        VirtualAddress = er.Read<uint>();
        PhysicalAddress = er.Read<uint>();
        FileImageSize = er.Read<uint>();
        MemorySize = er.Read<uint>();
        Flags = er.Read<uint>();
        Alignment = er.Read<uint>();

        if (FileImageSize != 0)
        {
            long curpos = er.BaseStream.Position;
            er.BaseStream.Position = FileImageOffset;
            SegmentData = er.Read<byte>((int)FileImageSize);
            er.BaseStream.Position = curpos;
        }
    }

    public ElfSegmentType SegmentType;
    public uint FileImageOffset;
    public uint VirtualAddress;
    public uint PhysicalAddress;
    public uint FileImageSize;
    public uint MemorySize;
    public uint Flags;
    public uint Alignment;

    public byte[] SegmentData = [];
}
