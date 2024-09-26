using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;

// Inicializa o acesso ao banco de dados
var client = new MongoClient("mongodb://localhost:27117");
var database = client.GetDatabase("ace");
var collection = database.GetCollection<BsonDocument>("admin");
var filter = Builders<BsonDocument>.Filter.Empty;
var document = collection.Find(filter).FirstOrDefault();
var x_shadow = document.GetValue("x_shadow");
var id = document.GetValue("_id");
var username = document.GetValue("name");

// Cria interface do usuario
Console.WriteLine("Este programa realiza o acesso ao banco de dados local do Unifi Controller e altera o hash da senha armazenada para o primeiro usuario admin.\n");
Console.WriteLine("Digite a nova senha");
var senha = Console.ReadLine();
Console.WriteLine();

// Executa o binario do mkpasswd e armazena a senha gerada
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
string senhaCriptografada = process.StandardOutput.ReadToEnd().Replace("\n", "").Replace("\r", "");

Console.WriteLine($"Senha criptografada: {senhaCriptografada}\n");
Console.WriteLine("Nome do usuario:" + username + "\nId do usuario:" + id + "\n" + "Senha anterior:" + x_shadow + "\n");

// Atualiza o banco de dados com a informacao da senha
var update = Builders<BsonDocument>.Update.Set("x_shadow", senhaCriptografada);
collection.UpdateOne(filter,update);

Console.WriteLine("Programa finalizado");
Console.ReadLine();



