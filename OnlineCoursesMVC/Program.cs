    using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCoursesMVC.Data;
using OnlineCoursesMVC.Middleware;

var builder = WebApplication.CreateBuilder(args);

// --- РЕГИСТРАЦИЯ СЕРВИСОВ ---

// 1. Настройка подключения к основной БД (AppConnection)
// Это ваш работающий подход, но сделанный чуть более явно.
IConfigurationRoot configuration = builder.Configuration.AddUserSecrets<Program>().Build();

// 1. Настройка подключения к основной БД (AppConnection)
var appConnectionString = configuration.GetConnectionString("AppConnection");
var appSqlBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(appConnectionString)
{
    Password = configuration["Database:AppPassword"],
    UserID = configuration["Database:AppUser"]
};
builder.Services.AddDbContext<Db31048Context>(options => 
    options.UseSqlServer(appSqlBuilder.ConnectionString));

// 2. Настройка подключения к БД Identity (IdentityConnection)
var identityConnectionString = configuration.GetConnectionString("IdentityConnection");
var identitySqlBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(identityConnectionString)
{
    Password = configuration["Database:IdentityPassword"],
    UserID = configuration["Database:IdentityUser"]
};
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(identitySqlBuilder.ConnectionString));

// 3. Настройка и регистрация ASP.NET Core Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 4. Добавление сервисов MVC
int variantNumber = 9;
int cacheDuration = 2 * variantNumber + 240;

builder.Services.AddControllersWithViews(options =>
{
    options.CacheProfiles.Add("DefaultCache",
        new CacheProfile()
        {
            Duration = cacheDuration,
            Location = ResponseCacheLocation.Any
        });
}); 

var app = builder.Build();

// --- НАСТРОЙКА КОНВЕЙЕРА MIDDLEWARE ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Middleware сделает всю работу по инициализации обеих баз
app.UseDbInitializer();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
