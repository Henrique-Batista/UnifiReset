# UnifiReset

             Sintaxe: UnfiReset.exe --endereco localhost --porta 27117 --senha Teste
                      UnfiReset.exe -e localhost -p 27117 -s Teste
             
             Se não forem fornecidos argumentos de linha de comando, o programa solicitará o endereco e/ou porta do banco de dados e/ou senha.

             Retorno: Senha criptografada
                      Nome do usuario alterado, Id do usuario alterado

- Este programa é escrito em C# e utiliza o cliente do MongoDB para se comunicar com uma instância ativa do banco de dados de uma controladora Unifi. Após realizada a conexão, ele substitui o hash da senha atual por um hash da senha fornecida pelo usuário.
- A criação do hash só é possível por conta de um binário GO embutido no programa, que funciona como uma implementação do mkpasswd (utilitario do linux que vem junto do pacote whois para gerar senhas criptografadas). Precisei utilizar esta abordagem pois não encontrei uma implementação do mkpasswd para .Net, e não consegui criar uma por falta de conhecimento e de recursos que são exclusivos do linux. Segue codigo fonte do mkpasswd original e do binario utilizado :
- https://github.com/rfc1036/whois
- https://github.com/myENA/mkpasswd

- Para realizar o build do aplicativo, e necessario baixar e instalar a ultima versao do .Net 8 SDK, clonar ou baixar este repositorio git, abrir a pasta no terminal e digitar "dotnet publish". Isto criara uma pasta dentro de UnifiReset chamada exe, com o binario "UnifiReset.exe" resultado da compilacao.
- Para executar este aplicativo nao e necessario o runtime do .Net 8 instalado, ele ja vem embutido.