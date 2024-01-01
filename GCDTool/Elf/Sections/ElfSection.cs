namespace GCDTool.Elf.Sections;

class ElfSection
{
    public SectionHeaderTableEntry SectionHeader { get; }
    public string Name { get; }

    public ElfSection(SectionHeaderTableEntry sectionHeader, string name)
    {
        SectionHeader = sectionHeader;
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }
}
