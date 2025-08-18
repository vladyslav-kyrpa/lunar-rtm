
using Microsoft.EntityFrameworkCore;
using ServerApp.DataAccess;
using Microsoft.AspNetCore.Identity;
using ServerApp.DataAccess.Repositories.Interfaces;
using ServerApp.DataAccess.Repositories;
using ServerApp.BusinessLogic.Services.Interfaces;
using ServerApp.BusinessLogic.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAuthentication().AddCookie("Identity.Application");
        builder.Services.AddAuthorization();

        var connectionString = builder.Configuration.GetConnectionString("main-db");
        builder.Services.AddDbContext<MainDbContext>(options =>
            options.UseNpgsql(connectionString));

        // config profiles and auth
        builder.Services.AddIdentityCore<UserProfileEntity>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
        })
            .AddEntityFrameworkStores<MainDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        ConfigCors(builder);
        ConfigCookie(builder);

        // define services
        builder.Services.AddScoped<IAuthService, AuthService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("CorsPolicy");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }

    private static void ConfigCookie(WebApplicationBuilder builder)
    {
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };

            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });
    }

    private static void ConfigCors(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", corsBuilder =>
            {
                corsBuilder.WithOrigins("http://localhost:5173")
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }
}