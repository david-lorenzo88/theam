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
using System.Linq;
using System.IO;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;

namespace Theam
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Directory.CreateDirectory(Configuration["Settings:DatabaseDirectory"]);
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        ///  This method gets called by the runtime to add services to the container.
        /// </summary>
        /// <param name="services">Service Collection</param>
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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Theam CRM API", Version = Configuration["Settings:APIVersion"], Contact = new Contact { Email = "dlorenzo.1988@gmail.com", Name = "David Lorenzo López", Url = "https://github.com/david-lorenzo88" }, Description = "CRM Test API created for Theam's hiring process" });

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(security);
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


        }

        /// <summary>
        /// This method gets called by the runtime to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="env">Environment</param>
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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/"+ Configuration["Settings:APIVersion"] + "/swagger.json", "Theam CRM API");
                c.DocumentTitle = "Theam CRM API";
                c.DocExpansion(DocExpansion.None);
                
            });

            app.UseMvc();
        }
    }
}
