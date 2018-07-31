using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Theam.API.Data;
using Theam.API.Models;
using Theam.API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Theam.API.Utils;
using System.Net;
using Theam.API.Filters;
using System.Linq;

namespace Theam
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
            services.Configure<MyOptions>(Configuration.GetSection("Settings"));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = Configuration["Settings:Domain"],
                       ValidAudience = Configuration["Settings:Domain"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Settings:SecurityKey"]))
                   };
               });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.POLICIY_ADMIN_USER,
                    policy => policy
                    .RequireClaim(Constants.CLAIM_IS_ADMIN_USER));
            });

            services.AddDbContext<ApiContext>(
                opt => opt.UseLazyLoadingProxies()
                          .UseSqlite(Configuration.GetConnectionString("SqliteConnection"))
            );

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRepository<Customer>, Repository<Customer>>();
            services.AddScoped<IRepository<User>, Repository<User>>();

            services.AddAutoMapper(x => x.AddProfile(new MappingsProfile()));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
