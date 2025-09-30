using System.ComponentModel;
using Spectre.Console.Cli;

namespace UnifiAccess.TUI.CommandSettings;

internal sealed partial class SshCli
{
    internal partial class SshCliSettings
    {
        internal sealed class UpdateFirmwareCommandSettings : SshCliSettings
        {
            [CommandArgument(3, "<firmware-file>")]
            [Description("Endereço Web do Arquivo de firmware da Unifi")]
            public string? FirmwareFile { get; set; }
        }
    }
}