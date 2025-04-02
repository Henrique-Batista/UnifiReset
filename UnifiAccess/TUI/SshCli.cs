using System.Net;
using Renci.SshNet;
using Spectre.Console;
using Spectre.Console.Cli;

namespace UnifiAccess.TUI;

internal sealed partial class SshCli : Command<SshCli.SshCliSettings>
{
    public override int Execute(CommandContext context,
        SshCliSettings settings)
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

    internal sealed class ResetCommand : Command<SshCliSettings>
    {
        public override int Execute(CommandContext context,
            SshCliSettings settings)
        {
            InitializeContext(ref settings);
            return ResetUnifi(settings);
        }
    }

    private static int ResetUnifi(SshCliSettings settings)
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

    internal sealed class SetControllerCommand : Command<SshCliSettings.SetControllerCommandSettings>
    {
        public override int Execute(CommandContext context,
            SshCliSettings.SetControllerCommandSettings settings)
        {
            InitializeContext(ref settings);
            return SetController(settings);
        }
    }

    private static int SetController(SshCliSettings.SetControllerCommandSettings settings)
    {
        IPAddress controllerIpAddress;
        if (!IPAddress.TryParse(settings.Controller, out controllerIpAddress)){
            while (!IPAddress.TryParse(AnsiConsole.Prompt(new TextPrompt<string>(
                           "IP da controladora não válido. Favor informar o endereço IP da controladora:")),
                       out controllerIpAddress));
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

    private static void SetController(SshCliSettings settings)
    {
        var newSettings = new SshCliSettings.SetControllerCommandSettings();
        newSettings.Username = settings.Username;
        newSettings.Host = settings.Host;
        newSettings.Password = settings.Password;
        newSettings.Controller = AnsiConsole.Prompt(new TextPrompt<string>(
            "IP da controladora incorreto ou não definido. Favor informar o endereço IP da controladora:"));
        SetController(newSettings);
    }

    internal sealed class UpdateFirmwareCommand : Command<SshCliSettings.UpdateFirmwareCommandSettings>
    {
        public override int Execute(CommandContext context,
            SshCliSettings.UpdateFirmwareCommandSettings settings)
        {
            InitializeContext(ref settings);
            return UpdateFirmware(settings);
        }
    }

    private static int UpdateFirmware(SshCliSettings.UpdateFirmwareCommandSettings settings)
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

    private static void UpdateFirmware(SshCliSettings settings)
    {
        var newSettings = new SshCliSettings.UpdateFirmwareCommandSettings();
        newSettings.Username = settings.Username;
        newSettings.Host = settings.Host;
        newSettings.Password = settings.Password;
        newSettings.FirmwareFile = AnsiConsole.Prompt(new TextPrompt<string>(
            "Endereço para download do arquivo do firmware não informado. Favor informar o endereço para download do firmware:"));
        UpdateFirmware(newSettings);
    }

    internal sealed class GetInfoCommand : Command<SshCliSettings>
    {
        public override int Execute(CommandContext context,
            SshCliSettings settings)
        {
            InitializeContext(ref settings);
            return GetInfo(settings);
        }
    }

    private static int GetInfo(SshCliSettings settings)
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

    private static bool ValidateInput<T>(ref T settings) where T : SshCliSettings
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

    private static void InitializeContext<T>(ref T settings) where T : SshCliSettings
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText("UnifiAccess").Color(Color.Green1));
        AnsiConsole.WriteLine();
        //var cliSettings = settings as SshCliSettings;
        while (!ValidateInput(ref settings));
    }
}