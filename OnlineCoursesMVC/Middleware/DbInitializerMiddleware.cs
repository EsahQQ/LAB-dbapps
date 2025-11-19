using OnlineCoursesMVC.Data;

namespace OnlineCoursesMVC.Middleware
{
    public class DbInitializerMiddleware
    {
        private readonly RequestDelegate _next;
        private static bool _isInitialized = false;
        private static readonly object _lock = new object();

        public DbInitializerMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            lock (_lock)
            {
                if (!_isInitialized)
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var scopedProvider = scope.ServiceProvider;

                        var appDbContext = scopedProvider.GetRequiredService<Db31048Context>();
                        DbInitializer.Initialize(appDbContext);

                        IdentityDataInitializer.InitializeAsync(scopedProvider).GetAwaiter().GetResult();
                    }
                    _isInitialized = true;
                }
            }

            await _next.Invoke(context);
        }
    }

    public static class DbInitializerExtensions
    {
        public static IApplicationBuilder UseDbInitializer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DbInitializerMiddleware>();
        }
    }
}