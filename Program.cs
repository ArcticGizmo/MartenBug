using JasperFx.CodeGeneration;
using Marten;
using Marten.Services.Json;
using Microsoft.Extensions.Hosting;
using Weasel.Core;
using MartenBug;
using Microsoft.Extensions.DependencyInjection;
using JasperFx.Core;

const string CONNECTION_STRING = "host=localhost;database=marten_bug;password=Password12!;username=postgres";

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMarten(_ =>
{
    _.Connection(CONNECTION_STRING);
    _.Policies.AllDocumentsAreMultiTenanted();
    _.UseDefaultSerialization(
        EnumStorage.AsString,
        serializerType: SerializerType.SystemTextJson
    );

    _.AutoCreateSchemaObjects = AutoCreate.All;
    _.GeneratedCodeMode = TypeLoadMode.Auto;

})
.ApplyAllDatabaseChangesOnStartup();

var host = builder.Build();

var store = host.Services.GetRequiredService<IDocumentStore>();

const string tenantA = "tenant_aaaa";
const string tenantB = "tenant_bbbb";
const string userId = "geoff";

await using var sessionA = store.LightweightSession(tenantA);
await using var sessionB = store.LightweightSession(tenantB);

Console.WriteLine("-- generating User Lookup");
sessionA.Store(new User(Marten.Schema.Identity.CombGuidIdGeneration.NewGuid(), userId));
await sessionA.SaveChangesAsync();

await sessionA.QueryAsync(new UserLookup { Name = userId });
await sessionB.QueryAsync(new UserLookup { Name = userId });

// Search for tenant id reference in generated queries
const string compiledQueriesPath = "./Internal/Generated/CompiledQueries";

var filePaths = Directory.EnumerateFiles(compiledQueriesPath);

Console.WriteLine("\n\n\n==== Summary ====");
foreach (string filePath in filePaths)
{
    var file = File.OpenRead(filePath);
    var text = await file.ReadAllTextAsync();
    var hasTenantId = text.Contains("builder.AddNamedParameter(\"tenantid\", session.TenantId);");
    Console.WriteLine($"{filePath} has tenant id from session? ({hasTenantId})");
}

// cleaning up database
await using var session = store.LightweightSession();
await session.Database.CompletelyRemoveAllAsync();