using System.Text;
using _03101_Proyecto2.Api.Data;
using _03101_Proyecto2.Api.Entities;
using _03101_Proyecto2.Api.Security;
using _03101_Proyecto2.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Configuración JWT (validada al arrancar).
builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("Falta la sección 'Jwt' en la configuración.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = ClaimNames.Nombre,
            RoleClaimType = ClaimNames.Rol
        };
    });

// Denegar por defecto (OWASP A01): todo endpoint exige un token válido salvo que se marque
// [AllowAnonymous]; los de administración agregan [Authorize(Roles = ...)].
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

// Acceso a datos: EF Core sobre el esquema creado por los scripts de la carpeta database.
builder.Services.AddDbContext<GestionProyectosContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("GestionProyectos")
            ?? throw new InvalidOperationException("Falta la cadena de conexión 'GestionProyectos'.")));

// Autenticación: hash de contraseñas (PBKDF2), emisión de tokens y servicio de login.
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddScoped<AuthService>();

// Servicios de negocio.
builder.Services.AddScoped<ProyectosService>();
builder.Services.AddScoped<TareasService>();
builder.Services.AddScoped<CatalogosService>();

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
