using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GCDTool.Wram;

/// <summary>
/// Class for reading <see cref="WramConfig"/> from a JSON file.
/// </summary>
sealed class WramConfigJsonReader
{
    private const int WRAM_A_BLOCK_COUNT = 4;
    private const int WRAM_BC_BLOCK_COUNT = 8;

    private const string MASTER_ARM9 = "arm9";
    private const string MASTER_ARM7 = "arm7";
    private const string MASTER_DSP = "dsp";

    private const string SLOT_0 = "0";
    private const string SLOT_01 = "01";
    private const string SLOT_0123 = "0123";
    private const string SLOT_01234567 = "01234567";
    private const string SLOT_ALL = "all";

    /// <summary>
    /// Reads a <see cref="WramConfig"/> from the given <paramref name="jsonString"/>.
    /// </summary>
    /// <param name="jsonString">The JSON string containing the WRAM configuration.</param>
    /// <returns>The constructed <see cref="WramConfig"/>.</returns>
    public WramConfig ReadWramConfigFromJson(string jsonString)
    {
        var wramConfig = new WramConfig();
        var json = JsonDocument.Parse(jsonString).RootElement;
        ParseWramAConfig(wramConfig, json.GetProperty("wramA"));
        ParseWramBConfig(wramConfig, json.GetProperty("wramB"));
        ParseWramCConfig(wramConfig, json.GetProperty("wramC"));

        var ntrWram = json.GetProperty("ntrWram");
        if (ntrWram.GetArrayLength() != 2)
        {
            throw new InvalidDataException("'ntrWram' array should have exactly 2 items.");
        }

        wramConfig.NtrWramMapping[0] = ParseNtrWramMapping(ntrWram[0].GetString());
        wramConfig.NtrWramMapping[1] = ParseNtrWramMapping(ntrWram[1].GetString());
        wramConfig.VramCMapping = json.GetProperty("vramC").GetByte();
        wramConfig.VramDMapping = json.GetProperty("vramD").GetByte();
        return wramConfig;
    }

    private void ParseWramAConfig(WramConfig wramConfig, JsonElement wramAJson)
    {
        wramConfig.Arm9TwlWramAMapping = wramAJson.TryGetProperty("arm9", out var arm9)
            ? ParseWramAMapping(arm9)
            : new WramAMapping(0x03000000, 0, WramAMappedSlots.Slot0); // unmapped

        wramConfig.Arm7TwlWramAMapping = wramAJson.TryGetProperty("arm7", out var arm7)
            ? ParseWramAMapping(arm7)
            : new WramAMapping(0x03000000, 0, WramAMappedSlots.Slot0); // unmapped

        var blocks = wramAJson.GetProperty("blocks");
        if (blocks.GetArrayLength() != WRAM_A_BLOCK_COUNT)
        {
            throw new InvalidDataException($"Wram A 'blocks' array should have exactly {WRAM_A_BLOCK_COUNT} items.");
        }
        for (int i = 0; i < WRAM_A_BLOCK_COUNT; i++)
        {
            wramConfig.TwlWramABlockMapping[i] = ParseWramABlockMapping(
                blocks[i], out wramConfig.TwlWramABlockLocked[i]);
        }
    }

    private void ParseWramBConfig(WramConfig wramConfig, JsonElement wramBJson)
    {
        wramConfig.Arm9TwlWramBMapping = wramBJson.TryGetProperty("arm9", out var arm9)
            ? ParseWramBCMapping(arm9)
            : new WramBCMapping(0x03000000, 0, WramBCMappedSlots.Slot0); // unmapped

        wramConfig.Arm7TwlWramBMapping = wramBJson.TryGetProperty("arm7", out var arm7)
            ? ParseWramBCMapping(arm7)
            : new WramBCMapping(0x03000000, 0, WramBCMappedSlots.Slot0); // unmapped

        var blocks = wramBJson.GetProperty("blocks");
        if (blocks.GetArrayLength() != WRAM_BC_BLOCK_COUNT)
        {
            throw new InvalidDataException($"Wram B 'blocks' array should have exactly {WRAM_BC_BLOCK_COUNT} items.");
        }
        for (int i = 0; i < WRAM_BC_BLOCK_COUNT; i++)
        {
            wramConfig.TwlWramBBlockMapping[i] = ParseWramBBlockMapping(
                blocks[i], out wramConfig.TwlWramBBlockLocked[i]);
        }
    }

    private void ParseWramCConfig(WramConfig wramConfig, JsonElement wramCJson)
    {
        wramConfig.Arm9TwlWramCMapping = wramCJson.TryGetProperty("arm9", out var arm9)
            ? ParseWramBCMapping(arm9)
            : new WramBCMapping(0x03000000, 0, WramBCMappedSlots.Slot0); // unmapped

        wramConfig.Arm7TwlWramCMapping = wramCJson.TryGetProperty("arm7", out var arm7)
            ? ParseWramBCMapping(arm7)
            : new WramBCMapping(0x03000000, 0, WramBCMappedSlots.Slot0); // unmapped

        var blocks = wramCJson.GetProperty("blocks");
        if (blocks.GetArrayLength() != WRAM_BC_BLOCK_COUNT)
        {
            throw new InvalidDataException($"Wram C 'blocks' array should have exactly {WRAM_BC_BLOCK_COUNT} items.");
        }
        for (int i = 0; i < WRAM_BC_BLOCK_COUNT; i++)
        {
            wramConfig.TwlWramCBlockMapping[i] = ParseWramCBlockMapping(
                blocks[i], out wramConfig.TwlWramCBlockLocked[i]);
        }
    }

    private WramAMapping ParseWramAMapping(JsonElement mappingJson)
    {
        var wramMapping = mappingJson.Deserialize<JsonWramMapping>()
            ?? throw new InvalidDataException("Could not parse wram A mapping.");
        return new(
            ParseHexString(wramMapping.Start),
            ParseHexString(wramMapping.Length),
            wramMapping.Slots switch
            {
                SLOT_0 => WramAMappedSlots.Slot0,
                SLOT_01 => WramAMappedSlots.Slot01,
                SLOT_0123 or SLOT_ALL => WramAMappedSlots.All,
                _ => throw new InvalidDataException("Invalid value specified for wram A 'slots'.")
            });
    }

    private WramBCMapping ParseWramBCMapping(JsonElement mappingJson)
    {
        var wramMapping = mappingJson.Deserialize<JsonWramMapping>()
            ?? throw new InvalidDataException("Could not parse wram B or C mapping.");
        return new(
            ParseHexString(wramMapping.Start),
            ParseHexString(wramMapping.Length),
            wramMapping.Slots switch
            {
                SLOT_0 => WramBCMappedSlots.Slot0,
                SLOT_01 => WramBCMappedSlots.Slot01,
                SLOT_0123 => WramBCMappedSlots.Slot0123,
                SLOT_01234567 or SLOT_ALL => WramBCMappedSlots.All,
                _ => throw new InvalidDataException("Invalid value specified for wram B or C 'slots'.")
            });
    }

    private WramABlockMapping ParseWramABlockMapping(JsonElement blockMappingJson, out bool locked)
    {
        var blockMapping = blockMappingJson.Deserialize<JsonBlockMapping>()
            ?? throw new InvalidDataException("Could not parse wram A block mapping.");
        if (blockMapping.Slot >= WRAM_A_BLOCK_COUNT)
        {
            throw new InvalidDataException("Invalid slot specified for wram A block mapping.");
        }
        locked = blockMapping.Locked;
        return new(
            blockMapping.Master switch
            {
                MASTER_ARM7 => WramAMaster.Arm7,
                MASTER_ARM9 => WramAMaster.Arm9,
                _ => throw new InvalidDataException("Invalid master specified for wram A block mapping.")
            },
            (WramASlot)blockMapping.Slot,
            blockMapping.Enabled);
    }

    private WramBBlockMapping ParseWramBBlockMapping(JsonElement blockMappingJson, out bool locked)
    {
        var blockMapping = blockMappingJson.Deserialize<JsonBlockMapping>()
            ?? throw new InvalidDataException("Could not parse wram B block mapping.");
        if (blockMapping.Slot >= WRAM_BC_BLOCK_COUNT)
        {
            throw new InvalidDataException("Invalid slot specified for wram B block mapping.");
        }
        locked = blockMapping.Locked;
        return new(
            blockMapping.Master switch
            {
                MASTER_ARM9 => WramBMaster.Arm9,
                MASTER_ARM7 => WramBMaster.Arm7,
                MASTER_DSP => WramBMaster.DspCode,
                _ => throw new InvalidDataException("Invalid master specified for wram B block mapping.")
            },
            (WramBCSlot)blockMapping.Slot,
            blockMapping.Enabled);
    }

    private WramCBlockMapping ParseWramCBlockMapping(JsonElement blockMappingJson, out bool locked)
    {
        var blockMapping = blockMappingJson.Deserialize<JsonBlockMapping>()
            ?? throw new InvalidDataException("Could not parse wram C block mapping.");
        if (blockMapping.Slot >= WRAM_BC_BLOCK_COUNT)
        {
            throw new InvalidDataException("Invalid slot specified for wram C block mapping.");
        }
        locked = blockMapping.Locked;
        return new(
            blockMapping.Master switch
            {
                MASTER_ARM9 => WramCMaster.Arm9,
                MASTER_ARM7 => WramCMaster.Arm7,
                MASTER_DSP => WramCMaster.DspData,
                _ => throw new InvalidDataException("Invalid master specified for wram C block mapping.")
            },
            (WramBCSlot)blockMapping.Slot,
            blockMapping.Enabled);
    }

    private NtrWramMaster ParseNtrWramMapping(string? mapping) => mapping switch
    {
        MASTER_ARM9 => NtrWramMaster.Arm9,
        MASTER_ARM7 => NtrWramMaster.Arm7,
        _ => throw new InvalidDataException("Invalid master specified for ntr wram mapping.")
    };

    private uint ParseHexString(string hexString)
    {
        if (new UInt32Converter().ConvertFromString(hexString) is not uint value)
        {
            throw new InvalidDataException("Invalid start address or length specified.");
        }
        return value;
    }

    private sealed class JsonWramMapping
    {
        [JsonPropertyName("start")]
        public string Start { get; set; } = "0x03000000";

        [JsonPropertyName("length")]
        public string Length { get; set; } = "0";

        [JsonPropertyName("slots")]
        public string Slots { get; set; } = SLOT_0;
    }

    private sealed class JsonBlockMapping
    {
        [JsonPropertyName("master")]
        public string Master { get; set; } = MASTER_ARM9;

        [JsonPropertyName("slot")]
        public uint Slot { get; set; } = 0;

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = true;

        [JsonPropertyName("locked")]
        public bool Locked { get; set; } = false;
    }
}
