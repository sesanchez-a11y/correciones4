using System;
using MongoDB.Driver;

// Programa temporal para eliminar el usuario admin antiguo y dejar que el seed lo recree correctamente.
var mongoConn = Environment.GetEnvironmentVariable("MONGODB_CONN") ?? "mongodb://localhost:27017";
var mongoDb = Environment.GetEnvironmentVariable("MONGODB_DB") ?? "EduMentor";

Console.WriteLine($"Conectando a MongoDB: {mongoConn} DB={mongoDb}");
var client = new MongoClient(mongoConn);
var db = client.GetDatabase(mongoDb);
var usuarios = db.GetCollection<MongoDB.Bson.BsonDocument>("Usuarios");

// Listar primeros documentos para inspección
var docs = await usuarios.Find(_ => true).Limit(20).ToListAsync();
Console.WriteLine($"Documentos encontrados: {docs.Count}");
foreach (var d in docs)
{
    Console.WriteLine($"- Documento: {d.ToString()}");
}

// Limpiar toda la colección para empezar de cero
Console.WriteLine("\nLimpiando colección Usuarios...");
var delAll = await usuarios.DeleteManyAsync(_ => true);
Console.WriteLine($"Documentos eliminados: {delAll.DeletedCount}");
Console.WriteLine("Fin. Reinicia el backend para que el seed cree el admin correctamente.");
