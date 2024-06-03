using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApiFrutiFolio.Context;
using WebApiFrutiFolio.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Configuration;

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
builder.Services.AddSwaggerGen(options =>
{
    // Configuración de seguridad para JWT en Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingrese el token"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


//========================================= PRIMERO =======================================
builder.Configuration.AddJsonFile("appsettings.json");
var secretKey = builder.Configuration.GetSection("settings").GetSection("secretKey").ToString();

var keyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(config => {

    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config => {
    config.RequireHttpsMetadata = false;
    config.SaveToken = false;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


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

