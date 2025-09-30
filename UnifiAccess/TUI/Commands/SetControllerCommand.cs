using System.Net;
using Renci.SshNet;
using Spectre.Console;
using Spectre.Console.Cli;

namespace UnifiAccess.TUI.Commands;

internal sealed partial class SshCli
{
    internal sealed class SetControllerCommand : Command<CommandSettings.SshCli.SshCliSettings.SetControllerCommandSettings>
    {
        public override int Execute(CommandContext context,
            CommandSettings.SshCli.SshCliSettings.SetControllerCommandSettings settings)
        {
            InitializeContext(ref settings);
            return SetController(settings);
        }
    }

    private static int SetController(CommandSettings.SshCli.SshCliSettings.SetControllerCommandSettings settings)
    {
        IPAddress controllerIpAddress;
        if (!IPAddress.TryParse(settings.Controller, out controllerIpAddress))
        {
            while (!IPAddress.TryParse(AnsiConsole.Prompt(new TextPrompt<string>(
                           "IP da controladora não válido. Favor informar o endereço IP da controladora:")),
                       out controllerIpAddress)) ;
        }

        using var client = new SshClient(settings.Host,
            settings.Username,
            settings.Password);
        client.Connect();
        if (client.IsConnected)
        {
            AnsiConsole.WriteLine(client.RunCommand($"set-controller http://{controllerIpAddress}:8080/inform")
                    .Result,
                new Style(foreground: Color.Green));
        }

        return 0;
    }

    private static void SetController(CommandSettings.SshCli.SshCliSettings settings)
    {
        var newSettings = new CommandSettings.SshCli.SshCliSettings.SetControllerCommandSettings();
        newSettings.Username = settings.Username;
        newSettings.Host = settings.Host;
        newSettings.Password = settings.Password;
        newSettings.Controller = AnsiConsole.Prompt(new TextPrompt<string>(
            "IP da controladora incorreto ou não definido. Favor informar o endereço IP da controladora:"));
        SetController(newSettings);
    }
}