namespace GCDTool.Wram;

/// <summary>
/// Specifies the mapping of TWL wram B or C in memory.
/// </summary>
readonly record struct WramBCMapping
{
    private const uint WRAM_START_ADDRESS = 0x03000000u;
    private const uint WRAM_END_ADDRESS = 0x04000000u;

    private readonly uint _startOffset;
    private readonly uint _endOffset;

    /// <summary>
    /// The WRAM A slots that are mapped and mirrored in memory.
    /// </summary>
    public WramBCMappedSlots MappedSlots { get; }

    /// <summary>
    /// The start address of the WRAM B or C area in memory.
    /// </summary>
    public uint StartAddress => WRAM_START_ADDRESS + (_startOffset << 15);

    /// <summary>
    /// The end address of the WRAM B or C area in memory.
    /// </summary>
    public uint EndAddress => WRAM_START_ADDRESS + (_endOffset << 15);

    public WramBCMapping(uint startAddress, uint length, WramBCMappedSlots mappedSlots)
    {
        if (startAddress < WRAM_START_ADDRESS)
        {
            throw new ArgumentException(
                $"Start address (0x{startAddress:X8}) must be at least (0x{WRAM_START_ADDRESS:X8}).", nameof(startAddress));
        }
        if ((startAddress & 0x7FFF) != 0)
        {
            throw new ArgumentException("Start address must be a multiple of 32 kB.", nameof(startAddress));
        }
        if ((length & 0x7FFF) != 0)
        {
            throw new ArgumentException("Length must be a multiple of 32 kB.", nameof(length));
        }
        uint endAddress = startAddress + length;
        if (endAddress > WRAM_END_ADDRESS)
        {
            throw new ArgumentException(
                $"Length is too large. Resulting end address (0x{endAddress:X8}) must be at most (0x{WRAM_END_ADDRESS:X8}).",
                nameof(length));
        }

        _startOffset = (startAddress - WRAM_START_ADDRESS) >> 15;
        _endOffset = (endAddress - WRAM_START_ADDRESS) >> 15;
        MappedSlots = mappedSlots;
    }

    public static explicit operator uint(WramBCMapping wramBCMapping)
    {
        return (wramBCMapping._startOffset << 3)
            | ((uint)wramBCMapping.MappedSlots << 12)
            | (wramBCMapping._endOffset << 19);
    }
}
