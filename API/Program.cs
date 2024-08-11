using System.Text.Json.Serialization;
using API.Middlewares;
using Domain.SeedWork.Notification;
using Infra.IoC;
using Infra.Utils.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("App:Settings"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("0.0.1",
        new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Mail API",
            Version = "0.0.1",
            Description = "API responsavel pelo envio de emails no dominio Air Finder",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact { Name = "Air Finder" }
        });
});

#region Local Injections
builder.Services.AddLocalServices(builder.Configuration);
builder.Services.AddLocalUnitOfWork(builder.Configuration);
#endregion

builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

builder.Services.AddHttpContextAccessor();
builder.Services.AddOptions();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var key = Convert.FromBase64String(builder.Configuration.GetSection("App:Settings:Jwt:Secret").Value!);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
ServiceLocator.Initialize(app.Services.GetRequiredService<IContainer>());
app.MapControllers();
app.UseRouting();
app.UseCors("AllowAllOrigins");
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/0.0.1/swagger.json", "Mail API");
});
app.UseMiddleware<ControllerMiddleware>();

try
{
    Log.Information("Starting the application...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}

