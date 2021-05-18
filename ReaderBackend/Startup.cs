using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using ReaderBackend.Context;
using ReaderBackend.Jwt;
using ReaderBackend.Repositories;
using ReaderBackend.Scraper;
using ReaderBackend.Services;
using System;
using System.Text;

namespace ReaderBackend
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
            var jwtTokenConfig = Configuration.GetSection("JwtTokenConfig").Get<JwtTokenConfig>();
            services.AddSingleton(jwtTokenConfig);

            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtTokenConfig.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
                ValidateAudience = true,
                ValidAudience = jwtTokenConfig.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                jwt.RequireHttpsMetadata = true;
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = tokenValidationParams;
            });

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddHttpContextAccessor();

            services.AddDbContext<ReaderContext>(opt => opt.UseSqlServer
                (Configuration.GetConnectionString("ReaderConnection")));

            services.AddControllers().AddNewtonsoftJson(s =>
            {
                s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddHttpClient<IArticleScraper, ArticleScraper>();

            services.AddScoped<IWebPageService, WebPageService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();

            services.AddSingleton<IArticleScraper, ArticleScraper>();
            services.AddScoped<IJwtGenerator, JwtGenerator>();

            services.AddScoped<IWebPageRepository, WebPageRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddScoped<IJwtGenerator, JwtGenerator>();
            services.AddSingleton(tokenValidationParams);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Reader API" });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {securityScheme, new string[] { }}
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reader API");
                c.RoutePrefix = "swagger";
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}