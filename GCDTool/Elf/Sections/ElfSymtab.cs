using GCDTool.IO;

namespace GCDTool.Elf.Sections;

sealed class ElfSymtab : ElfSection
{
    public IReadOnlyList<ElfSymbol> Symbols { get; }

    public ElfSymtab(SectionHeaderTableEntry section, string name)
        : base(section, name)
    {
        uint nrEntries = section.FileImageSize / section.EntrySize;
        var symbols = new ElfSymbol[nrEntries];
        var er = new EndianBinaryReader(new MemoryStream(section.SectionData), Endianness.LittleEndian);
        for (int i = 0; i < nrEntries; i++)
        {
            symbols[i] = new ElfSymbol(er);
        }
        Symbols = symbols;
        er.Close();
    }
}
