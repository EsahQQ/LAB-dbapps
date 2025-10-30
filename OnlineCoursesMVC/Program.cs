using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCoursesMVC.Data;
using OnlineCoursesMVC.Middleware;

var builder = WebApplication.CreateBuilder(args);

string connectionString = "";
IConfigurationRoot configuration = builder.Configuration.AddUserSecrets<Program>().Build();
connectionString = configuration.GetConnectionString("RemoteSQLConnection");
string secretPass = configuration["Database:password"];
string secretUser = configuration["Database:login"];
var sqlConnectionStringBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString)
{
    Password = secretPass,
    UserID = secretUser
};
connectionString = sqlConnectionStringBuilder.ConnectionString;

builder.Services.AddDbContext<Db31048Context>(options =>
    options.UseSqlServer(connectionString));

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

app.UseDbInitializer();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();