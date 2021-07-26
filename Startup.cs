using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
// using Microsoft.AspNetCore.Cors;
using Backend.Repositories;
using Backend.Services;
using Backend.Models;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Backend
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers();

      // services.AddCors(options =>
      // {
      //     options.AddPolicy(MyAllowSpecificOrigins,
      //     builder =>
      //     {
      //         builder.WithOrigins(
      //             "http://localhost:4200",
      //             "http://localhost:4201");
      //     });
      // });

      services.AddMvc()
      .AddNewtonsoftJson(
          options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
      );

      services.AddDbContext<MyContext>(options => options.UseSqlite("Data Source=db.sqlite3"));

      services
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
                // When receiving a token, check that we've signed it.
                ValidateIssuerSigningKey = true,
                // When receiving a token, check that it is still valid.
                ValidateLifetime = true,
                // This defines the maximum allowable clock skew - i.e. provides a tolerance on the token expiry time 
                // when validating the lifetime. As we're creating the tokens locally and validating them on the same 
                // machines which should have synchronised time, this can be set to zero. and default value will be 5minutes
                ClockSkew = TimeSpan.FromMinutes(0)
        };
      });

      services.AddScoped<IUserRepository, UserRepository>();
      services.AddScoped<ITicketRepository, TicketRepository>();
      services.AddScoped<IUnitOfWork, UnitOfWork>();
      services.AddScoped<UserService, UserService>();
      services.AddScoped<TicketService, TicketService>();
      services.AddScoped<InitService, InitService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();

      // app.UseAuthorization();

      // app.UseStaticFiles();
      app.UseStaticFiles(new StaticFileOptions()
      {
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Static")),
        RequestPath = new PathString("/Static")
      });

      #region Handle Exception
      app.UseExceptionHandler(appBuilder =>
      {
        appBuilder.Use(async (context, next) =>
              {
            var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;

                  //when authorization has failed, should retrun a json message to client
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
                  //when orther error, retrun a error message json to client
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
                  //when no error, do next.
                  else await next();
          });
      });
      #endregion

      app.UseRouting();

      // app.UseCors(MyAllowSpecificOrigins);
      app.UseCors(
          options => options
          .WithOrigins(
              "http://localhost:3000",
              "http://localhost:4200",
              "http://localhost:4201")
              .AllowAnyMethod()
              .AllowAnyHeader()
      );

      app.UseAuthentication();
      app.UseAuthorization();
      
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
      //app.UseMvc();              
    }
  }
}
