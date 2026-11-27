using _03101_Proyecto2.Web.Filters;
using _03101_Proyecto2.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// La identidad del usuario en el cliente MVC se mantiene en una cookie cifrada.
// El JWT emitido por la API se guarda dentro de esa cookie y se reenvía en cada
// llamada a la API como encabezado Authorization: Bearer.
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/Login";
        options.LogoutPath = "/Cuenta/Logout";
        options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = false;

        // A una solicitud hecha con fetch no se le responde con una redirección al login o a
        // Acceso denegado (fetch la seguiría y recibiría esa página), sino con 401 o 403.
        options.Events.OnRedirectToLogin = contexto => ResponderRedireccion(contexto, StatusCodes.Status401Unauthorized);
        options.Events.OnRedirectToAccessDenied = contexto => ResponderRedireccion(contexto, StatusCodes.Status403Forbidden);
    });

// Toda página exige sesión iniciada, salvo las marcadas con [AllowAnonymous] (login y error).
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

// Clientes HTTP hacia la API REST. Los controladores MVC nunca acceden a la base de datos.
var apiBaseUrl = new Uri(builder.Configuration["Api:BaseUrl"]
    ?? throw new InvalidOperationException("Falta 'Api:BaseUrl' en la configuración."));
builder.Services.AddHttpClient<AuthApiClient>(client => client.BaseAddress = apiBaseUrl);

// Los demás clientes reenvían el JWT de la sesión en cada solicitud (TokenBearerHandler).
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<TokenBearerHandler>();
builder.Services.AddHttpClient<ProyectosApiClient>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<TokenBearerHandler>();
builder.Services.AddHttpClient<CatalogosApiClient>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<TokenBearerHandler>();
builder.Services.AddHttpClient<TareasApiClient>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<TokenBearerHandler>();

// ErroresApiFilter traduce las respuestas 401 y 403 de la API y sus fallas de conexión.
builder.Services.AddControllersWithViews(options => options.Filters.Add<ErroresApiFilter>());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Los archivos estáticos (CSS, JS) son públicos: la página de login los necesita sin sesión.
app.MapStaticAssets().AllowAnonymous();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

static Task ResponderRedireccion(RedirectContext<CookieAuthenticationOptions> contexto, int codigoAjax)
{
    if (SolicitudAjax.Es(contexto.Request))
    {
        contexto.Response.StatusCode = codigoAjax;
    }
    else
    {
        contexto.Response.Redirect(contexto.RedirectUri);
    }

    return Task.CompletedTask;
}
