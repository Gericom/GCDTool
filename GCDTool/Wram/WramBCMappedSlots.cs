namespace GCDTool.Wram;

/// <summary>
/// Specifies which TWL wram B or C slots are mapped in memory.
/// </summary>
enum WramBCMappedSlots
{
    /// <summary>
    /// Only slot 0 mapped. 32 kB mirrors.
    /// </summary>
    Slot0 = 0,

    /// <summary>
    /// Only slot 0 and 1 mapped. 64 kB mirrors.
    /// </summary>
    Slot01 = 1,

    /// <summary>
    /// Only slot 0, 1, 2 and 3 mapped. 128 kB mirrors.
    /// </summary>
    Slot0123 = 2,

    /// <summary>
    /// All slots mapped. 256 kB mirrors.
    /// </summary>
    All = 3
}