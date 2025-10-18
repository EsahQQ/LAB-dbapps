using Microsoft.EntityFrameworkCore;
using OnlineCoursesWeb.Data;
using OnlineCoursesWeb.Infrastucture;
using OnlineCoursesWeb.Models;
using OnlineCoursesWeb.Services;
using System.Collections;
using System.Linq;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        #region Service Registration
        // Настройка подключения к БД с UserSecrets
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

        builder.Services.AddDbContext<Db28021Context>(options => options.UseSqlServer(connectionString));

        builder.Services.AddMemoryCache();
        builder.Services.AddScoped<IDataCacheService, DataCacheService>();

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession();
        #endregion

        var app = builder.Build();

        app.UseSession();

        app.MapGet("/", async (HttpContext context, IDataCacheService cacheService) =>
        {
            cacheService.PrimeCache(); 

            context.Response.ContentType = "text/html; charset=utf-8";
            string htmlString = "<HTML><HEAD><TITLE>Главная</TITLE></HEAD>" +
                                "<BODY><H1>Главная страница</H1>" +
                                "<H2>Данные из всех таблиц были кэшированы на сервере.</H2>" +
                                "<H3>Доступные маршруты:</H3>" +
                                "<UL>" +
                                "<LI><A href='/info'>Информация о запросе</A></LI>" +
                                "<LI><A href='/table/students'>Кэш таблицы студентов</A></LI>" +
                                "<LI><A href='/table/courses'>Кэш таблицы курсов</A></LI>" +
                                "<LI><A href='/table/instructors'>Кэш таблицы инструкторов</A></LI>" +
                                "<LI><A href='/table/enrollments'>Кэш таблицы записей на курсы</A></LI>" +
                                "<LI><A href='/table/modules'>Кэш таблицы модулей</A></LI>" +
                                "<LI><A href='/table/tests'>Кэш таблицы тестов</A></LI>" +
                                "<LI><A href='/table/testresults'>Кэш таблицы результатов тестов</A></LI>" +
                                "<LI><A href='/searchform1'>Форма с состоянием в Cookies</A></LI>" +
                                "<LI><A href='/searchform2'>Форма с состоянием в Session</A></LI>" +
                                "</UL></BODY></HTML>";
            await context.Response.WriteAsync(htmlString);
        });

        app.MapGet("/info", async (HttpContext context) =>
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            string strResponse = "<HTML><HEAD><TITLE>Информация</TITLE></HEAD>" +
                                 "<BODY><H1>Информация о запросе:</H1>";
            strResponse += $"<BR> Сервер: {context.Request.Host}";
            strResponse += $"<BR> Путь: {context.Request.Path}";
            strResponse += $"<BR> Протокол: {context.Request.Protocol}";
            strResponse += "<BR><A href='/'>На главную</A></BODY></HTML>";
            await context.Response.WriteAsync(strResponse);
        });

        var tableDelegate = async (HttpContext context, IDataCacheService cacheService) =>
        {
            var tableName = context.Request.Path.Value.Split('/').Last();
            IEnumerable data = cacheService.GetData(tableName);

            context.Response.ContentType = "text/html; charset=utf-8";
            var htmlBuilder = new StringBuilder();
            htmlBuilder.Append($"<HTML><HEAD><TITLE>Кэш: {tableName}</TITLE></HEAD>");
            htmlBuilder.Append($"<BODY><H1>Кэшированные данные из таблицы: {tableName}</H1>");

            if (data != null && data.Cast<object>().Any())
            {
                htmlBuilder.Append("<TABLE BORDER=1>");
                var firstItem = data.Cast<object>().First();
                var properties = firstItem.GetType().GetProperties().Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string)).ToList();
                htmlBuilder.Append("<TR>");
                foreach (var prop in properties) { htmlBuilder.Append($"<TH>{prop.Name}</TH>"); }
                htmlBuilder.Append("</TR>");
                foreach (var item in data)
                {
                    htmlBuilder.Append("<TR>");
                    foreach (var prop in properties) { htmlBuilder.Append($"<TD>{prop.GetValue(item)}</TD>"); }
                    htmlBuilder.Append("</TR>");
                }
                htmlBuilder.Append("</TABLE>");
            }
            else
            {
                htmlBuilder.Append("<P>Данные для этой таблицы не найдены в кэше.</P>");
            }
            htmlBuilder.Append("<BR><A href='/'>На главную</A></BODY></HTML>");
            await context.Response.WriteAsync(htmlBuilder.ToString());
        };

        app.MapGet("/table/students", tableDelegate);
        app.MapGet("/table/courses", tableDelegate);
        app.MapGet("/table/instructors", tableDelegate);
        app.MapGet("/table/enrollments", tableDelegate);
        app.MapGet("/table/modules", tableDelegate);
        app.MapGet("/table/tests", tableDelegate);
        app.MapGet("/table/testresults", tableDelegate);

        app.MapGet("/searchform1", async (HttpContext context, Db28021Context db) =>
        {
            Console.WriteLine("\n--> /searchform1: Загрузка данных формы из Session.");
            context.Response.ContentType = "text/html; charset=utf-8";

            var model = context.Session.Get<SearchFormModel>("searchForm2") ?? new SearchFormModel();

            if (context.Request.Query.Count > 0)
            {
                var modelToSave = new SearchFormModel 
                {
                    SearchText = context.Request.Query["searchText"],
                    SelectedCourse = context.Request.Query["selectedCourse"],
                    DifficultyLevel = context.Request.Query["difficultyLevel"]
                };

                var cookieOptions = new CookieOptions { Expires = DateTime.Now.AddMinutes(20) };
                context.Response.Cookies.Append("searchText1", modelToSave.SearchText ?? "", cookieOptions);
                context.Response.Cookies.Append("selectedCourse1", modelToSave.SelectedCourse ?? "", cookieOptions);
                context.Response.Cookies.Append("difficultyLevel1", modelToSave.DifficultyLevel ?? "", cookieOptions);

                model = modelToSave;
            }
            var courses = await db.Courses.Select(c => c.Title).ToListAsync(); 
            string strResponse = "<HTML><HEAD><TITLE>Форма 1 (Cookies)</TITLE></HEAD>" +
                                 "<BODY><H1>Форма поиска 1 (состояние в Cookies)</H1><FORM method='GET'>";
            strResponse += $"Текстовое поле: <INPUT type='text' name='searchText' value='{model.SearchText}'><BR><BR>";
            strResponse += "Выпадающий список (Курсы): <SELECT name='selectedCourse'>";
            foreach (var course in courses)
            {
                string selected = model.SelectedCourse == course ? "selected" : "";
                strResponse += $"<OPTION value='{course}' {selected}>{course}</OPTION>";
            }
            strResponse += "</SELECT><BR><BR>";
            strResponse += "Переключатели (Сложность):<BR>";
            strResponse += $"<INPUT type='radio' name='difficultyLevel' value='Начальный' {(model.DifficultyLevel == "Начальный" ? "checked" : "")}> Начальный<BR>";
            strResponse += $"<INPUT type='radio' name='difficultyLevel' value='Средний' {(model.DifficultyLevel == "Средний" ? "checked" : "")}> Средний<BR>";
            strResponse += $"<INPUT type='radio' name='difficultyLevel' value='Продвинутый' {(model.DifficultyLevel == "Продвинутый" ? "checked" : "")}> Продвинутый<BR><BR>";
            strResponse += "<INPUT type='submit' value='Отправить'>";
            strResponse += "</FORM><BR><A href='/'>На главную</A></BODY></HTML>";
            await context.Response.WriteAsync(strResponse);
        });

        app.MapGet("/searchform2", async (HttpContext context, Db28021Context db) =>
        {
            Console.WriteLine("\n--> /searchform2: Загрузка данных формы из Cookies.");
            context.Response.ContentType = "text/html; charset=utf-8";

            var model = new SearchFormModel
            {
                SearchText = context.Request.Cookies["searchText1"],
                SelectedCourse = context.Request.Cookies["selectedCourse1"],
                DifficultyLevel = context.Request.Cookies["difficultyLevel1"]
            };

            if (context.Request.Query.Count > 0)
            {
                var modelToSave = new SearchFormModel
                {
                    SearchText = context.Request.Query["searchText"],
                    SelectedCourse = context.Request.Query["selectedCourse"],
                    DifficultyLevel = context.Request.Query["difficultyLevel"]
                };
                context.Session.Set("searchForm2", modelToSave);

                model = modelToSave;
            }
            var courses = await db.Courses.Select(c => c.Title).ToListAsync();
            string strResponse = "<HTML><HEAD><TITLE>Форма 2 (Session)</TITLE></HEAD>" +
                                 "<BODY><H1>Форма поиска 2 (состояние в Session)</H1><FORM method='GET'>";
            strResponse += $"Текстовое поле: <INPUT type='text' name='searchText' value='{model.SearchText}'><BR><BR>";
            strResponse += "Выпадающий список (Курсы): <SELECT name='selectedCourse'>";
            foreach (var course in courses)
            {
                string selected = model.SelectedCourse == course ? "selected" : "";
                strResponse += $"<OPTION value='{course}' {selected}>{course}</OPTION>";
            }
            strResponse += "</SELECT><BR><BR>";
            strResponse += "Переключатели (Сложность):<BR>";
            strResponse += $"<INPUT type='radio' name='difficultyLevel' value='Начальный' {(model.DifficultyLevel == "Начальный" ? "checked" : "")}> Начальный<BR>";
            strResponse += $"<INPUT type='radio' name='difficultyLevel' value='Средний' {(model.DifficultyLevel == "Средний" ? "checked" : "")}> Средний<BR>";
            strResponse += $"<INPUT type='radio' name='difficultyLevel' value='Продвинутый' {(model.DifficultyLevel == "Продвинутый" ? "checked" : "")}> Продвинутый<BR><BR>";
            strResponse += "<INPUT type='submit' value='Отправить'>";
            strResponse += "</FORM><BR><A href='/'>На главную</A></BODY></HTML>";
            await context.Response.WriteAsync(strResponse);
        });

        app.Use(async (context, next) =>
        {
            await next(); 
            if (context.Response.StatusCode == 404) 
            {
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync("Page Not Found");
            }
        });

        app.Run();
    }
}