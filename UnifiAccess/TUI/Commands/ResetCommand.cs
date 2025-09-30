using Renci.SshNet;
using Spectre.Console;
using Spectre.Console.Cli;

namespace UnifiAccess.TUI.Commands;

internal sealed partial class SshCli
{
    internal sealed class ResetCommand : Command<CommandSettings.SshCli.SshCliSettings>
    {
        public override int Execute(CommandContext context,
            CommandSettings.SshCli.SshCliSettings settings)
        {
            InitializeContext(ref settings);
            return ResetUnifi(settings);
        }
    }

    private static int ResetUnifi(CommandSettings.SshCli.SshCliSettings settings)
    {
        var confirmation = AnsiConsole.Prompt(new ConfirmationPrompt(
            "[red] Este comando irá realizar o reset das configurações desta Unifi, tem certeza que quer executar?[/]"));
        if (!confirmation)
            return 1;

        using var client = new SshClient(settings.Host,
            settings.Username,
            settings.Password);
        client.Connect();
        if (client.IsConnected)
        {
            AnsiConsole.WriteLine(client.RunCommand("set-default")
                    .Result,
                new Style(foreground: Color.Green));
        }

        return 0;
    }
}