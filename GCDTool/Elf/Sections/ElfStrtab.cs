namespace GCDTool.Elf.Sections;

sealed class ElfStrtab : ElfSection
{
    public ElfStrtab(SectionHeaderTableEntry section, string name)
        : base(section, name) { }

    public string GetString(uint offset)
    {
        string result = string.Empty;
        while (offset < SectionHeader.SectionData.Length)
        {
            char c = (char)SectionHeader.SectionData[offset++];
            if (c == '\0')
            {
                return result;
            }
            result += c;
        }
        return result;
    }
}
