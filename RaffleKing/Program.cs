using Blazored.LocalStorage;
using Hangfire;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using RaffleKing.Components;
using RaffleKing.Components.Account;
using RaffleKing.Data;
using MudBlazor.Services;
using RaffleKing.Infrastructure;
using RaffleKing.Services.BLL.Implementations;
using RaffleKing.Services.BLL.Interfaces;
using RaffleKing.Services.DAL.Implementations;
using RaffleKing.Services.DAL.Interfaces;
using RaffleKing.Services.Utilities.Implementations;
using RaffleKing.Services.Utilities.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

// Add BLL services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDrawManagementService, DrawManagementService>();
builder.Services.AddScoped<IPrizeManagementService, PrizeManagementService>();
builder.Services.AddScoped<IEntryManagementService, EntryManagementService>();
builder.Services.AddScoped<IDrawExecutionService, DrawExecutionService>();

// Add DAL services
builder.Services.AddScoped<IDrawService, DrawService>();
builder.Services.AddScoped<IPrizeService, PrizeService>();
builder.Services.AddScoped<IEntryService, EntryService>();
builder.Services.AddScoped<IWinnerService, WinnerService>();

// Add utility services
builder.Services.AddScoped<ISnackbarHelper, SnackbarHelper>();

// Add email service
var emailConfig = builder.Configuration.GetSection("EmailSettings");
builder.Services.AddSingleton<IEmailService>((serviceProvider) => {
    var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
    var emailTemplatesPath = Path.Combine(env.ContentRootPath, "EmailTemplates");

    return new EmailService(
        emailConfig["SmtpServer"] ?? string.Empty,
        int.Parse(emailConfig["SmtpPort"] ?? string.Empty),
        emailConfig["FromAddress"] ?? string.Empty,
        emailConfig["SmtpUsername"] ?? string.Empty,
        emailConfig["SmtpPassword"] ?? string.Empty,
        emailTemplatesPath
    );
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Add Hangfire configuration
builder.Services.AddHangfire(configuration => configuration.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();

var app = builder.Build();

// TODO: Temporarily use Hangfire dashboard for dev purposes
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new [] { new AllowAllAuthorizationFilter() }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapAccountServices();

// Automatically apply migrations and create the database at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    
    // Not async to ensure application does not start until database is ready
    dbContext.Database.Migrate();
}

// Set up Identity roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "User", "Host", "Admin" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

// Create default account for each role (testing purposes)
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    
    // "User" account
    if (await userManager.FindByEmailAsync("user@default.com") == null)
    {
        var user = new ApplicationUser
        {
            UserName = "DefaultUser",
            Email = "user@default.com"
        };
        
        await userManager.CreateAsync(user, "User12345!");
        await userManager.AddToRoleAsync(user, "User");
    }
    
    // "Host" account
    if (await userManager.FindByEmailAsync("host@default.com") == null)
    {
        var user = new ApplicationUser
        {
            UserName = "DefaultHost",
            Email = "host@default.com"
        };
        
        await userManager.CreateAsync(user, "Host12345!");
        await userManager.AddToRoleAsync(user, "Host");
    }
    
    // "Admin" account
    if (await userManager.FindByEmailAsync("admin@default.com") == null)
    {
        var user = new ApplicationUser
        {
            UserName = "SiteAdmin",
            Email = "admin@default.com"
        };
        
        await userManager.CreateAsync(user, "Admin12345!");
        await userManager.AddToRoleAsync(user, "Admin");
    }
}

app.Run();