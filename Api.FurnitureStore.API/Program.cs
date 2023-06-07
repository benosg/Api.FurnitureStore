using Api.FurnitureStore.API.Configuration;
using Api.FurnitureStore.API.Services;
using Api.FurnitureStore.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Text;
using NLog;
using NLog.Web;
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
var builder = WebApplication.CreateBuilder(args);
logger.Debug("init main");
try
{
    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(
        c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Furniture_Store_API",
                Version = "v1"
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = $@"JWT Authorizacion header using the bearer scheme.{Environment.NewLine}{Environment.NewLine} Enter 'Bearer' [space]," +
                            $@" and then your token in text input bellow.  {Environment.NewLine}{Environment.NewLine}" +
                            $@"Example: 'Bearer 123412323fdgsdfgsdfgsfd'"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme {
                Reference=new OpenApiReference {
                        Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                },
                new string[] { }
            }
            });
        });

    builder.Services.AddDbContext<ApiFurnitureStoreContext>(options =>
                    options.UseSqlite(builder.Configuration.GetConnectionString("ApiFurnitureStoreContext")));

    builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("jwtConfig"));
    //EMAIL
    builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
    builder.Services.AddSingleton<IEmailSender, EmailService>();
    var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value);
    var tokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,//produccion siempre true
        ValidateAudience = false,//produccion siempre true
        RequireAudience = false,//produccion siempre true
        ValidateLifetime = true,
    };

    builder.Services.AddSingleton(tokenValidationParameters);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }
    )
    .AddJwtBearer(jwt =>
    {
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = tokenValidationParameters;
    });
    //solo para test, esto siempre es true
    builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            options.SignIn.RequireConfirmedAccount = false)
        .AddEntityFrameworkStores<ApiFurnitureStoreContext>();

    /*NLog*/
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex )
{
    logger.Error(ex, "there has been an error");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}

