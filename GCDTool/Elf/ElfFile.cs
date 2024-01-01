using GCDTool.Elf.Sections;
using GCDTool.IO;

namespace GCDTool.Elf;

sealed class ElfFile
{
    public ElfFile(byte[] data)
    {
        var er = new EndianBinaryReader(new MemoryStream(data), Endianness.LittleEndian);
        Header = new ElfHeader(er);

        er.BaseStream.Position = Header.ProgramHeaderTableOffset;
        ProgramHeaderTable = new ProgramHeaderTableEntry[Header.ProgramHeaderTableEntryCount];
        for (int i = 0; i < Header.ProgramHeaderTableEntryCount; i++)
        {
            ProgramHeaderTable[i] = new ProgramHeaderTableEntry(er);
        }

        er.BaseStream.Position = Header.SectionHeaderTableOffset;
        SectionHeaderTable = new SectionHeaderTableEntry[Header.SectionHeaderTableEntryCount];
        for (int i = 0; i < Header.SectionHeaderTableEntryCount; i++)
        {
            SectionHeaderTable[i] = new SectionHeaderTableEntry(er);
        }

        er.Close();

        var namestab = new ElfStrtab(SectionHeaderTable[Header.SectionNamesIndex], string.Empty);
        Sections = new ElfSection[SectionHeaderTable.Length];
        var sectionFactory = new ElfSectionFactory();
        for (int i = 0; i < SectionHeaderTable.Length; i++)
        {
            Sections[i] = sectionFactory.CreateElfSection(SectionHeaderTable[i],
                namestab.GetString(SectionHeaderTable[i].NameOffset));
        }
    }

    public ElfHeader Header;

    public ProgramHeaderTableEntry[] ProgramHeaderTable;

    public SectionHeaderTableEntry[] SectionHeaderTable;

    public ElfSection[] Sections { get; }

    public ElfSection? GetSectionByName(string name)
    {
        return Sections.FirstOrDefault(section => section.Name == name);
    }
}
