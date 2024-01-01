# GCDTool
Tool to create a GCD rom for DSi ntrboot.

## Usage
`GCDTool --arm9 arm9.elf --arm7 arm7.elf --wram wramConfig.json --rsakey gcdKey.der output.gcd`

### Arguments
#### Mandatory
- `--arm9` specifies the arm9 elf file;
- `--arm7` specifies the arm7 elf file;
- `--wram` specifies the wram configuration json file (see below);
- `--rsakey` specifies the der file with the RSA private key to use.

The arm7 and arm9 binaries should have a load address in internal ram (for example TWL wram), as main memory is not yet enabled when the GCD rom is booted.

#### Optional
- `--gamecode` specifies the 4 character game code to use (default: ####);
- `--arm9-67-mhz` runs the arm9 at 67 MHz instead of 134 MHz.

### Wram configuration JSON file
Specifies the initial wram configuration to use.

#### Example
```json
{
    "wramA": {
        "arm7": { "start": "0x03800000", "length": "0x40000", "slots": "all" },
        "blocks": [
            { "master": "arm7", "slot": 0 },
            { "master": "arm7", "slot": 1 },
            { "master": "arm7", "slot": 2 },
            { "master": "arm7", "slot": 3 }
        ]
    },
    "wramB": {
        "arm9": { "start": "0x03800000", "length": "0x40000", "slots": "all" },
        "blocks": [
            { "master": "arm9", "slot": 0 },
            { "master": "arm9", "slot": 1 },
            { "master": "arm9", "slot": 2 },
            { "master": "arm9", "slot": 3 },
            { "master": "arm9", "slot": 4 },
            { "master": "arm9", "slot": 5 },
            { "master": "arm9", "slot": 6 },
            { "master": "arm9", "slot": 7 }
        ]
    },
    "wramC": {
        "arm9": { "start": "0x03840000", "length": "0x40000", "slots": "all" },
        "blocks": [
            { "master": "arm9", "slot": 0 },
            { "master": "arm9", "slot": 1 },
            { "master": "arm9", "slot": 2 },
            { "master": "arm9", "slot": 3 },
            { "master": "arm9", "slot": 4 },
            { "master": "arm9", "slot": 5 },
            { "master": "arm9", "slot": 6 },
            { "master": "arm9", "slot": 7 }
        ]
    },
    "ntrWram" : [ "arm7", "arm7" ],
    "vramC": 7,
    "vramD": 7
}
```

#### TWL wram mapping
For TWL wram A, B and C the mapping in memory can be specified for both the arm9 and arm7 side with the `"arm9"` and `"arm7"` properties (can be omitted to not map the ram at all). Start and length must be 64 kB aligned for wram A, and 32 kB aligned for wram B and C. `"slots"` specifies which slots are actually mapped in memory, and as such also the size of the mirroring. For wram A the options are `"0"` (64 kB), `"01"` (128 kB) and `"all"` or `"0123"` (256 kB). For wram B and C the options are `"0"` (32 kB), `"01"` (64 kB), `"0123"` (128 kB) and `"all"` or `"01234567"` (256 kB).

#### TWL wram block mapping
For each block of TWL wram the following properties are available:
- `"master"` specifies what the block is mapped to. All wrams support `"arm9"` and `"arm7"`, and wram B and C additionally support `"dsp"` for dsp code and data respectively;
- `"slot"` specifies the slot the block is mapped to;
- `"enabled"` specifies whether the block is enabled. This defaults to `true` and can be omitted;
- `"locked"` specifies whether the mapping of the block should be locked, such that the arm9 cannot change it. This defaults to `false` and can be omitted.

When `"enabled"` is set to `false`, `"master"` and `"slot"` can be omitted.

#### NTR wram mapping
Specifies for the two NTR wram blocks whether they are mapped to `"arm9"` or `"arm7"`.

#### Vram mapping
Not sure how this works. There are only 3 bits available, but the vram C and D mapping registers need more bits properly specify the mapping and enable it. Default value is 7 for both.

## Booting the GCD rom
The GCD rom needs to be flashed to a (dev) flashcard or other means of emulating a DS cartridge. Make sure the  blowfish table is used that is included in the generated .gcd file (p table at `0x1600` and s boxes at `0x1C00`).

With the cartridge inserted, the GCD rom is booted by putting a small magnet between the ABXY buttons (to trigger the lid close detection), holding START, SELECT and X and pressing the power button.
