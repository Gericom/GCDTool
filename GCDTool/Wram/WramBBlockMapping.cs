namespace GCDTool.Wram;

/// <summary>
/// Specifies the mapping of TWL wram B block.
/// </summary>
readonly record struct WramBBlockMapping(WramBMaster Master, WramBCSlot Slot, bool Enable)
{
    public static explicit operator byte(WramBBlockMapping wramBBlockMapping)
    {
        return (byte)(((byte)wramBBlockMapping.Master & 3)
            | (((byte)wramBBlockMapping.Slot & 7) << 2)
            | (wramBBlockMapping.Enable ? 0x80 : 0));
    }
}