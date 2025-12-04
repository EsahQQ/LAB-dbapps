using Microsoft.EntityFrameworkCore;
using OnlineCoursesAPI.Data;

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

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Online Courses API",
        Version = "v1",
        Description = "API дл€ управлени€ данными центра онлайн-обучени€"
    });

    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(System.IO.Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles(); 

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();