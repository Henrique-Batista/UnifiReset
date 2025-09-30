using MongoDB.Bson;
using MongoDB.Driver;
using CryptSharp.Core;

string? enderecoBanco = null;
string? portaBanco = null;
MongoClient? client = null;
IMongoDatabase? database = null;
IMongoCollection<BsonDocument>? collection = null;
FilterDefinition<BsonDocument>? filter = null;
BsonDocument? document = null;
BsonValue? x_shadow = null;
BsonValue? id = null;
BsonValue? username = null;
string? senha = null;
string? senhaCriptografada = null;
var indexEndereco = Array.IndexOf(args, "-e") == -1 ? Array.IndexOf(args, "--endereco") : Array.IndexOf(args, "-e");
var indexPorta = Array.IndexOf(args, "-p") == -1 ? Array.IndexOf(args, "--porta") : Array.IndexOf(args, "-p");
var indexSenha = Array.IndexOf(args, "-s") == -1 ? Array.IndexOf(args, "--senha") : Array.IndexOf(args, "-s");

Console.WriteLine(
    "Este programa realiza o acesso ao banco de dados local do Unifi Controller e altera o hash da senha armazenada para o primeiro usuario admin.\n");

if (Array.IndexOf(args, "-h") != -1 || Array.IndexOf(args, "--help") != -1)
{
    Console.WriteLine(
        @"Ajuda:
             Sintaxe: UnifiReset.exe --endereco localhost --porta 27117 --senha Teste
                      UnifiReset.exe -e localhost -p 27117 -s Teste
                      UnifiReset.exe --help
                      UnifiReset.exe -h
             
             Se não forem fornecidos argumentos de linha de comando, o programa solicitará o endereco e/ou porta do banco de dados e/ou senha.

             Retorno: Senha criptografada
                      Nome do usuario alterado, Id do usuario alterado"
    );
    Console.ReadKey();
    return 0;
}

try
{
    if (indexEndereco == -1)
    {
        Console.WriteLine("\nInforme o endereco do banco ( Deixe vazio para utilizar o padrao localhost )");
        enderecoBanco = Console.ReadLine();
    }
    else
    {
        enderecoBanco = args[indexEndereco + 1];
        Console.WriteLine($"Utilizando argumento {enderecoBanco} como endereco do bancos de dados");
    }

    if (indexPorta == -1)
    {
        Console.WriteLine("Informe a porta do banco ( Deixe vazio para utilizar o padrao 27117 )");
        portaBanco = Console.ReadLine();
    }
    else
    {
        portaBanco = args[indexPorta + 1];
        Console.WriteLine($"Utilizando argumento {portaBanco} como porta do banco de dados");
    }

    if (indexSenha != -1)
    {
        senha = args[indexSenha + 1];
        Console.WriteLine($"Utilizando argumento {senha} como senha");
    }
}
catch
{
    Console.WriteLine(
        "Foram fornecidos argumentos incompativeis para o endereco ou porta do banco de dados. Favor utilizar o esquema \"--endereco {endereco} --porta {porta}\" ou \"-e {endereco} -p {porta}\".");
    Console.ReadKey();
    return 3;
}

Console.Write("Tentando conectar no banco...");

try
{
    ConectaBanco(enderecoBanco, portaBanco);
    if (senha == null)
    {
        GeraInterface();
    }

    if (senha != string.Empty)
    {
        ExecutaMkPasswd();
        ExibeInfo();
    }
}
catch (TimeoutException)
{
    Console.WriteLine(
        "\nHouve um problema para se conectar ao banco, por favor verifique se ele esta online, se o endereco esta correto e se esta funcionando na porta informada.");
    Console.ReadKey();
    return 1;
}
catch (Exception)
{
    Console.WriteLine("\nHouve algum problema na aplicacao, por favor contate o desenvolvedor.");
    Console.ReadKey();
    return 2;
}
finally
{
    Console.WriteLine("\nPrograma finalizado, digite qualquer tecla para encerrar.");
    Console.ReadKey();
}

void GeraInterface()
{
    Console.Write("OK");
    Console.WriteLine("\nDigite a nova senha");
    senha ??= Console.ReadLine();
    Console.WriteLine();
}

void ConectaBanco(string? endereco, string? porta)
{
    if (endereco == string.Empty)
    {
        endereco = "localhost";
        Console.Write("Endereco invalido, utilizando endereco padrao localhost...");
    }

    if (porta == string.Empty || int.TryParse(porta, out var portaParseado) == false)
    {
        porta = Convert.ToString(27117);
        Console.Write("Porta invalida, utilizando porta padrao 27117...");
    }
    else
    {
        porta = Convert.ToString(porta);
    }

    client = new MongoClient($"mongodb://{endereco}:{porta}");
    database = client.GetDatabase("ace");
    collection = database.GetCollection<BsonDocument>("admin");
    filter = Builders<BsonDocument>.Filter.Empty;
    document = collection.Find(filter).FirstOrDefault();
    x_shadow = document.GetValue("x_shadow");
    id = document.GetValue("_id");
    username = document.GetValue("name");
}

void ExecutaMkPasswd()
{
    senhaCriptografada = Crypter.Sha512.Crypt(senha!,"$6$9Ter1EZ9");
}

void ExibeInfo()
{
    Console.WriteLine($"\nSenha criptografada: {senhaCriptografada}\n");
    Console.WriteLine("Nome do usuario:" + username + "\nId do usuario:" + id + "\n");
    // Atualiza o banco de dados com a informacao da senha
    var update = Builders<BsonDocument>.Update.Set("x_shadow", senhaCriptografada);
    collection?.UpdateOne(filter, update);
}

return 0;