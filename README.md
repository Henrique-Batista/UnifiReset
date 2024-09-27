# UnifiReset

- Este programa é escrito em C# e utiliza o cliente do MongoDB para se comunicar com uma instância ativa do banco de dados de uma controladora Unifi. Após realizada a conexão, ele substitui o hash da senha atual por um hash da senha fornecida pelo usuário.
- A criação do hash só é possível por conta de um binário GO embutido no programa, que funciona como uma implementação do mkpasswd (utilitario do linux que vem junto do pacote whois para gerar senhas criptografadas). Precisei utilizar esta abordagem pois não encontrei uma implementação do mkpasswd para .Net, e não consegui criar uma por falta de conhecimento e de recursos que são exclusivos do linux. Segue codigo fonte do binario utilizado e do mkpasswd original:
- https://github.com/rfc1036/whois
- https://github.com/myENA/mkpasswd