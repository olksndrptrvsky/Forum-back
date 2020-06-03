using BLL.Interfaces;
using BLL.Services;
using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Microsoft.Extensions.Logging;

namespace PL.Extensions
{
    public static class ForumServicesExtension
    {
        public static void AddForumServices(this IServiceCollection services, IConfiguration config, ILoggerFactory myLoggerFactory)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            services.AddScoped<IThemeService, ThemeService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IUserService, UserService>();


            services.AddAutoMapper(typeof(Startup));



            services.Configure<IdentityOptions>(options =>
            {
                //// Default Password settings.
                //options.Password.RequireDigit = true;
                //options.Password.RequireLowercase = true;
                //options.Password.RequireNonAlphanumeric = true;
                //options.Password.RequireUppercase = true;
                //options.Password.RequiredLength = 6;
                //options.Password.RequiredUniqueChars = 1;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 1;
                options.User.RequireUniqueEmail = true;
                
            });


            services.AddDbContext<ForumContext>(opt => 
                opt.UseLoggerFactory(myLoggerFactory).UseSqlServer(config.GetConnectionString("Forum")));
            services.AddIdentity<User, Role>().AddEntityFrameworkStores<ForumContext>();


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";

            })
            .AddJwtBearer("JwtBearer", jwtBarearOptions =>
            {

                jwtBarearOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
            });



        }
    }
}
