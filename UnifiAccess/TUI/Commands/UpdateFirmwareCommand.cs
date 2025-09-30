using Renci.SshNet;
using Spectre.Console;
using Spectre.Console.Cli;

namespace UnifiAccess.TUI.Commands;

internal sealed partial class SshCli
{
    internal sealed class UpdateFirmwareCommand : Command<CommandSettings.SshCli.SshCliSettings.UpdateFirmwareCommandSettings>
    {
        public override int Execute(CommandContext context,
            CommandSettings.SshCli.SshCliSettings.UpdateFirmwareCommandSettings settings)
        {
            InitializeContext(ref settings);
            return UpdateFirmware(settings);
        }
    }

    private static int UpdateFirmware(CommandSettings.SshCli.SshCliSettings.UpdateFirmwareCommandSettings settings)
    {
        string firmwareFile = settings.FirmwareFile ??
                              AnsiConsole.Prompt(new TextPrompt<string>(
                                  "Endereço para download do arquivo do firmware não informado. Favor informar o endereço para download do firmware:"));
        if (!AnsiConsole.Confirm(
                "Atualizar o firmware para uma versão incorreta pode danificar a Unifi, deseja realmente continuar?"))
        {
            return 1;
        }

        using var client = new SshClient(settings.Host,
            settings.Username,
            settings.Password);
        client.Connect();
        if (client.IsConnected)
        {
            AnsiConsole.WriteLine(client.RunCommand($"update {firmwareFile}")
                    .Result,
                new Style(foreground: Color.Green));
        }

        return 0;
    }

    private static void UpdateFirmware(CommandSettings.SshCli.SshCliSettings settings)
    {
        var newSettings = new CommandSettings.SshCli.SshCliSettings.UpdateFirmwareCommandSettings();
        newSettings.Username = settings.Username;
        newSettings.Host = settings.Host;
        newSettings.Password = settings.Password;
        newSettings.FirmwareFile = AnsiConsole.Prompt(new TextPrompt<string>(
            "Endereço para download do arquivo do firmware não informado. Favor informar o endereço para download do firmware:"));
        UpdateFirmware(newSettings);
    }
}