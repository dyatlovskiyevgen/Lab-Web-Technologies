//Program.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using OSS.UI.Data;
using OSS.UI.Services.CategoryService;
using OSS.UI.Services.ProductService;
using System.Net.Http.Headers;
using System.Security.Claims;
using Serilog;
using Serilog.Events;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Приложение запускается...");

    var builder = WebApplication.CreateBuilder(args);

// Использование Serilog вместо стандартного логгера, логируются все события
// builder.Host.UseSerilog();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("SqliteConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<AppUser>(options =>
    { 
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("admin", p =>
    p.RequireClaim(ClaimTypes.Role, "admin"));
});

builder.Services.AddTransient<IEmailSender,NoOpEmailSender>();
builder.Services.AddControllersWithViews();

//***
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // можно настроить время хранения
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//***

builder.Services.AddHttpContextAccessor();

// Регистрация HttpClient для API
builder.Services.AddHttpClient<IProductService, ApiProductService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7002/");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient<ICategoryService, ApiCategoryService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7002/");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();


var app = builder.Build();

// Инициализация базы данных с администратором
//await DbInit.SeedData(app);
await DbInit.SetupIdentityAdmin(app);


// Configure the HTTP request pipeline./*8
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();  //***

app.UseAuthentication();//***
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Приложение завершилось аварийно");
}
finally
{
    Log.CloseAndFlush();
}