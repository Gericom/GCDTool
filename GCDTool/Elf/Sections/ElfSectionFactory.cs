namespace GCDTool.Elf.Sections;

sealed class ElfSectionFactory
{
    public ElfSection CreateElfSection(SectionHeaderTableEntry section, string name)
    {
        return section.SectionType switch
        {
            ElfSectionType.Strtab => new ElfStrtab(section, name),
            ElfSectionType.Symtab => new ElfSymtab(section, name),
            _ => new ElfSection(section, name),
        };
    }
}