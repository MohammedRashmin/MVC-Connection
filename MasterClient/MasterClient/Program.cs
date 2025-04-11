using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;

namespace MasterClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Controllers with Views
            builder.Services.AddControllersWithViews();

            // Configure JWT Authentication in MasterClient
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],  // Same as the issuer in MasterAdmin
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["Jwt:Audience"],  // Same as the audience in MasterAdmin
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            // Retrieve JWT token from cookies or header
                            var token = context.Request.Cookies["AuthToken"];
                            if (string.IsNullOrEmpty(token))
                            {
                                token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                            }
                            if (!string.IsNullOrEmpty(token))
                            {
                                context.Token = token;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            var app = builder.Build();

            // Middleware to read JWT from cookie and add it to Authorization header
            app.Use(async (context, next) =>
            {
                var token = context.Request.Cookies["AuthToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers["Authorization"] = $"Bearer {token}";
                }
                await next();
            });

            // Middleware to redirect to login if unauthorized
            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == 401 || context.Response.StatusCode == 403)
                {
                    var returnUrl = Uri.EscapeDataString(context.Request.GetDisplayUrl());
                    context.Response.Redirect($"https://localhost:7275/Login/Login?returnUrl={returnUrl}");
                }
            });

            // Middleware setup
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // Enable Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Map default controller route
            app.MapDefaultControllerRoute();

            app.Run();
        }
    }
}
