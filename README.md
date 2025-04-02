# UnifiReset / UnifiAccess

# UnifiReset
             Sintaxe: UnifiReset.exe --endereco localhost --porta 27117 --senha Teste
                      UnifiReset.exe -e localhost -p 27117 -s Teste
             
             Se não forem fornecidos argumentos de linha de comando, o programa solicitará o endereco e/ou porta do banco de dados e/ou senha.

             Retorno: Senha criptografada
                      Nome do usuario alterado, Id do usuario alterado
- Este programa é escrito em C# e utiliza o cliente do MongoDB para se comunicar com uma instância ativa do banco de dados de uma controladora Unifi. Após realizada a conexão, ele substitui o hash da senha atual por um hash da senha fornecida pelo usuário.
- A criação do hash só é possível por conta de um binário GO embutido no programa, que funciona como uma implementação do mkpasswd (utilitario do linux que vem junto do pacote whois para gerar senhas criptografadas). Precisei utilizar esta abordagem pois não encontrei uma implementação do mkpasswd para .Net, e não consegui criar uma por falta de conhecimento e de recursos que são exclusivos do linux. Segue codigo fonte do mkpasswd original e do binario utilizado :
- https://github.com/rfc1036/whois
- https://github.com/myENA/mkpasswd

# UnifiAccess
            Sintaxe: 
                DESCRIÇÃO:
                Realiza o acesso via SSH a uma Unifi para mandar comandos de reset, apontar uma nova controladora ou atualizar o
                firmware
                
                USO:
                UnifiAccess.exe [host] [username] [password] [OPÇÕES] [COMANDO]
                
                ARGUMENTOS:
                [host]        IP e porta ( Ex: 192.168.0.1:22 ) do dispositivo a se conectar
                [username]    Usuário de acesso ao servidor SSH
                [password]    Senha de acesso ao servidor SSH
                
                OPÇÕES:
                -h, --help       Exibe informações de ajuda
                -v, --version    Exibe informações de versão
                
                COMANDOS:
                set-controller <controller>        Aponta a Unifi para uma nova controladora, se não estiver achando na rede um
                dispositivo com o nome unifi
                reset                              Realiza o reset para as configurações de fábrica da Unifi. (true para resetar ou
                false para não resetar)
                update-firmware <firmware-file>    Realiza o upgrade do firmware da unifi
                info                               Imprime as informações da Unifi
- Utiliza um cliente SSH embutido para acessar uma Unifi e realizar alguns comandos.


- Para realizar o build do aplicativo, e necessario baixar e instalar a ultima versao do .Net 9 SDK, clonar ou baixar este repositorio git, abrir a pasta do UnifiAccess ou UnifiReset no terminal e digitar "dotnet publish". Isto criara uma pasta chamada exe\, com o binario resultado da compilacao.
- Para executar este aplicativo nao e necessario o runtime do .Net 8 instalado, ele ja vem embutido.
