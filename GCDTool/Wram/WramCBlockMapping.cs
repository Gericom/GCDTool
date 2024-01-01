namespace GCDTool.Wram;

/// <summary>
/// Specifies the mapping of TWL wram C block.
/// </summary>
readonly record struct WramCBlockMapping(WramCMaster Master, WramBCSlot Slot, bool Enable)
{
    public static explicit operator byte(WramCBlockMapping wramCBlockMapping)
    {
        return (byte)(((byte)wramCBlockMapping.Master & 3)
            | (((byte)wramCBlockMapping.Slot & 7) << 2)
            | (wramCBlockMapping.Enable ? 0x80 : 0));
    }
}