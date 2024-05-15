using System.Text.Json.Serialization;
using API.Middlewares;
using Domain.SeedWork.Notification;
using Infra.IoC;
using Infra.Utils.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
);
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("App:Settings"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("0.0.1",
        new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Template API",
            Version = "0.0.1",
            Description = "Template de API responsavel pelo dominio Air Finder",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "Air Finder"
            }
        });
});

#region Local Injections
builder.Services.AddLocalServices(builder.Configuration);
builder.Services.AddLocalHttpClients(builder.Configuration);
builder.Services.AddLocalUnitOfWork(builder.Configuration);
#endregion

builder.Services.AddOptions();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    });
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
app.UseRouting();
app.MapControllers();
app.UseCors("AllowAllOrigins");
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/0.0.1/swagger.json", "Template API");
});

app.UseMiddleware<ControllerMiddleware>();

app.Run();
