using System.Net;
using Spectre.Console;
using Spectre.Console.Cli;

namespace UnifiAccess.TUI.Commands;

internal sealed partial class SshCli : Command<CommandSettings.SshCli.SshCliSettings>
{
    public override int Execute(CommandContext context,
        CommandSettings.SshCli.SshCliSettings settings)
    {
        InitializeContext(ref settings);
        
        AnsiConsole.MarkupLine("[bold green]Selecione a ação para ser tomada na Unifi:[/]");
        var choices = new SelectionPrompt<string>();
        choices.AddChoice("Resetar Unifi");
        choices.AddChoice("Apontar uma nova controladora");
        choices.AddChoice("Mostrar informações da Unifi");
        choices.AddChoice("Atualizar Firmware da Unifi");
        var choice = AnsiConsole.Prompt(choices);

        switch (choice)
        {
            case "Mostrar informações da Unifi":
                GetInfo(settings);
                break;
            case "Resetar Unifi":
                ResetUnifi(settings);
                break;
            case "Apontar uma nova controladora":
                SetController(settings);
                break;
            case "Atualizar Firmware da Unifi":
                UpdateFirmware(settings);
                break;
        }

        return 0;
}

    private static bool ValidateInput<T>(ref T settings) where T : CommandSettings.SshCli.SshCliSettings
    {
        if (settings.Host is null | !IPEndPoint.TryParse(settings.Host, out _))
        {
            settings.Host = AnsiConsole
                .Prompt(new TextPrompt<string>(
                    "IP da unifi incorreto ou não informado. Favor informar novamente o endereço IP da Unifi:"));
            return false;
        }

        if (settings.Username is null)
        {
            var choice = AnsiConsole.Prompt(new ConfirmationPrompt("Deseja utilizar o usuário padrão 'ubnt'?"));
            if (choice)
            {
                settings.Username = "ubnt";
                AnsiConsole.WriteLine("Usando o usuário padrão 'ubnt'");
            }
            else
            {
                settings.Username = AnsiConsole.Prompt(new TextPrompt<string>("Digite o nome de usuário da Unifi:"));
            }

            return false;
        }

        if (char.IsUpper(settings.Username[0]))
        {
            settings.Username = AnsiConsole
                .Prompt(new TextPrompt<string>(
                    "Nome de usuário incorreto ou não informado. Favor informar novamente o nome de usuário de acesso a Unifi."));
            return false;
        }

        if (settings.Password is null)
        {
            var choice = AnsiConsole.Prompt(new ConfirmationPrompt("Deseja utilizar a senha padrão 'ubnt'?"));
            if (choice)
            {
                settings.Password = "ubnt";
                AnsiConsole.WriteLine("Usando a senha padrão 'ubnt'");
            }
            else
            {
                settings.Password = AnsiConsole.Prompt(new TextPrompt<string>("Digite a senha:").Secret());
            }

            return false;
        }
        return true;
    }

    private static void InitializeContext<T>(ref T settings) where T : CommandSettings.SshCli.SshCliSettings
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText("UnifiAccess").Color(Color.Green1));
        AnsiConsole.WriteLine();
        //var cliSettings = settings as SshCliSettings;
        while (!ValidateInput(ref settings));
    }
}