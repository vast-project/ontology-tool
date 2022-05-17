using System.Reflection;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OntologyAPI.Auth;
using VAST.Ontology.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables("VAST_Ontology_");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy  =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
});

// Add services to the container.

builder.Services
    //.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Audience = builder.Configuration["Authentication:ClientId"];
        options.Authority = builder.Configuration["Authentication:ServerAddress"];
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidAudience = builder.Configuration["Authentication:ClientId"],
            ValidIssuer = builder.Configuration["Authentication:ServerAddress"],
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:ClientSecret"])),
        };
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.Validate();

    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ontology", policy => policy.Requirements.Add(new HasScopeRequirement("cn", builder.Configuration["Authentication:ServerAddress"])));
});

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "VAST Ontology API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });
});

builder.Services.AddDbContext<VastOntologyContext>(
    options => options.UseNpgsql(@$"Server={builder.Configuration["Database:ServerName"]};Port={builder.Configuration["Database:Port"]};Database={builder.Configuration["Database:Database"]};User Id={builder.Configuration["Database:User"]};Password={builder.Configuration["Database:Password"]};"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();

