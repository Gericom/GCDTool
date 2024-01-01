﻿using GCDTool.IO;
using System.Buffers.Binary;

namespace GCDTool;

/// <summary>
/// Static helper class for the GCD blowfish table.
/// </summary>
static class GcdBlowfish
{
    /// <summary>
    /// Creates the GCD blowfish table corresponding to the given <paramref name="gameCode"/>.
    /// </summary>
    /// <param name="gameCode">The game code.</param>
    /// <returns>The transformed blowfish table.</returns>
    public static byte[] GetTransformedKeyTable(uint gameCode)
    {
        var keyTable = KeyTable.ToArray();
        uint keyCode = BinaryPrimitives.ReverseEndianness(gameCode);
        for (int i = 0; i < Blowfish.P_TABLE_ENTRY_COUNT; i++)
        {
            IOUtil.WriteU32Le(keyTable, i * 4,
                IOUtil.ReadU32Le(keyTable, i * 4) ^ keyCode);
        }

        var scratch = new byte[8];
        for (int i = 0; i < Blowfish.KEY_TABLE_LENGTH; i += Blowfish.BLOCK_LENGTH)
        {
            var blowfish = new Blowfish(keyTable);
            blowfish.Encrypt(scratch);
            Array.Copy(scratch, 4, keyTable, i, 4);
            Array.Copy(scratch, 0, keyTable, i + 4, 4);
        }

        return keyTable;
    }

    /// <summary>
    /// The untransformed GCD blowfish table.
    /// </summary>
    public static ReadOnlySpan<byte> KeyTable =>
    [
        0xEE, 0xA8, 0x95, 0x75, 0x2D, 0xF3, 0xFF, 0x84, 0xC6, 0xAE, 0xF8, 0x58,
        0xC2, 0x44, 0x64, 0x6F, 0xBC, 0xCF, 0xA6, 0x10, 0x13, 0xB8, 0xE1, 0xBE,
        0xC3, 0xAB, 0xEF, 0x88, 0xCD, 0x26, 0x20, 0xC7, 0x3A, 0x91, 0x0B, 0xC0,
        0xC0, 0x74, 0xB0, 0x9F, 0x0F, 0x83, 0xD4, 0x56, 0xE5, 0xDE, 0xAB, 0x69,
        0xF2, 0x5F, 0x6E, 0xCF, 0x2F, 0xBE, 0xFE, 0xD7, 0xE2, 0xD5, 0xF5, 0x84,
        0xDA, 0xCC, 0xA1, 0x73, 0x99, 0x59, 0x20, 0x44, 0x63, 0x8F, 0x27, 0x74,
        0x53, 0x72, 0x90, 0xF0, 0x8F, 0xD4, 0x95, 0x1C, 0x99, 0xCE, 0xDB, 0x7C,
        0x8A, 0x50, 0xB9, 0xA8, 0x9E, 0x9F, 0x37, 0x79, 0xFE, 0x44, 0x91, 0x12,
        0x4D, 0x55, 0xB3, 0xD2, 0xC6, 0x02, 0xD7, 0x72, 0x43, 0x81, 0x05, 0xCE,
        0xB8, 0x11, 0xB4, 0x72, 0xE7, 0x2A, 0xCF, 0x9A, 0x95, 0xD1, 0xC0, 0x62,
        0xE5, 0x61, 0x08, 0x7D, 0xF6, 0xC8, 0x3A, 0x33, 0x59, 0x7B, 0xC1, 0xAF,
        0x12, 0xAB, 0xAB, 0x7F, 0xDF, 0xB8, 0x20, 0x5A, 0xE1, 0x08, 0xC9, 0x43,
        0x54, 0x7B, 0x05, 0xDF, 0x17, 0x7D, 0x23, 0x65, 0x1C, 0x58, 0x88, 0x89,
        0xD9, 0xCA, 0x02, 0x1D, 0x8A, 0x3C, 0xE6, 0xDD, 0x12, 0xEE, 0x2F, 0x30,
        0x9D, 0xDD, 0xBE, 0x0A, 0x74, 0xB5, 0x6F, 0x58, 0xAD, 0x0A, 0x13, 0x10,
        0x35, 0xAD, 0x1E, 0x0A, 0x70, 0x08, 0x6F, 0xFE, 0x02, 0x4F, 0xAC, 0x8C,
        0x2C, 0xEF, 0x56, 0x9C, 0xCD, 0x9B, 0xAB, 0xC0, 0x52, 0x9A, 0xAF, 0x7F,
        0xCD, 0x2D, 0x82, 0xEF, 0xC2, 0xFD, 0xC0, 0x2B, 0x57, 0xA5, 0x22, 0xDE,
        0x67, 0x28, 0xF8, 0xCC, 0x19, 0x9D, 0x2A, 0x58, 0x15, 0x07, 0x79, 0xD2,
        0x89, 0xF6, 0x8D, 0xDC, 0xE2, 0xA6, 0x0C, 0x25, 0x6D, 0x23, 0x35, 0xFE,
        0xD6, 0xE2, 0x48, 0x86, 0xE7, 0xF6, 0x0B, 0xAA, 0x20, 0x06, 0x24, 0xD1,
        0xA7, 0x5E, 0x3A, 0x41, 0x43, 0x91, 0xF5, 0x4C, 0x72, 0x54, 0x4F, 0x4F,
        0x08, 0x9A, 0xF2, 0xA5, 0x8F, 0x4E, 0xFF, 0x1F, 0x2D, 0xEC, 0xF0, 0x14,
        0x2A, 0xF4, 0xD6, 0x47, 0xF0, 0x21, 0xB0, 0x85, 0x0A, 0xF2, 0x36, 0x78,
        0xD7, 0xD0, 0x08, 0xD9, 0xD5, 0x9D, 0xCC, 0xBD, 0xFB, 0x0B, 0xAC, 0xA6,
        0xAF, 0x7D, 0xFB, 0x96, 0xFF, 0x76, 0x54, 0xB6, 0x51, 0x9B, 0xE9, 0xBD,
        0x8E, 0x4B, 0xC8, 0xE8, 0x30, 0x86, 0xC8, 0x72, 0x79, 0x09, 0x8D, 0x3F,
        0xDC, 0x45, 0x1E, 0x5C, 0xDB, 0xB1, 0x55, 0x75, 0x90, 0x5A, 0xDB, 0x3E,
        0x66, 0xAC, 0x7F, 0xB0, 0xBA, 0x6A, 0x31, 0xF7, 0xBD, 0x88, 0xA0, 0x84,
        0x9D, 0xFB, 0xB2, 0xF0, 0xE1, 0x48, 0x4B, 0x01, 0xE3, 0x67, 0x09, 0x6D,
        0xE4, 0x60, 0x8D, 0xA3, 0xF2, 0xED, 0x8E, 0x14, 0x64, 0x88, 0x89, 0x81,
        0xAA, 0x73, 0x0D, 0xFE, 0xD5, 0x7B, 0xC3, 0x58, 0x45, 0xC3, 0xE2, 0x8C,
        0xE7, 0x1D, 0x95, 0x82, 0xA6, 0x1A, 0xBE, 0x17, 0x1B, 0xAD, 0xE8, 0xBF,
        0x76, 0x41, 0x6B, 0x4D, 0x3D, 0xA4, 0x0D, 0x3A, 0xAC, 0xDC, 0xA4, 0x50,
        0x2E, 0x28, 0xF1, 0x69, 0x76, 0x95, 0xB4, 0x16, 0xFE, 0x1F, 0x52, 0x6B,
        0x74, 0x81, 0x58, 0x67, 0x28, 0x1D, 0xDE, 0xB3, 0xBD, 0x8C, 0x19, 0x06,
        0xEA, 0x40, 0xA2, 0xEE, 0x8E, 0x35, 0x3E, 0xD1, 0x6C, 0xB0, 0x64, 0x36,
        0x27, 0xE2, 0xF4, 0x59, 0xD3, 0x22, 0x41, 0xB5, 0xE3, 0x71, 0xBB, 0x94,
        0x94, 0xFB, 0x15, 0x03, 0xD6, 0x01, 0x73, 0x64, 0x0D, 0x1F, 0x3F, 0x94,
        0xC4, 0xAE, 0x2A, 0x7A, 0xF7, 0x88, 0xBF, 0x51, 0x1A, 0x09, 0x2C, 0x71,
        0x3E, 0x6E, 0x3B, 0x6D, 0x52, 0x4D, 0xB2, 0x6E, 0xA4, 0x2C, 0xC8, 0x9F,
        0x8E, 0x18, 0xFC, 0x8F, 0x0A, 0x14, 0x31, 0xBE, 0x56, 0x57, 0x3E, 0x1D,
        0x6E, 0xE1, 0x74, 0xC5, 0x93, 0xA7, 0x05, 0xB1, 0xEC, 0x58, 0xF6, 0x94,
        0x4C, 0x86, 0xD4, 0x6E, 0xCF, 0xE3, 0xBA, 0xB2, 0x15, 0xCF, 0x42, 0xE3,
        0x27, 0x4D, 0xBC, 0xF2, 0xB1, 0x34, 0x03, 0x09, 0x42, 0x2A, 0x63, 0xC2,
        0x92, 0x00, 0x4F, 0x88, 0x51, 0xBD, 0x9A, 0xA5, 0xF9, 0x03, 0xD2, 0xE9,
        0x57, 0x22, 0x12, 0x7D, 0xAA, 0x09, 0xA7, 0x02, 0x9B, 0xC3, 0x9F, 0xAC,
        0xED, 0xBE, 0x40, 0x48, 0xA4, 0x37, 0xBE, 0x74, 0xD3, 0xA4, 0xB9, 0x24,
        0xA7, 0xBC, 0x78, 0x81, 0xE5, 0x85, 0x6B, 0x60, 0xB2, 0x46, 0xCB, 0x1D,
        0x60, 0x20, 0x11, 0xE6, 0x8A, 0x00, 0x44, 0xBD, 0x64, 0x29, 0xBE, 0xD1,
        0x32, 0x50, 0xCC, 0xCF, 0x43, 0x0E, 0xE1, 0x74, 0xAB, 0xC5, 0xFA, 0x53,
        0xA4, 0xE0, 0xA6, 0x84, 0x7A, 0x99, 0x80, 0x90, 0x00, 0x01, 0x1A, 0xA3,
        0x1C, 0x1C, 0xE2, 0xCE, 0x73, 0xE9, 0xCE, 0xEB, 0xC7, 0xA4, 0xE9, 0x77,
        0x9B, 0x1B, 0x26, 0x3A, 0x16, 0xCA, 0x49, 0xCE, 0x2D, 0x43, 0x3A, 0xEC,
        0xBF, 0xC1, 0x10, 0x14, 0x24, 0xC8, 0x22, 0x8D, 0x66, 0x82, 0x01, 0xB0,
        0xE2, 0xE2, 0x64, 0x79, 0x14, 0xEB, 0xC6, 0xFC, 0x35, 0xE0, 0xE6, 0x12,
        0x0A, 0x9D, 0x68, 0xCA, 0xFC, 0x39, 0x9F, 0x5F, 0xE9, 0xF7, 0x14, 0xBF,
        0xF2, 0x13, 0x78, 0x5D, 0x01, 0x74, 0x34, 0x87, 0x60, 0x76, 0x10, 0x1A,
        0xB7, 0x1B, 0xAA, 0xFE, 0xA2, 0xB1, 0xEB, 0x53, 0x57, 0xA4, 0x47, 0xFC,
        0x17, 0xDE, 0x1C, 0xF7, 0x36, 0x9A, 0x37, 0x25, 0x5D, 0x20, 0xED, 0xB2,
        0x36, 0xBF, 0x25, 0x12, 0xD4, 0x6B, 0x53, 0x14, 0xDF, 0x8C, 0x13, 0x31,
        0xF4, 0xB7, 0xEC, 0xEE, 0x19, 0x5B, 0x6E, 0xBB, 0x0F, 0x7D, 0x87, 0x39,
        0x24, 0xAC, 0x66, 0x43, 0x3C, 0x89, 0x13, 0xFB, 0xB2, 0x39, 0x70, 0x3A,
        0x37, 0x80, 0x1B, 0x3A, 0x34, 0x86, 0x66, 0xFC, 0x83, 0xB7, 0x3F, 0x0A,
        0x26, 0x05, 0x38, 0x50, 0x1A, 0x4B, 0x0E, 0x9C, 0x98, 0x93, 0x2B, 0x46,
        0x2D, 0x7D, 0x6C, 0x0E, 0x9E, 0x92, 0x9A, 0x68, 0xE6, 0x01, 0x57, 0x71,
        0x7D, 0xD1, 0x61, 0x2E, 0x45, 0x91, 0xDA, 0x09, 0xA1, 0x3B, 0x14, 0xFB,
        0x59, 0x92, 0x19, 0x5B, 0xEE, 0xA5, 0xE3, 0xC4, 0x9A, 0xDA, 0x4B, 0x0B,
        0x6C, 0x52, 0x8C, 0xE1, 0x1A, 0x1C, 0x17, 0x73, 0x4B, 0xA9, 0x72, 0x15,
        0x29, 0xB7, 0x15, 0x6D, 0x91, 0x9E, 0x50, 0x71, 0x6E, 0xC7, 0x1B, 0x57,
        0x18, 0x10, 0x72, 0x8C, 0x42, 0x17, 0xCC, 0x1F, 0x75, 0x2C, 0x60, 0xA6,
        0xA9, 0x88, 0x7F, 0x78, 0xC4, 0xF9, 0x98, 0x05, 0x1B, 0xC6, 0xDD, 0x5A,
        0x41, 0x7B, 0x9B, 0x8E, 0x72, 0xAF, 0x4D, 0x2F, 0x2A, 0x9E, 0xF0, 0xD9,
        0xCA, 0x54, 0x60, 0xE7, 0x4E, 0x0B, 0xBC, 0x28, 0x32, 0x0C, 0x32, 0x4A,
        0x7E, 0x68, 0x2B, 0xDF, 0xCC, 0xFB, 0xDF, 0x2C, 0x69, 0x6B, 0xF1, 0x77,
        0x65, 0x56, 0x42, 0x5D, 0xD9, 0x7F, 0x20, 0xCF, 0x6D, 0x16, 0x4D, 0x2E,
        0x57, 0x79, 0xE5, 0x46, 0x39, 0x0F, 0xF5, 0x37, 0x4A, 0xB7, 0x42, 0x67,
        0x47, 0xDF, 0x21, 0xBB, 0x64, 0x09, 0xDF, 0xE2, 0x89, 0xD9, 0xD6, 0x0D,
        0x6E, 0x5A, 0x63, 0x6F, 0x57, 0xA4, 0x05, 0x8F, 0x10, 0xC0, 0x5D, 0x6E,
        0x43, 0x6F, 0xE7, 0xEA, 0x6A, 0x20, 0x5C, 0x13, 0x38, 0xD9, 0x6B, 0x87,
        0x07, 0xC6, 0xCB, 0x1C, 0x28, 0x0E, 0x60, 0xAF, 0xE0, 0x4F, 0xAA, 0x0D,
        0x33, 0xFE, 0xE0, 0x95, 0xB1, 0x44, 0x3E, 0x09, 0xAD, 0xB2, 0x16, 0x1D,
        0xA2, 0x0B, 0x71, 0x8D, 0xD9, 0xC1, 0xA3, 0x3C, 0xE1, 0xA8, 0x54, 0x8F,
        0xC8, 0x1E, 0xB1, 0x7E, 0x0E, 0x2A, 0xB8, 0xCD, 0x6D, 0xFF, 0x1A, 0xDE,
        0xE2, 0x2C, 0xAE, 0x68, 0x3E, 0xCA, 0xE1, 0x80, 0x2C, 0x0C, 0xD6, 0x67,
        0xDA, 0xD1, 0x6A, 0xAC, 0x02, 0xC8, 0x30, 0x53, 0x4D, 0xA0, 0x67, 0x5F,
        0x9D, 0x3C, 0x6F, 0x5D, 0xB0, 0x25, 0x47, 0x1F, 0x69, 0x4C, 0x4A, 0x09,
        0x21, 0xEA, 0x1D, 0x4C, 0xD5, 0xCE, 0x09, 0xFD, 0x41, 0xC3, 0x0F, 0x27,
        0xF5, 0x81, 0x60, 0xF1, 0xB4, 0xBD, 0xDC, 0x54, 0xF5, 0x6B, 0xA3, 0x72,
        0x6A, 0xC1, 0xAB, 0xBE, 0x02, 0xBB, 0x82, 0xDD, 0x20, 0x5D, 0xE7, 0xF0,
        0xB0, 0x64, 0x6A, 0xEA, 0x2C, 0x5C, 0xCB, 0x85, 0xAA, 0x5A, 0x3A, 0xD5,
        0xD1, 0x91, 0x0F, 0x40, 0x5E, 0x69, 0x20, 0xC0, 0x76, 0x5C, 0xD0, 0xA3,
        0x46, 0x04, 0xC0, 0x8A, 0xCE, 0xE1, 0xEB, 0xE0, 0x13, 0xCA, 0xD2, 0x7E,
        0xCA, 0xBB, 0x41, 0x7F, 0xC0, 0xFF, 0xDA, 0x09, 0x09, 0x9C, 0xD3, 0x16,
        0x9E, 0xFE, 0x17, 0xD4, 0x36, 0x4B, 0xFC, 0x11, 0x11, 0x21, 0xB9, 0x87,
        0x89, 0xEA, 0x1C, 0x58, 0x6D, 0x51, 0x51, 0x6A, 0x94, 0xF9, 0x91, 0xA6,
        0xAB, 0xC6, 0xE5, 0x06, 0x80, 0x07, 0x20, 0x81, 0xB7, 0x60, 0xEA, 0xE6,
        0xAF, 0x67, 0x09, 0x8D, 0x79, 0x1D, 0xF8, 0x32, 0x78, 0x3B, 0xF6, 0x1B,
        0xBB, 0x70, 0xFE, 0xEE, 0x42, 0x3B, 0x12, 0x74, 0x16, 0x5B, 0x34, 0xF7,
        0x88, 0x13, 0xAA, 0x7A, 0x94, 0xA3, 0x52, 0x01, 0xA1, 0x3E, 0x46, 0xB5,
        0x33, 0x86, 0x96, 0xE7, 0x17, 0x0F, 0xB0, 0xF6, 0x26, 0x59, 0xE6, 0xFE,
        0x88, 0x5F, 0xC9, 0xFA, 0x38, 0x88, 0x78, 0xF7, 0xC8, 0x27, 0x4D, 0x40,
        0x5A, 0x72, 0x40, 0x0A, 0x18, 0x86, 0x94, 0xAC, 0x6F, 0x96, 0xFB, 0xB2,
        0xC3, 0x40, 0x0B, 0x73, 0xA0, 0xB3, 0xA5, 0x3F, 0x57, 0x7B, 0x4A, 0x2A,
        0x94, 0x09, 0xEE, 0xE2, 0x03, 0x2B, 0x66, 0xE2, 0x53, 0xC2, 0x17, 0xA5,
        0xC2, 0x9A, 0xCD, 0x5F, 0x1A, 0x86, 0xF1, 0x7C, 0x92, 0x6D, 0x9C, 0xFB,
        0xF6, 0xFC, 0x6C, 0x72, 0x55, 0x08, 0xC2, 0x6B, 0x57, 0x77, 0x3C, 0xE0,
        0xFC, 0xAE, 0xF7, 0x15, 0xB2, 0xF4, 0x90, 0x04, 0x9E, 0x9D, 0xC4, 0xC5,
        0x03, 0xC4, 0x1B, 0xD9, 0x16, 0x90, 0x79, 0x84, 0x2F, 0x5D, 0x8B, 0x5E,
        0x68, 0xEF, 0x8A, 0xDD, 0x7A, 0xB8, 0x25, 0x42, 0x70, 0x27, 0x91, 0xD3,
        0xF2, 0x4D, 0xF8, 0x56, 0x69, 0xD4, 0x91, 0xE1, 0x14, 0x65, 0x30, 0xB6,
        0x61, 0x74, 0x6B, 0xF6, 0x42, 0x8B, 0x52, 0xAA, 0x11, 0xD2, 0xD8, 0xD7,
        0x09, 0xE8, 0x09, 0x04, 0x89, 0x54, 0x99, 0x3E, 0xD0, 0x2C, 0x4E, 0x98,
        0x26, 0x87, 0xDA, 0x21, 0xCE, 0xDC, 0x01, 0xA9, 0x27, 0xD6, 0x9E, 0xC5,
        0x95, 0xA3, 0x08, 0xA5, 0x10, 0x1C, 0x7E, 0xE6, 0x94, 0xC5, 0x86, 0x77,
        0x12, 0x9C, 0xB4, 0x7D, 0xFA, 0x48, 0xA0, 0xF3, 0x39, 0xC9, 0x03, 0x6C,
        0xB2, 0x38, 0xE5, 0x5F, 0xF1, 0x94, 0x8D, 0xE6, 0x18, 0x68, 0x11, 0x74,
        0x38, 0x1B, 0x04, 0xBC, 0x9F, 0xF5, 0xE9, 0x37, 0xB4, 0xD6, 0xF0, 0xD5,
        0x09, 0xDC, 0xBE, 0x3A, 0xCF, 0x7F, 0x27, 0xF2, 0xCA, 0x90, 0xC0, 0xB1,
        0xAF, 0xBE, 0x9F, 0x17, 0x69, 0xF2, 0x99, 0x67, 0xBA, 0xE1, 0xF2, 0x4E,
        0xFE, 0x30, 0x85, 0x9A, 0xD3, 0xBA, 0x29, 0xE6, 0x06, 0x18, 0x2D, 0xEF,
        0x1A, 0xD4, 0xE0, 0xEF, 0x38, 0x80, 0xBD, 0x8F, 0xB8, 0xB3, 0x93, 0xAE,
        0x9A, 0xF6, 0x6A, 0xB9, 0x0A, 0xF3, 0x76, 0xE9, 0xF2, 0xED, 0x35, 0x37,
        0x80, 0xAB, 0x0A, 0xA1, 0x46, 0xC3, 0x2B, 0x57, 0xE5, 0x52, 0xD3, 0xD9,
        0xFB, 0x82, 0x83, 0xA8, 0x6A, 0x9A, 0x6D, 0xC3, 0xF5, 0x69, 0x21, 0x62,
        0x47, 0x81, 0x66, 0x2B, 0x94, 0x8E, 0x8D, 0xC6, 0xDF, 0x25, 0x37, 0xD8,
        0xC0, 0x8D, 0xCA, 0x75, 0x7D, 0xF0, 0xFE, 0xE0, 0x8F, 0x34, 0x20, 0x30,
        0xC5, 0x41, 0xE9, 0xD7, 0x00, 0xDA, 0x18, 0x0F, 0xCA, 0xCA, 0x09, 0x99,
        0x03, 0x4C, 0x5F, 0x1B, 0x6E, 0xA6, 0x1B, 0x61, 0x8D, 0x5C, 0xC2, 0xE3,
        0x54, 0x16, 0xFE, 0x87, 0x9E, 0x09, 0xED, 0x20, 0x51, 0x62, 0xEF, 0x2E,
        0x47, 0x27, 0xB6, 0xF8, 0x16, 0x37, 0x86, 0xA8, 0x0A, 0x4E, 0x65, 0x2E,
        0x15, 0x55, 0x98, 0x17, 0xF0, 0xA7, 0x21, 0xD1, 0x5B, 0x5A, 0x54, 0xF0,
        0xA3, 0x9D, 0x9F, 0xED, 0x10, 0x7B, 0x60, 0xFE, 0x9B, 0x59, 0xFC, 0x3A,
        0x05, 0x33, 0xC1, 0xA3, 0x0A, 0xA2, 0x67, 0x96, 0x0C, 0x9F, 0x63, 0xBD,
        0xA0, 0xE3, 0x47, 0xE6, 0xB2, 0x04, 0x13, 0x0E, 0xEC, 0x1C, 0xA3, 0x26,
        0x73, 0x52, 0x43, 0xD1, 0x41, 0x53, 0x84, 0x33, 0xF5, 0x83, 0x33, 0xBE,
        0x7F, 0xE0, 0xE5, 0x22, 0xD2, 0xAE, 0x16, 0xB5, 0xC1, 0xD3, 0xDA, 0xE7,
        0xEC, 0x2E, 0x0B, 0x7E, 0x35, 0xD2, 0x09, 0x77, 0x97, 0x33, 0x92, 0x48,
        0xA6, 0x62, 0x6B, 0x08, 0x43, 0x08, 0x27, 0x05, 0x32, 0x3D, 0x07, 0x48,
        0x8E, 0xDB, 0x22, 0x3E, 0xEB, 0xD2, 0x0F, 0x7B, 0x1D, 0x53, 0x99, 0x91,
        0xFC, 0x9E, 0x7E, 0xA4, 0x31, 0xBF, 0xB1, 0x76, 0x31, 0x6F, 0x7B, 0xEE,
        0xC0, 0xE2, 0x6D, 0xDC, 0x2C, 0x4D, 0x0D, 0x12, 0x8B, 0x1A, 0x7D, 0x1C,
        0x42, 0x21, 0x19, 0xD9, 0x66, 0xCE, 0x51, 0x52, 0xCF, 0x0A, 0xDD, 0x26,
        0x04, 0x3F, 0x07, 0x18, 0xA3, 0xF0, 0x01, 0x59, 0xA0, 0xFE, 0x24, 0xFC,
        0x4B, 0x47, 0xD4, 0xE9, 0xDD, 0x7B, 0xDA, 0xBB, 0xF6, 0x45, 0xCB, 0x66,
        0x90, 0x7A, 0x13, 0x66, 0x36, 0xA0, 0x10, 0xA0, 0x6C, 0xF9, 0xFF, 0xA3,
        0x4F, 0x19, 0xC8, 0x61, 0x2B, 0x0B, 0x3E, 0x55, 0xE2, 0x4D, 0xB5, 0x72,
        0xE9, 0xEF, 0xAE, 0x59, 0xB4, 0xDA, 0x01, 0xDA, 0x91, 0x8B, 0xC4, 0xDA,
        0x7A, 0x6E, 0xA6, 0xBB, 0xE2, 0x9D, 0x81, 0xA3, 0xCE, 0x0F, 0x95, 0x1F,
        0x97, 0xD6, 0xC4, 0xA3, 0xCE, 0x8D, 0xE2, 0xC4, 0xEE, 0x79, 0x01, 0x53,
        0x68, 0xC0, 0x7C, 0xAA, 0x6A, 0x14, 0x44, 0x1B, 0x09, 0xD2, 0x67, 0xB2,
        0xBE, 0xF1, 0x4F, 0x98, 0x6B, 0x79, 0xFD, 0x90, 0x5F, 0x2A, 0xE4, 0xE0,
        0xFB, 0x33, 0xD0, 0x36, 0x82, 0xF4, 0xC2, 0x13, 0x6D, 0xA8, 0x19, 0x3B,
        0x56, 0xD2, 0x0A, 0xFF, 0x47, 0xCB, 0x6A, 0xF0, 0xA0, 0xBF, 0x4F, 0x0D,
        0xD3, 0x6F, 0x24, 0x53, 0xD5, 0x6D, 0x29, 0xF9, 0x4A, 0x5D, 0xBB, 0x42,
        0x75, 0x6E, 0xD8, 0xD4, 0x50, 0x26, 0x44, 0x96, 0x51, 0xBC, 0xDE, 0x0B,
        0x8D, 0xE9, 0xCE, 0xB3, 0xE2, 0x83, 0x8C, 0x18, 0x50, 0x28, 0x63, 0xBE,
        0x59, 0xE6, 0x0C, 0x9A, 0xCD, 0x50, 0x12, 0xC5, 0x77, 0x9F, 0x1B, 0x14,
        0xD4, 0x44, 0x28, 0x29, 0xD4, 0x50, 0xCE, 0xA8, 0xA3, 0x71, 0xF4, 0xE6,
        0xF9, 0x7E, 0x1B, 0xE5, 0x79, 0x3B, 0xE3, 0xEC, 0x6A, 0x7A, 0xC1, 0x1E,
        0x5E, 0x12, 0xA0, 0x01, 0x04, 0x1E, 0x07, 0x09, 0xF4, 0x2D, 0xE7, 0xBF,
        0xCF, 0xD1, 0x0E, 0x17, 0xAC, 0x59, 0xA2, 0x2F, 0x4F, 0x70, 0x21, 0x48,
        0x04, 0x27, 0xB8, 0x95, 0x6A, 0x0C, 0x1E, 0xCD, 0xCE, 0x33, 0x7A, 0xB5,
        0xFE, 0x04, 0x72, 0x05, 0x6B, 0xDE, 0x17, 0x76, 0xD5, 0xF1, 0x72, 0xAA,
        0x1A, 0xD6, 0x46, 0x4B, 0x78, 0x73, 0x79, 0xAC, 0xCF, 0xD9, 0x84, 0x49,
        0x22, 0x60, 0x72, 0x40, 0x5C, 0x5B, 0x42, 0x2B, 0xAB, 0x5D, 0xA2, 0x4C,
        0xA3, 0x3A, 0xE2, 0xA0, 0x7A, 0x52, 0xE1, 0x4F, 0xAC, 0xB7, 0xD3, 0x6F,
        0x60, 0xBA, 0x47, 0xEB, 0xB7, 0xC8, 0xE7, 0xCD, 0x0F, 0x5D, 0xEF, 0x9A,
        0xCD, 0x74, 0x16, 0x82, 0x7D, 0xEA, 0x58, 0xC2, 0xA5, 0x13, 0xA8, 0xA6,
        0x84, 0x8A, 0xE8, 0x93, 0xE3, 0xD2, 0x32, 0x8F, 0x0D, 0x44, 0xAE, 0x58,
        0x15, 0x97, 0x79, 0xC5, 0xD0, 0x84, 0x52, 0x42, 0x64, 0x6E, 0x69, 0x1B,
        0x3A, 0xE9, 0x3C, 0x7D, 0x5C, 0xF2, 0xD0, 0xDA, 0x14, 0xDD, 0xB0, 0xC4,
        0xE5, 0xE2, 0x79, 0x70, 0x1D, 0xE6, 0xE8, 0xA9, 0xB7, 0x86, 0xF6, 0xFA,
        0x7B, 0xB8, 0xF1, 0x1C, 0xC7, 0x43, 0xAB, 0x31, 0xBB, 0xD1, 0x45, 0xDC,
        0xEC, 0xB6, 0x6C, 0x38, 0xF2, 0x83, 0xD2, 0xCA, 0xAF, 0xBF, 0xCC, 0x4D,
        0x8B, 0xF2, 0x34, 0x49, 0xD4, 0x3D, 0xBC, 0x2F, 0xF7, 0x64, 0x9B, 0x78,
        0x8F, 0x91, 0xE8, 0xB2, 0xF7, 0xCB, 0x6A, 0x50, 0x60, 0x33, 0xA1, 0x50,
        0xD3, 0xD4, 0x28, 0xD1, 0x26, 0x75, 0x68, 0xBA, 0x40, 0x49, 0x47, 0xC5,
        0xC7, 0xDF, 0xB5, 0x6B, 0x1F, 0xDF, 0x9B, 0xEF, 0x3B, 0x64, 0xCC, 0x92,
        0xF0, 0xD4, 0x2A, 0xFB, 0x0B, 0x41, 0xF9, 0x4F, 0x1E, 0xC8, 0x13, 0xB0,
        0x74, 0x95, 0xE6, 0x9F, 0x88, 0x0D, 0xED, 0x5F, 0x20, 0x77, 0xE8, 0xCF,
        0x06, 0xDF, 0xCD, 0x09, 0x6F, 0x69, 0x57, 0x9E, 0xB9, 0xDF, 0x29, 0xC4,
        0xD2, 0x5D, 0x0C, 0x46, 0x5B, 0x50, 0x9E, 0x9B, 0xF6, 0xBC, 0x12, 0xC0,
        0xDA, 0xBD, 0x55, 0xD9, 0x31, 0xED, 0x10, 0x73, 0xE2, 0xEB, 0xAA, 0xCD,
        0xF6, 0x1F, 0x76, 0xC9, 0x45, 0x0F, 0xE0, 0x5D, 0x49, 0xE9, 0xA5, 0x66,
        0x1D, 0xDF, 0xC5, 0x45, 0xAB, 0x8F, 0xA3, 0x96, 0x5B, 0x9D, 0x93, 0x18,
        0x83, 0x50, 0x01, 0x94, 0x79, 0x13, 0x7B, 0xD2, 0xE7, 0x07, 0x01, 0xE1,
        0xF8, 0x6E, 0xEF, 0xDA, 0xB3, 0x83, 0x8F, 0xAF, 0x29, 0x70, 0xD3, 0xAF,
        0xE0, 0x03, 0x68, 0x7D, 0x55, 0x50, 0x54, 0x3A, 0x25, 0x99, 0x41, 0xFD,
        0x5F, 0xA2, 0x9F, 0xB4, 0xF4, 0xD3, 0x3A, 0x53, 0xE4, 0x3F, 0x9F, 0xC4,
        0x96, 0xB1, 0x9E, 0x9B, 0x8E, 0x40, 0x15, 0x20, 0x85, 0x64, 0xF4, 0xD6,
        0x79, 0x64, 0xE6, 0xA8, 0xE1, 0x66, 0x88, 0x5B, 0x8D, 0x47, 0x7D, 0xC9,
        0x0A, 0xB3, 0xFC, 0xB7, 0xF0, 0xDF, 0xEF, 0xDD, 0x4C, 0xB9, 0xAF, 0x34,
        0x2F, 0xD9, 0x55, 0xA1, 0xB1, 0x79, 0x39, 0xF5, 0x75, 0xD0, 0xCD, 0xF0,
        0xAE, 0xCC, 0x5A, 0xF2, 0xFD, 0x8A, 0x20, 0xE4, 0x77, 0xBB, 0x11, 0x5B,
        0xE4, 0x37, 0xB3, 0x37, 0xBE, 0xA8, 0x9D, 0xA6, 0x86, 0xEE, 0xA9, 0x28,
        0x0E, 0xB3, 0x84, 0xB2, 0x98, 0x75, 0x52, 0xC3, 0x6E, 0x93, 0x23, 0xF4,
        0x51, 0x1D, 0x2E, 0x7E, 0x3D, 0x61, 0xF2, 0xCA, 0x1E, 0x39, 0x05, 0x75,
        0xD5, 0x3D, 0x2D, 0x25, 0x03, 0x82, 0xD1, 0x00, 0x7E, 0x57, 0xF4, 0x7E,
        0xA9, 0x98, 0x4E, 0x24, 0x40, 0x58, 0x3B, 0xB7, 0xED, 0xC9, 0xF9, 0xB0,
        0x23, 0xBC, 0xD4, 0xA8, 0x43, 0x82, 0x22, 0xB7, 0xD3, 0x18, 0x14, 0x24,
        0x29, 0x44, 0xFD, 0x39, 0x37, 0x9B, 0xAB, 0xE8, 0xDF, 0xEC, 0x74, 0x3A,
        0x9C, 0x49, 0x99, 0xBF, 0x26, 0xEB, 0xC3, 0x70, 0x10, 0x5D, 0xAC, 0xC6,
        0x0A, 0x41, 0x85, 0x70, 0x24, 0xCA, 0xC5, 0x9B, 0x32, 0x4D, 0xCF, 0x37,
        0xF1, 0x7A, 0x40, 0x5A, 0xAF, 0xC2, 0x38, 0x7F, 0x26, 0x57, 0xF9, 0x6C,
        0xE9, 0xCC, 0x34, 0x29, 0x5E, 0x4A, 0x8E, 0x47, 0x71, 0xCD, 0xBC, 0x86,
        0x04, 0x38, 0x67, 0x46, 0xDA, 0xE9, 0xD7, 0x5C, 0x47, 0x0E, 0x1C, 0x60,
        0x5C, 0xC9, 0xF3, 0x32, 0x07, 0xB2, 0xF3, 0x98, 0xF2, 0x09, 0x9F, 0x2A,
        0x9B, 0x3B, 0x8E, 0xA6, 0x44, 0x7F, 0xA6, 0xE0, 0xEB, 0x01, 0x48, 0x8D,
        0x00, 0x09, 0x65, 0x0D, 0x8F, 0x9D, 0xBD, 0x2D, 0x47, 0xFE, 0x13, 0x91,
        0x0E, 0x7B, 0x31, 0x5A, 0x71, 0x8D, 0x5A, 0x3D, 0x45, 0x0E, 0xD6, 0xFB,
        0xDC, 0xEE, 0xF6, 0x86, 0xC6, 0xFE, 0x77, 0x3F, 0xB2, 0x0B, 0x6A, 0xE5,
        0xF7, 0x5E, 0xAB, 0x32, 0x29, 0xC4, 0x62, 0x22, 0xC6, 0x87, 0xD3, 0x7E,
        0x7E, 0xC8, 0x87, 0x00, 0xB7, 0xE4, 0x70, 0x65, 0xE1, 0x80, 0x8D, 0x64,
        0x1A, 0x76, 0xBD, 0xC3, 0x47, 0x0C, 0x3C, 0x5C, 0x7C, 0xB0, 0xC5, 0x9A,
        0x8F, 0x15, 0xCB, 0x6E, 0x23, 0xD3, 0xE2, 0x73, 0x5E, 0x43, 0x95, 0x50,
        0xE4, 0x7E, 0xA9, 0x21, 0x19, 0x95, 0x59, 0x0C, 0x41, 0x64, 0x6C, 0x1F,
        0xD7, 0xEF, 0x52, 0xEE, 0xE2, 0x6E, 0x88, 0xBD, 0x66, 0x3C, 0xEC, 0x66,
        0x0C, 0x82, 0x42, 0x0E, 0xCB, 0x44, 0xF2, 0xB4, 0xFC, 0x2E, 0x6F, 0xA5,
        0x16, 0x2F, 0x30, 0xC0, 0xAA, 0x95, 0xB7, 0xF9, 0x3F, 0x0F, 0xA0, 0x60,
        0xA9, 0xB0, 0x3F, 0xA8, 0x24, 0xF7, 0xB2, 0xE9, 0x4F, 0xB4, 0xC3, 0xA3,
        0x80, 0xCC, 0x51, 0x81, 0x9C, 0xE9, 0xE0, 0x2A, 0x00, 0xFF, 0xB0, 0x0C,
        0x6D, 0x64, 0x9A, 0x2E, 0xD6, 0x2C, 0x1D, 0x99, 0x6B, 0xF1, 0x2B, 0xEF,
        0xD2, 0x0A, 0x61, 0x0A, 0x07, 0xEA, 0x74, 0x16, 0xE7, 0xCF, 0x7F, 0x56,
        0xAF, 0xF5, 0x5E, 0xF0, 0xCB, 0x47, 0xDF, 0xDF, 0x59, 0xBB, 0x3E, 0x6C,
        0xAD, 0x2C, 0x05, 0x2F, 0x04, 0xCA, 0x01, 0x47, 0x4A, 0x16, 0x65, 0x0C,
        0xB3, 0xEC, 0x85, 0xA2, 0x0A, 0xD2, 0x8F, 0x34, 0xFF, 0xF8, 0x15, 0x79,
        0x33, 0x9D, 0x26, 0xDC, 0x72, 0x8F, 0x57, 0x74, 0x80, 0xED, 0x3C, 0x76,
        0x51, 0x59, 0x27, 0x53, 0xBD, 0xEE, 0x51, 0x8C, 0x52, 0x44, 0x8B, 0x6D,
        0x4E, 0xC3, 0x67, 0x52, 0x7B, 0x19, 0xB7, 0xE9, 0xAB, 0x00, 0x28, 0x91,
        0xA4, 0x07, 0xD2, 0x10, 0xF7, 0xEA, 0x38, 0x42, 0x12, 0xBD, 0x71, 0x21,
        0x0B, 0xFF, 0x7E, 0x9C, 0xFF, 0x79, 0x1B, 0x25, 0x53, 0x77, 0xBB, 0x41,
        0x83, 0x65, 0xEA, 0xEC, 0xE5, 0x93, 0x13, 0x03, 0xE9, 0x8F, 0xA5, 0x49,
        0x87, 0x75, 0xBB, 0x83, 0x56, 0x31, 0x2A, 0x4A, 0xA7, 0x8D, 0xAB, 0xA3,
        0xAA, 0x74, 0xF7, 0x38, 0x4E, 0x4D, 0x84, 0xD3, 0x18, 0x14, 0xC8, 0x7C,
        0x9D, 0xA0, 0x4C, 0x89, 0x56, 0xDE, 0x1A, 0x34, 0xA5, 0xD6, 0x74, 0x95,
        0x51, 0x97, 0x29, 0xD2, 0x93, 0x32, 0xD4, 0xDB, 0xE8, 0x53, 0x3D, 0x25,
        0xC4, 0x5C, 0x00, 0x64, 0x6F, 0xE5, 0x3D, 0x81, 0xDA, 0x3C, 0x1C, 0x63,
        0xBD, 0x81, 0x16, 0x93, 0xB8, 0x0A, 0xDD, 0x77, 0x6F, 0xB5, 0xD6, 0x15,
        0xB1, 0x06, 0x31, 0x2A, 0xBF, 0x02, 0x31, 0x60, 0x3D, 0x9A, 0xF2, 0xFE,
        0x38, 0xAA, 0x8B, 0x64, 0x91, 0xE7, 0x0A, 0x47, 0xAB, 0x25, 0xE6, 0x02,
        0x5D, 0x4D, 0x90, 0x04, 0xA4, 0xCE, 0x31, 0x3B, 0x6D, 0x12, 0xA2, 0xA9,
        0x75, 0x45, 0x81, 0x76, 0x11, 0x55, 0x6A, 0x4C, 0x19, 0xC6, 0xD7, 0xE2,
        0xA5, 0x8C, 0x0D, 0x84, 0xF8, 0xE8, 0x15, 0x4D, 0x95, 0x6A, 0x25, 0x59,
        0x56, 0x6A, 0xED, 0xF1, 0xAB, 0xEF, 0x96, 0xB9, 0x13, 0xC9, 0xA9, 0x5D,
        0xD8, 0xBE, 0x52, 0x76, 0xBE, 0xA0, 0xBD, 0xED, 0x08, 0x19, 0xF8, 0x59,
        0xD7, 0x5E, 0xD4, 0xD1, 0x76, 0xF8, 0xB5, 0x00, 0xE4, 0x97, 0x6B, 0x98,
        0x62, 0x98, 0xF9, 0x0B, 0xC6, 0x48, 0xA4, 0xF3, 0x73, 0x25, 0xF9, 0x05,
        0xC4, 0xC7, 0xB2, 0x9A, 0xF2, 0xBB, 0x92, 0xBF, 0x36, 0xE0, 0xE5, 0xA8,
        0x2B, 0xA0, 0x30, 0x30, 0x3B, 0x83, 0x1E, 0xAB, 0x39, 0xD0, 0x90, 0x51,
        0xC2, 0xEE, 0x31, 0x3D, 0x27, 0xF4, 0xAD, 0x4E, 0x17, 0xD2, 0x65, 0xEA,
        0xD3, 0x4D, 0xAC, 0xAF, 0xFE, 0xF6, 0x5E, 0xDF, 0x6C, 0xDD, 0x8C, 0xE8,
        0xDF, 0xD1, 0x55, 0xAB, 0x8B, 0x9E, 0x68, 0x77, 0x81, 0xB0, 0xA6, 0x3B,
        0x08, 0x79, 0xD3, 0x9E, 0x78, 0x36, 0x38, 0x42, 0xA6, 0x21, 0xF2, 0xF9,
        0x5D, 0x35, 0xCF, 0xB6, 0x09, 0xAA, 0x10, 0x1C, 0x86, 0x54, 0x64, 0x38,
        0x0E, 0x32, 0x46, 0xAB, 0xCE, 0xBD, 0x01, 0x6C, 0xBF, 0x77, 0x9B, 0xAA,
        0x85, 0x70, 0x8A, 0xDC, 0xC0, 0x40, 0x3C, 0x84, 0x12, 0x8B, 0x21, 0x35,
        0x35, 0x5A, 0xC1, 0x57, 0xD4, 0x79, 0xA8, 0x37, 0x71, 0x86, 0xFA, 0x41,
        0xF3, 0xE4, 0xBA, 0xE9, 0x23, 0xC9, 0x21, 0x95, 0xB3, 0xBA, 0x32, 0x2A,
        0x6B, 0xB4, 0x15, 0x54, 0xDD, 0x45, 0x72, 0xCB, 0xBD, 0x07, 0x64, 0x6E,
        0x82, 0x77, 0x47, 0x88, 0xA6, 0x4E, 0x42, 0x18, 0x27, 0x44, 0x21, 0x48,
        0xB8, 0xB8, 0x66, 0xD2, 0xD4, 0x96, 0xB7, 0x70, 0x23, 0x82, 0xE4, 0xE4,
        0x45, 0xBF, 0xA5, 0xF6, 0x00, 0xC4, 0x4C, 0xA3, 0x1C, 0xF2, 0x62, 0x0B,
        0xA7, 0x6C, 0xEB, 0xF0, 0xA3, 0x66, 0x85, 0x4F, 0x04, 0x59, 0x3E, 0xD2,
        0xFD, 0xC0, 0xEA, 0x86, 0x7D, 0x0A, 0x08, 0x95, 0x24, 0x6A, 0x92, 0xE2,
        0xA1, 0xC4, 0x83, 0x99, 0x84, 0x8B, 0x39, 0x7E, 0xF1, 0xB2, 0xCC, 0x9F,
        0x6B, 0x69, 0x70, 0xFF, 0xF7, 0x7A, 0xB4, 0xCC, 0x96, 0xBF, 0xD3, 0x61,
        0x85, 0x03, 0x17, 0xF4, 0xEF, 0x6F, 0xC8, 0x0B, 0x90, 0x5C, 0xFB, 0x06,
        0xFE, 0xD9, 0x92, 0xCE, 0xCF, 0xA3, 0x0C, 0x12, 0x59, 0x2F, 0xDE, 0xDC,
        0x38, 0x2B, 0xFD, 0xD6, 0x74, 0xD9, 0x58, 0x42, 0x3C, 0x0A, 0x2C, 0x4B,
        0xEC, 0x67, 0x09, 0x1F, 0x51, 0x7E, 0x98, 0x25, 0xC7, 0xCF, 0x7A, 0x4E,
        0x94, 0x96, 0x7F, 0x1F, 0x30, 0x32, 0xDE, 0x18, 0x5D, 0x09, 0xD9, 0x85,
        0x16, 0x76, 0x73, 0x54, 0x56, 0x69, 0xC2, 0x6E, 0x7B, 0x63, 0xB3, 0x9E,
        0x92, 0x51, 0x8A, 0x90, 0x3E, 0xB4, 0x9F, 0x83, 0x2A, 0x0F, 0xE7, 0xEF,
        0xA5, 0x68, 0xE1, 0x42, 0x22, 0x5C, 0x6E, 0x77, 0xA9, 0xE3, 0x6B, 0x73,
        0xED, 0x66, 0x2D, 0xB7, 0x94, 0x47, 0xF9, 0xA4, 0x55, 0xBD, 0x14, 0xB4,
        0xD3, 0x23, 0xE6, 0x5D, 0xA9, 0xE1, 0xD1, 0x9C, 0x7E, 0x7F, 0x95, 0x42,
        0x93, 0xF5, 0x1C, 0x38, 0x07, 0x7D, 0x8A, 0x67, 0x04, 0xB6, 0x8B, 0x15,
        0xBD, 0x26, 0x49, 0xE6, 0x38, 0x74, 0x04, 0x5A, 0xCB, 0x68, 0x0D, 0x36,
        0x8E, 0x7A, 0x48, 0xAB, 0x0B, 0x9A, 0x76, 0x0D, 0x39, 0xC9, 0x3F, 0xDE,
        0xC0, 0xA9, 0x3E, 0xF3, 0x55, 0x74, 0xB8, 0x28, 0x0C, 0xC0, 0xF6, 0xD5,
        0xFE, 0x8F, 0x3F, 0x85, 0xC3, 0x38, 0xD2, 0xC8, 0x3E, 0x47, 0xB5, 0x3B,
        0x97, 0x90, 0x77, 0xC9, 0x0C, 0x3A, 0xAF, 0xC4, 0xC0, 0x98, 0x90, 0x89,
        0xE4, 0xC0, 0x51, 0x3C, 0x4E, 0x21, 0x02, 0x7E, 0xA4, 0x85, 0x5F, 0xE6,
        0x57, 0xA8, 0xC4, 0xE7, 0x9E, 0x17, 0x96, 0x60, 0x59, 0xB5, 0x9E, 0x9C,
        0x7D, 0x5F, 0x9A, 0x70, 0xD5, 0x6F, 0x8F, 0x54, 0xCB, 0x58, 0xAD, 0xF1,
        0x83, 0xE7, 0xBC, 0xE4, 0x6C, 0x2B, 0xCA, 0x43, 0x8F, 0xDE, 0x43, 0x6F,
        0xF9, 0x02, 0x0A, 0xA6, 0x35, 0x0F, 0xDD, 0xBC, 0xB3, 0x82, 0xF5, 0x58,
        0x0D, 0x3B, 0x63, 0x30, 0x49, 0x7D, 0x7C, 0xF1, 0xF7, 0x09, 0xF8, 0x35,
        0x17, 0x04, 0xE9, 0x9E, 0x4F, 0x67, 0x60, 0x63, 0x7B, 0x5B, 0x37, 0x44,
        0x97, 0x9C, 0x94, 0x72, 0xD2, 0xDC, 0x78, 0xFF, 0x9A, 0xF5, 0xBF, 0x08,
        0x4E, 0x4D, 0xB1, 0x00, 0xC1, 0xF8, 0x7E, 0xF3, 0x10, 0xA0, 0x60, 0x67,
        0xEE, 0x4D, 0xDD, 0x87, 0x8A, 0x20, 0xF5, 0x31, 0xDD, 0x3A, 0x1C, 0x72,
        0x46, 0xAB, 0xB9, 0x6E, 0x41, 0x03, 0x25, 0x29, 0x21, 0xC7, 0xDC, 0x2F,
        0xBB, 0x2E, 0x81, 0x9D, 0xA0, 0x78, 0xA0, 0x60, 0x2D, 0xDC, 0xFC, 0x20,
        0xF9, 0x94, 0xC5, 0xE8, 0x88, 0xE4, 0x56, 0x49, 0x03, 0x38, 0x87, 0x81,
        0x3D, 0x95, 0x1A, 0xF5, 0xE1, 0xC7, 0x43, 0x9B, 0x40, 0x54, 0xD9, 0xDF,
        0x7E, 0xB0, 0xBD, 0x8D, 0x2F, 0xF2, 0xCA, 0x1E, 0xCA, 0xCE, 0x4D, 0xB1,
        0x3D, 0x00, 0xC8, 0xC4, 0x1B, 0x05, 0x26, 0xDC, 0x11, 0x10, 0xDC, 0x8B,
        0xDB, 0x04, 0xF2, 0x6D, 0xCF, 0x91, 0x24, 0xC7, 0x23, 0x46, 0xD7, 0x9F,
        0x7F, 0xC5, 0x63, 0xA6, 0x6D, 0xF9, 0xF6, 0xD0, 0x49, 0xD2, 0xF5, 0x60,
        0x81, 0x6B, 0xB6, 0x10, 0x37, 0x64, 0xB2, 0xEE, 0x2C, 0x33, 0x39, 0xE7,
        0xFD, 0xBD, 0x94, 0xEB, 0xE3, 0xE0, 0xC4, 0xBF, 0xA8, 0xE7, 0x8E, 0x5D,
        0xC3, 0xC5, 0x76, 0xF6, 0x61, 0x70, 0x35, 0x38, 0x68, 0x4D, 0x3E, 0x8D,
        0x0B, 0x28, 0x60, 0xD5, 0x38, 0xB4, 0x85, 0x7A, 0x19, 0xDA, 0xE7, 0x00,
        0x1B, 0x02, 0xE3, 0x44, 0x73, 0xEE, 0x2B, 0xB2, 0x14, 0x11, 0xF1, 0x7A,
        0xD3, 0xAC, 0x4A, 0x9A, 0x7A, 0xA9, 0x5E, 0xBB, 0x0B, 0xAC, 0x93, 0x79,
        0xD7, 0x2E, 0xC1, 0x3D, 0xA8, 0xED, 0xC7, 0x7A, 0x57, 0x8E, 0x52, 0x26,
        0xC9, 0x78, 0x5B, 0xB1, 0x77, 0xB1, 0xE2, 0xEA, 0x3C, 0x7A, 0xBD, 0xA9,
        0x80, 0x21, 0x81, 0x03, 0x1F, 0x45, 0xD1, 0x0E, 0x7D, 0xC5, 0xB1, 0x9A,
        0xC5, 0xC3, 0xAC, 0xA0, 0x3D, 0x53, 0x0B, 0xBF, 0x13, 0x84, 0x7F, 0x01,
        0x2A, 0x45, 0x28, 0x32, 0xE1, 0x48, 0xEB, 0x86, 0xFF, 0xBC, 0x9C, 0x37,
        0x75, 0x0F, 0x20, 0x3B, 0x2B, 0xB6, 0xBB, 0x0C, 0x7F, 0x62, 0x67, 0x74,
        0xE3, 0xB5, 0x82, 0xC7, 0x37, 0xC5, 0x02, 0xD4, 0x42, 0xE8, 0x6B, 0x02,
        0x7A, 0x2D, 0xA3, 0x41, 0xFF, 0xB6, 0x10, 0x59, 0xB4, 0x26, 0x99, 0x90,
        0x1E, 0x81, 0xBD, 0x05, 0xC8, 0x98, 0xFD, 0x92, 0x80, 0xE9, 0x66, 0xCF,
        0x9C, 0xB7, 0x5E, 0x1F, 0xE4, 0x31, 0x2C, 0x4E, 0x0C, 0x7F, 0x9B, 0x5F,
        0x91, 0x2E, 0x97, 0x91, 0x5A, 0x2E, 0x0F, 0xF7, 0x27, 0xB6, 0xB6, 0xD5,
        0x3A, 0x24, 0x1F, 0x3B, 0x7B, 0xCB, 0x04, 0xF4, 0xC3, 0x2E, 0x23, 0xE6,
        0x06, 0x89, 0xDA, 0x22, 0xFF, 0x48, 0xB3, 0x0C, 0x9D, 0x34, 0xA6, 0x0D,
        0x98, 0x8B, 0x85, 0x7C, 0xF1, 0x58, 0xB2, 0xD9, 0xD4, 0x9D, 0x3C, 0x62,
        0x26, 0xC6, 0x93, 0x84, 0x3B, 0x1F, 0x55, 0x91, 0x53, 0x7A, 0xD9, 0x6E,
        0x32, 0x0C, 0xA1, 0xB5, 0x3E, 0x9F, 0xD3, 0x23, 0x04, 0xDA, 0xE7, 0x57,
        0x89, 0x26, 0x0B, 0x8F, 0x29, 0x45, 0xB5, 0xEB, 0x72, 0x55, 0x01, 0x0F,
        0x33, 0xB8, 0x4D, 0x2F, 0x2B, 0x1D, 0x7E, 0x40, 0x1A, 0x5F, 0x81, 0xFD,
        0x64, 0xBB, 0xAC, 0xDE, 0x59, 0x88, 0xC3, 0xE8, 0x0B, 0x11, 0x04, 0xDE,
        0xAB, 0x01, 0xC9, 0xB6, 0x1A, 0xEE, 0x3A, 0x15, 0x20, 0xF7, 0x5A, 0x6B,
        0x96, 0xE7, 0xBF, 0xC3, 0xC7, 0x2B, 0x41, 0x8B, 0x67, 0xE1, 0xCB, 0xE8,
        0xF2, 0x3D, 0xD8, 0x60, 0x81, 0x69, 0x50, 0x35, 0xC7, 0x1F, 0x8F, 0x2B,
        0xCF, 0x29, 0x10, 0x31, 0x6E, 0xAD, 0x6F, 0xF4, 0x6B, 0x05, 0x40, 0x74,
        0x79, 0x97, 0xDF, 0x61, 0xDA, 0x02, 0x3D, 0x69, 0xCC, 0x3C, 0xD9, 0xE7,
        0xD3, 0xD3, 0x1B, 0x86, 0xA0, 0x85, 0x0B, 0xAD, 0x7A, 0x27, 0xCD, 0x8B,
        0xB7, 0xCE, 0x2F, 0xB7, 0x4F, 0xD8, 0xA5, 0x8C, 0x50, 0x71, 0x84, 0xC0,
        0x28, 0x84, 0xD8, 0x1B, 0x0D, 0xCA, 0x73, 0x46, 0xE5, 0xDB, 0x30, 0xB0,
        0xC1, 0x06, 0x1C, 0xB5, 0xFB, 0xAF, 0x2B, 0x64, 0x3B, 0x04, 0xB5, 0xCC,
        0xCB, 0xEB, 0x4A, 0xB8, 0x52, 0x03, 0xD8, 0xBC, 0xD6, 0x20, 0x55, 0x38,
        0xAA, 0x18, 0x45, 0x3B, 0xBE, 0x1B, 0xD9, 0xFE, 0x32, 0x89, 0xF6, 0x95,
        0x0E, 0xAE, 0x10, 0x3A, 0xAF, 0xAE, 0x33, 0xD7, 0xA7, 0x5B, 0xF8, 0xF9,
        0xD1, 0xC3, 0x8A, 0x97, 0x0C, 0x54, 0x64, 0x6A, 0x5C, 0x57, 0xA1, 0xC5,
        0x2B, 0xCD, 0xAD, 0xC2, 0x65, 0x54, 0xD4, 0xC3, 0x05, 0x1F, 0x67, 0x47,
        0xF3, 0xFF, 0x42, 0x85, 0x92, 0x8F, 0x52, 0x0F, 0xCD, 0xB9, 0xA8, 0x3D,
        0x28, 0x69, 0xE9, 0x13, 0x86, 0xCB, 0xE5, 0x0A, 0x23, 0x24, 0x60, 0x55,
        0x22, 0xB0, 0xCF, 0x1A, 0x6B, 0xF1, 0xC9, 0x68, 0xA5, 0x28, 0xBC, 0xBE,
        0x5E, 0xD1, 0xD1, 0xB1, 0x30, 0x41, 0xF5, 0xA9, 0x93, 0x0B, 0xBB, 0x0E,
        0xB9, 0xFD, 0x3F, 0xAF, 0xAD, 0x8A, 0x07, 0x52, 0x60, 0x21, 0xBF, 0xB3,
        0x05, 0x9D, 0x4D, 0xFB, 0xA3, 0x85, 0x27, 0xE3, 0xE5, 0x87, 0x5A, 0xD0,
        0xA5, 0x68, 0x8E, 0x68, 0x76, 0x03, 0x84, 0x38, 0x8D, 0xAC, 0x46, 0x9B,
        0xFD, 0x86, 0xF2, 0x23
    ];
}