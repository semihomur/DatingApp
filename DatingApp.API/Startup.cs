using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(x=>x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            IdentityBuilder builder = services.AddIdentityCore<User>(opt => {
                opt.Password.RequireDigit =false;
                opt.Password.RequiredLength =4;
                opt.Password.RequireNonAlphanumeric =false;
                opt.Password.RequireLowercase=false;
                opt.Password.RequireUppercase =false;
            });

            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services); 
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();
             services.AddAuthentication( o =>
                {
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                }
             )
            .AddJwtBearer(options=>{options.TokenValidationParameters =
             new TokenValidationParameters{
                ValidateIssuerSigningKey=true,
                IssuerSigningKey =new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                ValidateIssuer=false,
                ValidateAudience=false,
                ClockSkew = TimeSpan.Zero
            };
            });
            services.AddAuthorization(options=>{
                options.AddPolicy("RequireAdminRole", policy=>policy.RequireRole("Admin"));
                options.AddPolicy("ModeratePhotoRole", policy=>policy.RequireRole("Admin","Moderator"));
                options.AddPolicy("VipOnly", policy=>policy.RequireRole("VIP"));
            });
            services.AddMvc(options => {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build(); //Controllerdaki authorize gerek kalmıyor.
                options.Filters.Add(new AuthorizeFilter(policy));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(opt=>{
                    opt.SerializerSettings.ReferenceLoopHandling=Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
            services.AddCors();
            services.AddScoped<TokenModel>();
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            Mapper.Reset();
            services.AddAutoMapper();
            services.AddTransient<Seed>();
            services.AddScoped<IDatingRepository,DatingRepository>();
            //Aynı httpler için aynısını kullanır
            //AddTransient-lightweight her zaman yeni bir service yaratıyor classların kullanabilmesi için
            //AddSingleton-concurrent işlemler için sıkıntılı.
            services.AddScoped<LogUserActivity>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else    
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            seeder.SeedUsers();
            app.UseCors(x=>x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication(); 
            app.UseMvc();
        }
    }
}
