using ApiEcommerce.Constants;
using ApiEcommerce.Data;
using ApiEcommerce.Models;
using ApiEcommerce.Repository;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Mapster;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var dbConnectionString = builder.Configuration.GetConnectionString("ConexionSql");
// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(dbConnectionString)
  .UseSeeding((context, _) =>
  {
      var appContext = (ApplicationDbContext)context;
      DataSeeder.SeedData(appContext);
  })
);
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024*1024;
    options.UseCaseSensitivePaths = true;
});

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository,UserRepository>();
// Mapster: scan assembly for IRegister implementations to configure mappings
TypeAdapterConfig.GlobalSettings.Scan(typeof(Program).Assembly);
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");
if (String.IsNullOrEmpty(secretKey)) 
{
    throw new InvalidOperationException("Secretkey no configurada");
}
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options => {
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuerSigningKey = true,
        ValidateAudience = false

    };

});
builder.Services.AddControllers(option => 
{
    option.CacheProfiles.Add(CacheProfiles.Default10, CacheProfiles.Profile10);
    option.CacheProfiles.Add(CacheProfiles.Default20, CacheProfiles.Profile20);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "Api Ecommerce",
            Version = "v1",
            Description = "Api para Gestionar productos y categorías",
            TermsOfService = new Uri("https://example.com/terms"),
            Contact = new OpenApiContact
            {
                Name = "DevTalles",
                Url = new Uri("https://devtalles.com")
            },
            License =new OpenApiLicense
            { 
                Name = "Licencia de uso",
                Url = new Uri("https://example.com/license"),
            }
        };
        //document.Info = new OpenApiInfo
        //{
        //    Title = "Api Ecommerce V2",
        //    Version = "v2",
        //    Description = "Api para Gestionar productos y categorías",
        //    TermsOfService = new Uri("https://example.com/terms"),
        //    Contact = new OpenApiContact
        //    {
        //        Name = "DevTalles",
        //        Url = new Uri("https://devtalles.com")
        //    },
        //    License = new OpenApiLicense
        //    {
        //        Name = "Licencia de uso",
        //        Url = new Uri("https://example.com/license"),
        //    }
        //};
        document.Components ??= new OpenApiComponents();

        // CORRECCIÓN: Usar IOpenApiSecurityScheme en lugar de OpenApiSecurityScheme
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        var schemeName = "Bearer";

        // Al instanciar el objeto concreto, se almacena sin problemas en el diccionario de interfaces
        document.Components.SecuritySchemes[schemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header
        };

        document.Security ??= [];

        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });

        return Task.CompletedTask;
    });
});

var apiVersioningBuilder = builder.Services.AddApiVersioning(option=>
{
    option.AssumeDefaultVersionWhenUnspecified = true;
    option.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    option.ReportApiVersions = true;
    //soption.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("api-version")); //Api version
});

apiVersioningBuilder.AddApiExplorer(option => 
{
    option.GroupNameFormat = "'v'VVV";
    option.SubstituteApiVersionInUrl = true;
    
});

builder.Services.AddCors(options=>
    {
        options.AddPolicy(PolicyNames.AllowSpecificOrigin,
            builder => {
                builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
            });
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); //

    // Map the Scalar API Reference UI
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Api Commerce.NET 10 API")
               .WithTheme(ScalarTheme.DeepSpace) // Options include Mars, DeepSpace, etc.
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecuritySchemes = ["Bearer"]
        };

    });
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors(PolicyNames.AllowSpecificOrigin);
app.UseResponseCaching();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
