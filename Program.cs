using JasperFx.CodeGeneration;
using Marten;
using Marten.Services.Json;
using Microsoft.Extensions.Hosting;
using Weasel.Core;
using Microsoft.Extensions.Configuration;
using MartenBug;
using Weasel.Core.Migrations;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using Marten.Schema.Identity;

const string CONNECTION_STRING = "host=localhost;database=marten_bug;password=Password12!;username=postgres";


// delete internal files to ensure clean generation
try
{
    Console.WriteLine("-- cleaning up internal folder");
    Directory.Delete("./Internal", true);
}
catch (DirectoryNotFoundException)
{
}


var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMarten(_ =>
{
    _.Connection(CONNECTION_STRING);
    _.Policies.AllDocumentsAreMultiTenanted();
    _.UseDefaultSerialization(
        EnumStorage.AsString,
        serializerType: SerializerType.SystemTextJson
    );

    _.Schema
        .For<ShapeBase>()
        .AddSubClass<Triangle>()
        .AddSubClass<Square>()
        .GinIndexJsonData();

    _.Schema.For<User>();

    _.AutoCreateSchemaObjects = AutoCreate.All;
    _.GeneratedCodeMode = TypeLoadMode.Auto;

})
.ApplyAllDatabaseChangesOnStartup();

var host = builder.Build();

var store = host.Services.GetRequiredService<IDocumentStore>();

const string tenantA = "tenant_aaaa";
const string tenantB = "tenant_bbbb";
const string userId = "geoff";
const string shapeColor = "green";

await using var sessionA = store.LightweightSession(tenantA);
await using var sessionB = store.LightweightSession(tenantB);

Console.WriteLine("-- generating User Lookup");
sessionA.Store(new User(CombGuidIdGeneration.NewGuid(), userId));
await sessionA.SaveChangesAsync();

await sessionA.QueryAsync(new UserLookup { Name = userId });
await sessionB.QueryAsync(new UserLookup { Name = userId });

Console.WriteLine("-- Shape Lookup");
sessionA.Store(new Triangle(CombGuidIdGeneration.NewGuid(), shapeColor));
await sessionA.SaveChangesAsync();

await sessionA.QueryAsync(new ShapeLookup { Color = shapeColor });
await sessionB.QueryAsync(new ShapeLookup { Color = shapeColor });