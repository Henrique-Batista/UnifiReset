using System.ComponentModel;
using Spectre.Console.Cli;

namespace UnifiAccess.TUI.CommandSettings;

internal sealed partial class SshCli
{
    internal partial class SshCliSettings : Spectre.Console.Cli.CommandSettings
    {

        [CommandArgument(0, "[host]")]
        [Description("IP e porta ( Ex: 192.168.0.1:22 ) do dispositivo a se conectar")]
        public string? Host { get; set; }

        [CommandArgument(1, "[username]")]
        [Description("Usuário de acesso ao servidor SSH")]
        public string? Username { get; set; }

        [CommandArgument(2, "[password]")]
        [Description("Senha de acesso ao servidor SSH")]
        public string? Password { get; set; }
    }
}