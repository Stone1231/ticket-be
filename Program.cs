using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Backend.Repositories;
using Backend.Services;
using Backend.Models;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// 配置 Kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8000); // HTTP
    serverOptions.ListenAnyIP(8001, listenOptions => // HTTPS
    {
        listenOptions.UseHttps();
    });
});

// Add services to the container.
builder.Services.AddControllers();
      
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder =>
        {
            builder.WithOrigins(
                    "http://localhost:3000", // React
                    "http://localhost:4100",
                    "http://localhost:4200",
                    "https://localhost:3000", // React HTTPS
                    "https://localhost:4100",
                    "https://localhost:4200"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials(); // 若要傳 cookie / token
        });
});        

builder.Services.AddMvc()
    .AddNewtonsoftJson(
        options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );

builder.Services.AddDbContext<MyContext>(options => options.UseSqlite("Data Source=db.sqlite3"));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            IssuerSigningKey = TokenAuthOption.Key,
            ValidAudience = TokenAuthOption.Audience,
            ValidIssuer = TokenAuthOption.Issuer,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(0)
        };
    });

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<UserService, UserService>();
builder.Services.AddScoped<TicketService, TicketService>();
builder.Services.AddScoped<InitService, InitService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

// 移除 URL 配置，因為我們已經在 Kestrel 中配置了
// app.Urls.Add("http://localhost:8000");
// app.Urls.Add("https://localhost:8000");

// 移除 HTTPS 重定向，因為我們要讓 HTTP 和 HTTPS 共存
// app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Static")),
    RequestPath = new PathString("/Static")
});

// Handle Exception
app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Use(async (context, next) =>
    {
        var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;

        if (error != null && error.Error is SecurityTokenExpiredException)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonConvert.SerializeObject(
                new
                {
                    state = 0,
                    msg = "token expired"
                })
            );
        }
        else if (error != null && error.Error != null)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(
                new
                {
                    state = -1,
                    msg = error.Error.Message
                }));
        }
        else await next();
    });
});

app.UseRouting();

// 確保 CORS 中間件在正確的位置
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
