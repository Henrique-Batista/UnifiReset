using System.ComponentModel;
using Spectre.Console.Cli;

namespace UnifiAccess.TUI.CommandSettings;

internal sealed partial class SshCli
{
    internal partial class SshCliSettings
    {
        internal sealed class SetControllerCommandSettings : SshCliSettings
        {
            [CommandArgument(3, "<controller>")]
            [Description("Controladora a qual a unifi deve apontar")]
            public string? Controller { get; set; }
        }
    }
}