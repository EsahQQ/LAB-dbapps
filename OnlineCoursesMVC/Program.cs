    using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCoursesMVC.Data;
using OnlineCoursesMVC.Middleware;

var builder = WebApplication.CreateBuilder(args);

IConfigurationRoot configuration = builder.Configuration.AddUserSecrets<Program>().Build();

var appConnectionString = configuration.GetConnectionString("AppConnection");
var appSqlBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(appConnectionString)
{
    Password = configuration["Database:AppPassword"],
    UserID = configuration["Database:AppUser"]
};
builder.Services.AddDbContext<Db31048Context>(options => 
    options.UseSqlServer(appSqlBuilder.ConnectionString));

var identityConnectionString = configuration.GetConnectionString("IdentityConnection");
var identitySqlBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(identityConnectionString)
{
    Password = configuration["Database:IdentityPassword"],
    UserID = configuration["Database:IdentityUser"]
};
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(identitySqlBuilder.ConnectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

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


builder.Services.AddDistributedMemoryCache(); 

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseDbInitializer();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
