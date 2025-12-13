using Microsoft.AspNetCore.Authentication.JwtBearer; // Nuevo
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; // Nuevo
using Microsoft.OpenApi.Models; // Para configurar el candadito en Swagger
using reservasApp.Models;
using reservasApp.Services;
using System.Text; // Nuevo

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. CONFIGURACIÓN DE CORS (ANGULAR)
// ==========================================
// Mantenemos tu configuración exacta para que el frontend no falle
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // URL de tu Frontend
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

// ==========================================
// 2. BASE DE DATOS (MYSQL DOCKER)
// ==========================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ==========================================
// 3. SERVICIOS PROPIOS
// ==========================================
builder.Services.AddScoped<IReservasService, ReservasService>();

// ==========================================
// 4. AUTENTICACIÓN JWT (LO QUE FALTABA)
// ==========================================
// Recuperamos la clave secreta de appsettings.json o usamos una por defecto segura
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? "EstaEsUnaClaveSecretaMuySeguraParaMiApp123!");

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // En producción puedes poner true y configurar el dominio
        ValidateAudience = false // En producción puedes poner true
    };
});

builder.Services.AddEndpointsApiExplorer();

// ==========================================
// 5. SWAGGER CON SOPORTE JWT
// ==========================================
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa 'Bearer' [espacio] y tu token. Ejemplo: 'Bearer eyJhbGci...'"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// ==========================================
// 6. MIDDLEWARE (EL ORDEN IMPORTA)
// ==========================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 1º Permitir Angular
app.UseCors("PermitirAngular");

// 2º Autenticación (¿Quién eres?)
app.UseAuthentication();

// 3º Autorización (¿Qué puedes hacer?)
app.UseAuthorization();

app.MapControllers();

app.Run();