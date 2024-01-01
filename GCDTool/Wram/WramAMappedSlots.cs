namespace GCDTool.Wram;

/// <summary>
/// Specifies which TWL wram A slots are mapped in memory.
/// </summary>
enum WramAMappedSlots
{
    /// <summary>
    /// Only slot 0 mapped. 64 kB mirrors.
    /// </summary>
    Slot0 = 0,

    /// <summary>
    /// Slot 0 and 1 mapped. 128 kB mirrors.
    /// </summary>
    Slot01 = 2,

    /// <summary>
    /// All slots mapped. 256 kB mirrors.
    /// </summary>
    All = 3
}
