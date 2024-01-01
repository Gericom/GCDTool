using GCDTool.IO;

namespace GCDTool.Elf.Sections;

sealed class ElfSymbol
{
    public ElfSymbol(EndianBinaryReader er)
    {
        NameOffset = er.Read<uint>();
        Value = er.Read<uint>();
        Size = er.Read<uint>();
        Info = er.Read<byte>();
        Other = er.Read<byte>();
        SectionIndex = er.Read<ushort>();
    }

    public uint NameOffset;
    public uint Value;
    public uint Size;
    public byte Info;
    public byte Other;
    public ushort SectionIndex;
}