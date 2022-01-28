using ASP.NET_API.Data;
using ASP.NET_API.Filters;
using ASP.NET_API.Helpers;
using ASP.NET_API.Middlewares;
using ASP.NET_API.Services;
using ASP.NET_API.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers( opt =>
{
    opt.Filters.Add(typeof(ExceptionFilter));
    opt.Conventions.Add(new SwaggerGroupByVersion());
}).AddJsonOptions(x => 
    x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles)
  .AddNewtonsoftJson();

builder.Services.AddDbContext<ApplicationDbContext>(
    opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt => opt.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"])),
                    ClockSkew = TimeSpan.Zero,
                });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {  
        Title = "AuthorsWebAPI", 
        Version = "v1",
        Description = "This a Asp.Net 6 API for educational purposes",
        Contact = new OpenApiContact
        {
            Name = "Alonso Ugalde Aguilar",
            Email = "augaldecr@gmail.com",
            Url = new Uri("https://www.linkedin.com/in/augaldecr/"),
        }
    });
    c.SwaggerDoc("v2", new OpenApiInfo {  
        Title = "AuthorsWebAPI", 
        Version = "v2",
        Description = "This a Asp.Net 6 API for educational purposes",
        Contact = new OpenApiContact
        {
            Name = "Alonso Ugalde Aguilar",
            Email = "augaldecr@gmail.com",
            Url = new Uri("https://www.linkedin.com/in/augaldecr/"),
        }
    });
    c.OperationFilter<AddHATEOASParameter>();
    c.OperationFilter<AddXVersionParameter>();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                }
            },
            new string[] { }
        }
    });

    var fileXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var routeXML = Path.Combine(AppContext.BaseDirectory, fileXML);
    c.IncludeXmlComments(routeXML);
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy(PoliciesHelper.IsAnAdmin, pol => pol.RequireClaim(PoliciesHelper.IsAnAdmin));
    // opt.AddPolicy("IsATeacher", pol => pol.RequireClaim("IsATeacher"));
});

builder.Services.AddDataProtection();
builder.Services.AddTransient<HashService>();

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://127.0.0.1").AllowAnyMethod().AllowAnyHeader()
                .WithExposedHeaders(new string[] { "recordsQty" });
    });
});

builder.Services.AddTransient<GenerateLinksService>();
builder.Services.AddTransient<HATEOASAuthorFilterAttribute>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthorsWebAPI v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "AuthorsWebAPI v2");
        });
}

//app.UseMiddleware<LogHttpResponsesMiddleware>();
//app.UseLogHttpResponses();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
