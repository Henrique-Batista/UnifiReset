using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;

string? enderecoBanco = null;
string? portaBanco = null;
MongoClient client = null;
IMongoDatabase database = null;
IMongoCollection<BsonDocument> collection = null;
FilterDefinition<BsonDocument> filter = null;
BsonDocument? document = null;
BsonValue? x_shadow = null;
BsonValue? id = null;
BsonValue? username = null;
string senha = null;
string senhaCriptografada = null;


Console.WriteLine("Este programa realiza o acesso ao banco de dados local do Unifi Controller e altera o hash da senha armazenada para o primeiro usuario admin.\n");

Console.WriteLine("\nInforme o endereco do banco ( Deixe vazio para utilizar o padrao localhost )");
enderecoBanco = Console.ReadLine();
Console.WriteLine("Informe a porta do banco ( Deixe vazio para utilizar o padrao 27117 )");
portaBanco = Console.ReadLine();


Console.Write("Tentando conectar no banco...");
try
{

    ConectaBanco(enderecoBanco, portaBanco);
    GeraInterface();
    if (senha != String.Empty)
    {
        ExecutaMkPasswd();
        ExibeInfo();
    }
}
catch (TimeoutException)
{
    Console.WriteLine(
        "\nHouve um problema para se conectar ao banco, por favor verifique se ele esta online, se o endereco esta correto e se esta funcionando na porta informada.");
}
catch (Exception)
{
    Console.WriteLine("\nHouve algum problema na aplicacao, por favor contate o desenvolvedor.");
}

void GeraInterface(){
    Console.Write("OK");
    Console.WriteLine("\nDigite a nova senha");
    senha = Console.ReadLine();
    Console.WriteLine();
}
void ConectaBanco(string? endereco, string? porta)
{
    if (endereco == String.Empty)
    {
        endereco = "localhost";
        Console.Write("Endereco invalido, utilizando endereco padrao localhost...");
    }
    if ((porta == String.Empty) || (Int32.TryParse(porta, out int portaParseado) == false))
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
    var process = new Process();
    var startinfo = new ProcessStartInfo();
    byte[] binaryData = UnifiReset.Properties.Resources.mkpasswd;
    string tempFilePath = Path.Combine(Path.GetTempPath(), "mkpasswd.exe");
    File.WriteAllBytes(tempFilePath, binaryData);
    startinfo.FileName = tempFilePath;
    startinfo.UseShellExecute = false;
    startinfo.Arguments = $"-hash sha512 -salt \"9Ter1EZ9$lSt6\" -password \"{senha}\"";
    startinfo.RedirectStandardOutput = true;
    process.StartInfo = startinfo;
    process.Start();
    senhaCriptografada = process.StandardOutput.ReadToEnd().Replace("\n", "").Replace("\r", "");
}

void ExibeInfo()
{
    Console.WriteLine($"\nSenha criptografada: {senhaCriptografada}\n");
    Console.WriteLine("Nome do usuario:" + username + "\nId do usuario:" + id + "\n");
    // Atualiza o banco de dados com a informacao da senha
    var update = Builders<BsonDocument>.Update.Set("x_shadow", senhaCriptografada);
    collection.UpdateOne(filter, update);
}

Console.WriteLine("\nPrograma finalizado, digite qualquer tecla para encerrar.");
Console.ReadKey();