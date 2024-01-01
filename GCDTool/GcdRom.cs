namespace GCDTool;

/// <summary>
/// Class representing a GCD rom file.
/// </summary>
sealed class GcdRom
{
    private const int BLOWFISH_P_TABLE_OFFSET = 0x1600;
    private const int BLOWFISH_S_BOXES_OFFSET = 0x1C00;
    private const int TEST_PATTERNS_OFFSET = 0x3000;

    /// <summary>
    /// The header of the GCD rom.
    /// </summary>
    public GcdHeader Header { get; set; } = new();

    /// <summary>
    /// The encrypted ARM 9 binary.
    /// </summary>
    public byte[] EncryptedArm9Binary { get; set; } = [];

    /// <summary>
    /// The encrypted ARM 7 binary.
    /// </summary>
    public byte[] EncryptedArm7Binary { get; set; } = [];

    /// <summary>
    /// Writes the GCD rom file to the given <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    public void Write(Stream stream)
    {
        stream.Write(Header.ToByteArray());
        WriteBlowfishTable(stream);
        WriteTestPatterns(stream);
        stream.Position = Header.Arm9RomOffset;
        stream.Write(EncryptedArm9Binary);
        stream.Position = Header.Arm7RomOffset;
        stream.Write(EncryptedArm7Binary);
    }

    private void WriteBlowfishTable(Stream stream)
    {
        var blowfishTable = GcdBlowfish.GetTransformedKeyTable(Header.GameCode);
        stream.Position = BLOWFISH_P_TABLE_OFFSET;
        stream.Write(blowfishTable, 0, Blowfish.P_TABLE_ENTRY_COUNT * 4);
        stream.Position = BLOWFISH_S_BOXES_OFFSET;
        stream.Write(blowfishTable, Blowfish.P_TABLE_ENTRY_COUNT * 4, Blowfish.S_BOX_COUNT * Blowfish.S_BOX_ENTRY_COUNT * 4);
    }

    private void WriteTestPatterns(Stream stream)
    {
        stream.Position = TEST_PATTERNS_OFFSET;
        stream.Write([0xFF, 0x00, 0xFF, 0x00, 0xAA, 0x55, 0xAA, 0x55]);
        for (int i = 8; i < 0x200; i++)
        {
            stream.WriteByte((byte)(i & 0xFF));
        }
        for (int i = 0; i < 0x200; i++)
        {
            stream.WriteByte((byte)(0xFF - (i & 0xFF)));
        }
        for (int i = 0; i < 0x200; i++)
        {
            stream.WriteByte(0x00);
        }
        for (int i = 0; i < 0x200; i++)
        {
            stream.WriteByte(0xFF);
        }
        for (int i = 0; i < 0x200; i++)
        {
            stream.WriteByte(0x0F);
        }
        for (int i = 0; i < 0x200; i++)
        {
            stream.WriteByte(0xF0);
        }
        for (int i = 0; i < 0x200; i++)
        {
            stream.WriteByte(0x55);
        }
        for (int i = 0; i < 0x1FF; i++)
        {
            stream.WriteByte(0xAA);
        }
    }
}
