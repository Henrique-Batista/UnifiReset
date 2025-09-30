using Renci.SshNet;
using Spectre.Console;
using Spectre.Console.Cli;

namespace UnifiAccess.TUI.Commands;

internal sealed partial class SshCli
{
    internal sealed class GetInfoCommand : Command<CommandSettings.SshCli.SshCliSettings>
    {
        public override int Execute(CommandContext context,
            CommandSettings.SshCli.SshCliSettings settings)
        {
            InitializeContext(ref settings);
            return GetInfo(settings);
        }
    }

    private static int GetInfo(CommandSettings.SshCli.SshCliSettings settings)
    {
        using var client = new SshClient(settings.Host,
            settings.Username,
            settings.Password);
        client.Connect();
        if (client.IsConnected)
        {
            AnsiConsole.WriteLine(client.RunCommand("info")
                    .Result,
                new Style(foreground: Color.Green));
        }

        return 0;
    }
}