namespace GCDTool.Wram;

/// <summary>
/// Specifies the mapping of TWL wram A block.
/// </summary>
readonly record struct WramABlockMapping(WramAMaster Master, WramASlot Slot, bool Enable)
{
    public static explicit operator byte(WramABlockMapping wramABlockMapping)
    {
        return (byte)(((byte)wramABlockMapping.Master & 1)
            | (((byte)wramABlockMapping.Slot & 3) << 2)
            | (wramABlockMapping.Enable ? 0x80 : 0));
    }
}