using Spectre.Console.Cli;
using UnifiAccess.TUI;
using UnifiAccess.TUI.Commands;

var app = new CommandApp<SshCli>();
app.WithDescription(
    "Realiza o acesso via SSH a uma Unifi para mandar comandos de reset, apontar uma nova controladora ou atualizar o firmware");
app.Configure(config =>
    {
        config.SetApplicationName("UnifiAccess.exe");
        config.SetApplicationVersion("1.0.0");
        
        config.AddCommand<SshCli.SetControllerCommand>("set-controller")
            .WithDescription("Aponta a Unifi para uma nova controladora, se não estiver achando na rede um dispositivo com o nome unifi.")
            .WithExample("set-controller <ip da controladora> <ip da unifi> <nome de usuário> [senha]");
        
        config.AddCommand<SshCli.ResetCommand>("reset")
            .WithDescription("Realiza o reset para as configurações de fábrica da Unifi.")
            .WithExample("reset <ip da unifi> <nome de usuário> [senha]");
        
        config.AddCommand<SshCli.UpdateFirmwareCommand>("update-firmware")
            .WithDescription("Realiza o upgrade do firmware da unifi.")
            .WithExample("update-firmware <arquivo do firmware> <ip da unifi> <nome de usuário> [senha]");
        
        config.AddCommand<SshCli.GetInfoCommand>("info")
            .WithDescription("Imprime as informações da Unifi")
            .WithExample("info <ip da unifi> <nome de usuário> [senha]");
    });

app.Run(args);