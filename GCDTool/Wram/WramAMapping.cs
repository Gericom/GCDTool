namespace GCDTool.Wram;

/// <summary>
/// Specifies the mapping of TWL wram A in memory.
/// </summary>
readonly record struct WramAMapping
{
    private const uint WRAM_START_ADDRESS = 0x03000000u;
    private const uint WRAM_END_ADDRESS = 0x04000000u;

    private readonly uint _startOffset;
    private readonly uint _endOffset;

    /// <summary>
    /// The WRAM A slots that are mapped and mirrored in memory.
    /// </summary>
    public WramAMappedSlots MappedSlots { get; }

    /// <summary>
    /// The start address of the WRAM A area in memory.
    /// </summary>
    public uint StartAddress => WRAM_START_ADDRESS + (_startOffset << 16);

    /// <summary>
    /// The end address of the WRAM A area in memory.
    /// </summary>
    public uint EndAddress => WRAM_START_ADDRESS + (_endOffset << 16);

    public WramAMapping(uint startAddress, uint length, WramAMappedSlots mappedSlots)
    {
        if (startAddress < WRAM_START_ADDRESS)
        {
            throw new ArgumentException(
                $"Start address (0x{startAddress:X8}) must be at least (0x{WRAM_START_ADDRESS:X8}).", nameof(startAddress));
        }
        if ((startAddress & 0xFFFF) != 0)
        {
            throw new ArgumentException("Start address must be a multiple of 64 kB.", nameof(startAddress));
        }
        if ((length & 0xFFFF) != 0)
        {
            throw new ArgumentException("Length must be a multiple of 64 kB.", nameof(length));
        }
        uint endAddress = startAddress + length;
        if (endAddress > WRAM_END_ADDRESS)
        {
            throw new ArgumentException(
                $"Length is too large. Resulting end address (0x{endAddress:X8}) must be at most (0x{WRAM_END_ADDRESS:X8}).",
                nameof(length));
        }

        _startOffset = (startAddress - WRAM_START_ADDRESS) >> 16;
        _endOffset = (endAddress - WRAM_START_ADDRESS) >> 16;
        MappedSlots = mappedSlots;
    }

    public static explicit operator uint(WramAMapping wramAMapping)
    {
        return (wramAMapping._startOffset << 4)
            | ((uint)wramAMapping.MappedSlots << 12)
            | (wramAMapping._endOffset << 20);
    }
}
