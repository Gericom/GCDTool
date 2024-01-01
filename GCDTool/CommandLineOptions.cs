using CommandLine;

namespace GCDTool;

/// <summary>
/// Class containing the command line options of GCDTool.
/// </summary>
sealed class CommandLineOptions
{
    [Option("arm9", Required = true, HelpText = "ARM9 elf file path.")]
    public required string Arm9Path { get; init; }

    [Option("arm7", Required = true, HelpText = "ARM7 elf file path.")]
    public required string Arm7Path { get; init; }

    [Option("wram", Required = true, HelpText = "Initial WRAM configuration json file path.")]
    public required string WramConfigJsonPath { get; init; }

    [Option("rsakey", Required = true, HelpText = "GCD RSA private key der file path.")]
    public required string GcdKeyPath { get; init; }

    [Option("gamecode", Default = "####", Required = false, HelpText = "The gamecode to use.")]
    public required string GameCode { get; init; }

    [Option("arm9-67-mhz", Default = false, Required = false, HelpText = "When true, the ARM9 will start at 67 MHz instead of 134 MHz.")]
    public bool Arm9Speed67MHz { get; init; }

    [Value(0, MetaName = "output path", Required = true, HelpText = "The path of the gcd file to create.")]
    public required string OutputPath { get; init; }
}
