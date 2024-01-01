using System.Buffers.Binary;

namespace GCDTool;

/// <summary>
/// Static class implementing the DSi AES key scrambler.
/// </summary>
public static class AesKeyScrambler
{
    private static readonly UInt128 scramblerMagic = new(0xFFFEFB4E29590258ul, 0x2A680F5F1A4F3E79ul);

    private const int SCRAMBLER_ROL = 42;

    /// <summary>
    /// Scrambles the given <paramref name="keyX"/> and <paramref name="keyY"/>.
    /// </summary>
    /// <param name="keyX">Input key X.</param>
    /// <param name="keyY">Input key Y.</param>
    /// <returns>The scrambled key.</returns>
    public static byte[] Scramble(ReadOnlySpan<byte> keyX, ReadOnlySpan<byte> keyY)
    {
        UInt128 scrambledKey = Scramble(
            BinaryPrimitives.ReadUInt128LittleEndian(keyX),
            BinaryPrimitives.ReadUInt128LittleEndian(keyY));
        var result = new byte[16];
        BinaryPrimitives.WriteUInt128LittleEndian(result, scrambledKey);
        return result;
    }

    /// <summary>
    /// Scrambles the given <paramref name="keyX"/> and <paramref name="keyY"/>.
    /// </summary>
    /// <param name="keyX">Input key X.</param>
    /// <param name="keyY">Input key Y.</param>
    /// <returns>The scrambled key.</returns>
    public static UInt128 Scramble(UInt128 keyX, UInt128 keyY)
    {
        return UInt128.RotateLeft((keyX ^ keyY) + scramblerMagic, SCRAMBLER_ROL);
    }
}