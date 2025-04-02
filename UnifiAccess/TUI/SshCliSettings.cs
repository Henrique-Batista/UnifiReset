using System.ComponentModel;
using Spectre.Console.Cli;

namespace UnifiAccess.TUI;

internal sealed partial class SshCli
{
    internal class SshCliSettings : CommandSettings
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

        internal sealed class SetControllerCommandSettings : SshCliSettings
        {
            [CommandArgument(3,"<controller>")]
            [Description("Controladora a qual a unifi deve apontar")]
            public string? Controller { get; set; }
        }

        internal sealed class UpdateFirmwareCommandSettings : SshCliSettings
        {
            [CommandArgument(3, "<firmware-file>")]
            [Description("Endereço Web do Arquivo de firmware da Unifi")]
            public string? FirmwareFile { get; set; }
        }
    }
}