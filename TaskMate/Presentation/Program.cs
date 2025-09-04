using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data;
using System.Text;
using TaskMate.Application.Services.EmailServices;
using TaskMate.Core.Models;
using TaskMate.Core.Roles;
using TaskMate.Infrastructure.AuthService;
using TaskMate.Infrastructure.Data;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using TaskMate.Application.Mapping;
using TaskMate.Application.Services.CustomMiddlewares;
using TaskMate.Infrastructure.Repository;
using TaskMate.Application.Services.AuthServices;
using TaskMate.Application.Services.EmailServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//Added the serilog
Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

try
{
    Log.Information("Application starts {Time}:", DateTime.UtcNow);

    builder.Host.UseSerilog();

    //Adding the exception Handler
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddControllers();

    builder.Services.AddAutoMapper(typeof(Program));

    //Added the EF DBCONTEXT
    builder.Services.AddDbContext<TaskProDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    //Added the IDENTITY
    builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;

        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 3;


        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<TaskProDbContext>()
    .AddDefaultTokenProviders();


    //Added JWT Bearer or simply Added Authentication
    var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });



    //Added and seeding roles in role manager
    async Task SeedRolesAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in UserRoles.roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }


    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<AuthService, AuthService>();

    //Added Scoped files
    builder.Services.AddScoped<JwtService>();
    builder.Services.AddScoped<EmailService>();


    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    //Adding the IExceptionHandler Middleware in the request pipeline
    app.UseExceptionHandler();


    //added serilog middleware in the request pipeline 
    app.UseMiddleware<UseSerilogMiddleware>();

    //Using Serilog
    app.UseSerilogRequestLogging();

    app.UseAuthorization();

    Log.Information("All Middleware crossed, reached controllers {Time}:", DateTime.UtcNow);
    app.MapControllers();


    await SeedRolesAsync(app);

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application crashed at {Time}", DateTime.UtcNow);
}
finally
{
    Log.CloseAndFlush();
}

