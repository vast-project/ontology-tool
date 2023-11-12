using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VAST.Ontology.Database;
using VAST.Ontology.Database.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables("VAST_Ontology_");

// Add services to the container.
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "vast_auth";
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        //The discovery endpoint is located at: https://login.vast-project.eu/openam/oauth2/realms/root/realms/VAST_Tools/.well-known/openid-configuration
        options.Authority = builder.Configuration["Authentication:ServerAddress"];
        options.GetClaimsFromUserInfoEndpoint = true;
        options.ResponseType = "code";
        options.ResponseMode = "query";
        options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
        options.ClientId = builder.Configuration["Authentication:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:ClientSecret"];
        options.DisableTelemetry = true;
        options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
        options.RequireHttpsMetadata = true;
        options.UseTokenLifetime = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidAudience = builder.Configuration["Authentication:ClientId"],
            ValidIssuer = options.Authority,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:ClientSecret"])),
        };
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.SaveTokens = true;
    });

builder.Services.AddRazorPages();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddDbContext<VastOntologyContext>(
    options => options.UseNpgsql(@$"Server={builder.Configuration["Database:ServerName"]};Port={builder.Configuration["Database:Port"]};Database={builder.Configuration["Database:Database"]};User Id={builder.Configuration["Database:User"]};Password={builder.Configuration["Database:Password"]};"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseForwardedHeaders();
app.Use((context, next) =>
{
    context.Request.Scheme = "https";
    return next();
});

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapFallbackToFile("index.html");

//Prepare the database relations
//Moved over to a database initialization script
//using (VastOntologyContext context = new VastOntologyContext())
//{
//    if (context.RelationshipTypes.Count() == 0)
//    {
//        context.RelationshipTypes.Add(new RelationshipType()
//            {OntologyId = "P2", Name = "has type (is type of)", Description = ""});
//        context.RelationshipTypes.Add(new RelationshipType()
//            {OntologyId = "P9", Name = "consists of (forms part of)", Description = ""});
//        context.RelationshipTypes.Add(new RelationshipType()
//            {OntologyId = "P127", Name = "has broader term (has narrower term)", Description = ""});
//        context.RelationshipTypes.Add(new RelationshipType()
//            {OntologyId = "P130", Name = "shows features of (features are also found on)", Description = ""});
//        context.RelationshipTypes.Add(new RelationshipType()
//            {OntologyId = "P900.1", Name = "contradicts", Description = ""});
//        context.RelationshipTypes.Add(new RelationshipType()
//            {OntologyId = "P900.2", Name = "is contemporary", Description = ""});

//        context.SaveChanges();

//    }
//}

app.Run();
