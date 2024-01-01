using GCDTool.IO;

namespace GCDTool.Wram;

/// <summary>
/// Class representing the initial wram configuration.
/// </summary>
sealed class WramConfig
{
    /// <summary>
    /// The block mapping of TWL wram A.
    /// </summary>
    public WramABlockMapping[] TwlWramABlockMapping { get; } = new WramABlockMapping[4];

    /// <summary>
    /// The block mapping of TWL wram B.
    /// </summary>
    public WramBBlockMapping[] TwlWramBBlockMapping { get; } = new WramBBlockMapping[8];

    /// <summary>
    /// The block mapping of TWL wram C.
    /// </summary>
    public WramCBlockMapping[] TwlWramCBlockMapping { get; } = new WramCBlockMapping[8];

    /// <summary>
    /// The mapping of TWL wram A on the ARM9.
    /// </summary>
    public WramAMapping Arm9TwlWramAMapping { get; set; }

    /// <summary>
    /// The mapping of TWL wram B on the ARM9.
    /// </summary>
    public WramBCMapping Arm9TwlWramBMapping { get; set; }

    /// <summary>
    /// The mapping of TWL wram C on the ARM9.
    /// </summary>
    public WramBCMapping Arm9TwlWramCMapping { get; set; }

    /// <summary>
    /// The mapping of TWL wram A on the ARM7.
    /// </summary>
    public WramAMapping Arm7TwlWramAMapping { get; set; }

    /// <summary>
    /// The mapping of TWL wram B on the ARM7.
    /// </summary>
    public WramBCMapping Arm7TwlWramBMapping { get; set; }

    /// <summary>
    /// The mapping of TWL wram C on the ARM7.
    /// </summary>
    public WramBCMapping Arm7TwlWramCMapping { get; set; }

    /// <summary>
    /// For each TWL wram A block <see langword="true"/> when locked, or <see langword="false"/> otherwise.
    /// </summary>
    public bool[] TwlWramABlockLocked { get; } = new bool[4];

    /// <summary>
    /// For each TWL wram B block <see langword="true"/> when locked, or <see langword="false"/> otherwise.
    /// </summary>
    public bool[] TwlWramBBlockLocked { get; } = new bool[8];

    /// <summary>
    /// For each TWL wram C block <see langword="true"/> when locked, or <see langword="false"/> otherwise.
    /// </summary>
    public bool[] TwlWramCBlockLocked { get; } = new bool[8];

    /// <summary>
    /// The mapping of nitro wram.
    /// </summary>
    public NtrWramMaster[] NtrWramMapping { get; } = new NtrWramMaster[2];

    /// <summary>
    /// The mapping of vram C.
    /// </summary>
    public byte VramCMapping { get; set; }

    /// <summary>
    /// The mapping of vram D.
    /// </summary>
    public byte VramDMapping { get; set; }

    /// <summary>
    /// Writes the wram configuration to the given <paramref name="destination"/> span.
    /// </summary>
    /// <param name="destination"></param>
    public void Write(Span<byte> destination)
    {
        for (int i = 0; i < 4; i++)
        {
            destination[i] = (byte)TwlWramABlockMapping[i];
        }
        for (int i = 0; i < 8; i++)
        {
            destination[4 + i] = (byte)TwlWramBBlockMapping[i];
            destination[12 + i] = (byte)TwlWramCBlockMapping[i];
        }
        IOUtil.WriteU32Le(destination[0x14..], (uint)Arm9TwlWramAMapping);
        IOUtil.WriteU32Le(destination[0x18..], (uint)Arm9TwlWramBMapping);
        IOUtil.WriteU32Le(destination[0x1C..], (uint)Arm9TwlWramCMapping);
        IOUtil.WriteU32Le(destination[0x20..], (uint)Arm7TwlWramAMapping);
        IOUtil.WriteU32Le(destination[0x24..], (uint)Arm7TwlWramBMapping);
        IOUtil.WriteU32Le(destination[0x28..], (uint)Arm7TwlWramCMapping);
        destination[0x2C] = CreateLockMask(TwlWramABlockLocked);
        destination[0x2D] = CreateLockMask(TwlWramBBlockLocked);
        destination[0x2E] = CreateLockMask(TwlWramCBlockLocked);
        destination[0x2F] = (byte)(((byte)NtrWramMapping[0] & 1) | (((byte)NtrWramMapping[1] & 1) << 1)
            | (VramCMapping & 7) << 2 | (VramDMapping & 7) << 5);
    }

    private byte CreateLockMask(ReadOnlySpan<bool> locks)
    {
        byte wramLocks = 0;
        for (int i = 0; i < locks.Length; i++)
        {
            if (locks[i])
            {
                wramLocks |= (byte)(1 << i);
            }
        }

        return wramLocks;
    }
}
