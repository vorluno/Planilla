// RISK: Removing Blazor components - converting to Web API + React SPA architecture
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vorluno.Planilla.Infrastructure.Data;
using Vorluno.Planilla.Domain.Entities;
using Vorluno.Planilla.Web.Extensions;
using Vorluno.Planilla.Application.Mappings;

var builder = WebApplication.CreateBuilder(args);

// --- INICIO DE LA CONFIGURACI�N DE SERVICIOS ---


// 1. LEER LA CADENA DE CONEXI�N
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 2. REGISTRAR EL DBCONTEXT PARA INYECCI�N DE DEPENDENCIAS
//    Le decimos a la aplicaci�n c�mo crear instancias de nuestro ApplicationDbContext,
//    configur�ndolo para que use SQL Server con la cadena de conexi�n.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. CONFIGURAR ASP.NET CORE IDENTITY
//    Configura el sistema de usuarios y roles, usando nuestro ApplicationDbContext para almacenar los datos
//    y nuestra clase AppUser como el modelo de usuario.
builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 4. <<<---  AUTOMAPPER ---<<<
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// 5. CONFIGURAR AUTHORIZATION POLICIES (Phase E)
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CanCalculatePayroll", p => p.RequireRole("PayrollOperator", "PayrollAdmin", "FinanceManager"))
    .AddPolicy("CanApprovePayroll", p => p.RequireRole("PayrollAdmin", "FinanceManager"))
    .AddPolicy("CanPayPayroll", p => p.RequireRole("FinanceManager"))
    .AddPolicy("CanCancelPayroll", p => p.RequireRole("PayrollAdmin", "FinanceManager"));

// 6. REGISTRAR SERVICIOS DE LA APLICACI�N (UnitOfWork, etc.)
builder.Services.ConfigureApplicationServices();

// 7. REGISTRAR HTTPCONTEXTACCESSOR (Phase E - para CurrentUserService)
builder.Services.AddHttpContextAccessor();

// --- FIN DE NUESTRA CONFIGURACI�N PRINCIPAL ---


// RISK: Removing Blazor services - Web API + Identity backend only
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configurar para ignorar ciclos de referencia en serialización JSON
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        // Opcional: usar naming policy camelCase para consistencia con frontend
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// RISK: Creating inline no-op email sender since Blazor Components.Account was removed
builder.Services.AddSingleton<IEmailSender<AppUser>>(provider => 
    new NoOpEmailSender());

// TODO: React will handle authentication UI - backend provides Identity API endpoints

var app = builder.Build();

// --- MIGRACIONES Y SEEDING ---
// Aplicar migraciones pendientes y ejecutar seed de configuración
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        logger.LogInformation("Aplicando migraciones pendientes...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Migraciones aplicadas correctamente");

        logger.LogInformation("Ejecutando seed de configuración...");
        await PayrollConfigSeeder.SeedAsync(context, logger);
        logger.LogInformation("Seed de configuración completado");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error durante migraciones o seeding");
        throw;
    }
}

// --- CONFIGURACI�N DEL PIPELINE DE PETICIONES HTTP (MIDDLEWARE) ---
// El orden aqu� es importante.

if (app.Environment.IsDevelopment())
{
    // En desarrollo, muestra p�ginas de error detalladas para la base de datos.
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseMigrationsEndPoint();
}
else
{
    // En producci�n, usa un manejador de errores gen�rico y fuerza el uso de HTTPS.
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Serves static files including React SPA from wwwroot
app.UseRouting();

// RISK: Removing UseAntiforgery - React SPA will handle CSRF via API tokens
app.UseAuthentication(); // Must come before UseAuthorization
app.UseAuthorization();

// API Controllers for React SPA
app.MapControllers();

// SPA fallback - serve React app for client-side routing
app.MapFallbackToFile("/react/index.html");

// Inicia y ejecuta la aplicaci�n.
app.Run();

// RISK: Inline NoOp email sender replacement for removed Blazor Components.Account
public class NoOpEmailSender : IEmailSender<AppUser>
{
    public Task SendConfirmationLinkAsync(AppUser user, string email, string confirmationLink) => Task.CompletedTask;
    public Task SendPasswordResetLinkAsync(AppUser user, string email, string resetLink) => Task.CompletedTask;
    public Task SendPasswordResetCodeAsync(AppUser user, string email, string resetCode) => Task.CompletedTask;
}