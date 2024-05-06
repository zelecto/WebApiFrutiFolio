using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApiFrutiFolio.Context;
using WebApiFrutiFolio.Models;

var builder = WebApplication.CreateBuilder(args);
var CorsRules = "Reglas";


builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddDbContext<FruityFolioContext>(con => con.UseNpgsql(builder.Configuration.GetConnectionString("Fl0ServerConnection")));

builder.Services.AddCors(opt =>
{
    opt.AddPolicy(name: CorsRules, builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddControllers().AddJsonOptions(c =>
{
    c.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});


var app = builder.Build();

app.UseRouting();
app.UseCors(CorsRules);
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();
app.Map("/swagger", swaggerApp =>
{
    swaggerApp.UseStaticFiles(); // Si es necesario para servir archivos estáticos
    swaggerApp.Use(async (context, next) =>
    {
        if (context.Request.Path.StartsWithSegments("/swagger") &&
            !context.Request.Path.Value.EndsWith("index.html"))
        {
            // Sirve la página HTML personalizada
            context.Request.Path = "/swagger/index.html";
        }

        await next();
    });
});
app.UseHttpsRedirection();

app.MapControllers();
app.Run();

