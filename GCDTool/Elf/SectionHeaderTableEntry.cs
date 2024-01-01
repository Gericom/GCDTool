using GCDTool.IO;

namespace GCDTool.Elf;

sealed class SectionHeaderTableEntry
{
    public SectionHeaderTableEntry(EndianBinaryReader er)
    {
        NameOffset = er.Read<uint>();
        SectionType = er.Read<ElfSectionType>();
        Flags = er.Read<uint>();
        VirtualAddress = er.Read<uint>();
        FileImageOffset = er.Read<uint>();
        FileImageSize = er.Read<uint>();
        Link = er.Read<uint>();
        Info = er.Read<uint>();
        Alignment = er.Read<uint>();
        EntrySize = er.Read<uint>();

        if (FileImageSize != 0)
        {
            long curpos = er.BaseStream.Position;
            er.BaseStream.Position = FileImageOffset;
            SectionData = er.Read<byte>((int)FileImageSize);
            er.BaseStream.Position = curpos;
        }
    }

    public uint NameOffset;
    public ElfSectionType SectionType;
    public uint Flags;
    public uint VirtualAddress;
    public uint FileImageOffset;
    public uint FileImageSize;
    public uint Link;
    public uint Info;
    public uint Alignment;
    public uint EntrySize;

    public byte[] SectionData = [];
}
