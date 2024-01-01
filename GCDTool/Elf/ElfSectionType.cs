namespace GCDTool.Elf;

enum ElfSectionType : uint
{
    Null = 0,
    Progbits = 1,
    Symtab = 2,
    Strtab = 3,
    Rela = 4,
    Hash = 5,
    Dynamic = 6,
    Note = 7,
    Nobits = 8,
    Rel = 9,
    Shlib = 10,
    Dynsym = 11,
    InitArray = 14,
    FiniArray = 15,
    PreinitArray = 16,
    Group = 17,
    SymtabShndx = 18,
    Num = 19,
    Loos = 0x60000000,
    Hios = 0x6fffffff,
    Loproc = 0x70000000,
    Hiproc = 0x7fffffff,
    Louser = 0x80000000,
    Hiuser = 0xffffffff
}