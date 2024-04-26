using ApiSegundoExamenMvc.Helpers;
using ApiSegundoExamenMvc.Data;
using ApiSegundoExamenMvc.Helpers;
using ApiSegundoExamenMvc.Repositories;
using Microsoft.EntityFrameworkCore;
using NSwag.Generation.Processors.Security;
//using Microsoft.Extensions.Azure;
//using Azure.Security.KeyVault.Secrets;
using ApiSegundoExamenMvc.Helpers;
using ApiSegundoExamenMvc.Repositories;
using ApiSegundoExamenMvc.Data;
using Microsoft.Extensions.Azure;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddOpenApiDocument(document =>
{
    document.Title = "Api Proyecto Cubos";
    document.Description = "api con seguridad Cubos";
    // CONFIGURAMOS LA SEGURIDAD JWT PARA SWAGGER,
    // PERMITE AÑADIR EL TOKEN JWT A LA CABECERA.
    document.AddSecurity("JWT", Enumerable.Empty<string>(),
        new NSwag.OpenApiSecurityScheme
        {
            Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = NSwag.OpenApiSecurityApiKeyLocation.Header,
            Description = "Copia y pega el Token en el campo 'Value:' así: Bearer {Token JWT}."
        }
    );
    document.OperationProcessors.Add(
    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

// Add services to the container. 

builder.Services.AddAzureClients(factory =>

{

    factory.AddSecretClient

    (builder.Configuration.GetSection("KeyVault"));

});



////DEBEMOS PODER RECUPERAR UN OBJETO INYECTADO EN CLASES  

////QUE NO TIENEN CONSTRUCTOR 

SecretClient secretClient =

builder.Services.BuildServiceProvider().GetService<SecretClient>();

KeyVaultSecret secret =

   await secretClient.GetSecretAsync("secretoconection");

string connectionString = secret.Value;


//builder.Services.AddSwaggerGen();
builder.Services.AddTransient<RepositoryCubos>();
//string connectionString = builder.Configuration.GetConnectionString("SqlAzure");
builder.Services.AddDbContext<CubosContext>(options => options.UseSqlServer(connectionString));

HelperActionServicesOAuth helper = new HelperActionServicesOAuth(builder.Configuration);
builder.Services.AddSingleton<HelperActionServicesOAuth>(helper);
builder.Services.AddAuthentication(helper.GetAuthenticateSchema()).AddJwtBearer(helper.GetJwtBearerOptions());



var app = builder.Build();
app.UseOpenApi();
//app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "api cubos v1");
    options.RoutePrefix = "";
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
