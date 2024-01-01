namespace GCDTool;

[Flags]
enum GcdHeaderFlags : byte
{
    Arm9Compressed = (1 << 0),
    Arm7Compressed = (1 << 1),
    Arm9Speed134MHz = (1 << 2)
}