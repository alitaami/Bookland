using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Web;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using Wallet.Infrastructure.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System.Text.Json.Serialization;
using Order.Core.Interfaces;
using Order.Application;

namespace WebFramework.Configuration
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
        {
            var logger = LogManager
                .Setup()
                .LoadConfigurationFromFile("nlog.config")
                .GetCurrentClassLogger();

            try
            {
                if (builder is null)
                    throw new ArgumentNullException(nameof(builder));

                ConfigLogging(builder);

                var configuration = builder.Configuration;

                SetupNlog(builder);

                AddAppServices(builder);

                AddHealthChecks(builder);

                AddMvcAndJsonOptions(builder);

                AddMinimalMvc(builder);

                AddCustomApiVersioning(builder);

                //AddSwagger(builder);

                AddCors(builder);

                AddAuthentication(builder);

                AddAppHsts(builder);


#if !DEBUG
           //ApplyRemainingMigrations(builder); // TODO : بررسی بشه امکان اجرا در این حالت داره یا نه و گرنه باید به قسمت middleware برده بشه
#endif
                return builder;
            }
            catch (Exception ex)
            {
                logger.Error(ex);

                throw;
            }
        }
        private static void AddCors(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
        }
        private static void AddAuthentication(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // Set to true if you want to validate the issuer
                    ValidateAudience = false, // Set to true if you want to validate the audience
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("g0NTnMYpskKBSufh5HBpD_Dpdx6r5cCz7zC6L_YhBvw=")),
                    ClockSkew = TimeSpan.Zero // Adjust as needed
                };
            });
        }
        private static void AddHealthChecks(WebApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks()
                .AddNpgSql(builder.Configuration["ConnectionStrings:BookLandDB"], name: "PostgreSQL Health Check");
        }

        private static void AddCustomApiVersioning(WebApplicationBuilder builder)
        {
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;

                ApiVersion.TryParse("1.0", out var version10);
                ApiVersion.TryParse("1", out var version1);
                var a = version10 == version1; // Note: This is unused. Do you need it?

                // Uncomment the version reader you wish to use. Do not set multiple, 
                // as each one will overwrite the previous one.

                // Use query string for versioning e.g., api/posts?api-version=1
                // options.ApiVersionReader = new QueryStringApiVersionReader("api-version"); 

                // Use URL segment for versioning e.g., api/v1/posts
                // options.ApiVersionReader = new UrlSegmentApiVersionReader(); 

                // Use header for versioning e.g., Header => Api-Version: 1
                // options.ApiVersionReader = new HeaderApiVersionReader(new[] { "Api-Version" }); 

                // Use MediaType for versioning (this requires more setup)
                // options.ApiVersionReader = new MediaTypeApiVersionReader();

                // Use a combination of query string and URL segment
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new UrlSegmentApiVersionReader()
                );
            });
        }

        private static void AddAppServices(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<ApplicationDbContext>();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
            });

            // Register other application services.
            builder.Services.AddApplicationServices();

            // Register Repository as transient.
            builder.Services.AddTransient<IOrderService, OrderService>();

            // Configure IISServerOptions if needed.
            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        //private static void ApplyRemainingMigrations(WebApplicationBuilder builder)
        //{
        //    var serviceScopeFactory = builder.Services.BuildServiceProvider().GetService<IServiceScopeFactory>();
        //    using (var serviceScope = serviceScopeFactory.CreateScope())
        //    {
        //        var dbContext = serviceScope.ServiceProvider.GetRequiredService<TavContext>();
        //        dbContext.Database.Migrate();
        //    }
        //}
        private static void SetupNlog(WebApplicationBuilder builder)
        {

            ILoggerFactory loggerFactory = new LoggerFactory();
            LogManager.Setup().LoadConfigurationFromFile("nlog.config");
            //loggerFactory.AddNLog().ConfigureNLog("nlog.config");
#if DEBUG
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Logging.AddEventSourceLogger();
            builder.Logging.AddEventLog();
            builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
#endif
            builder.Logging.ClearProviders();
            builder.Logging.AddNLogWeb();
            builder.Host.UseNLog();

        }

        private static void AddMvcAndJsonOptions(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers()
                           .AddJsonOptions(options =>
                           {
                               options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
                               options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                           })
                           .AddNewtonsoftJson(options =>
                           {
                               // Your existing settings for Newtonsoft.Json
                               options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                               options.SerializerSettings.Converters.Add(new StringEnumConverter());
                               options.SerializerSettings.Culture = new CultureInfo("en");
                               options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                               options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                               options.SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
                               options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                               options.SerializerSettings.ContractResolver = new DefaultContractResolver
                               {
                                   NamingStrategy = new SnakeCaseNamingStrategy()
                               };
                               options.AllowInputFormatterExceptionMessages = true;
                           });
        }

        private static void AddAppHsts(WebApplicationBuilder builder)
        {
            builder.Services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
                //options.ExcludedHosts.Add("example.com");
                //options.ExcludedHosts.Add("www.example.com");
            });
        }

        public static void AddMinimalMvc(WebApplicationBuilder builder)
        {
            //https://github.com/aspnet/AspNetCore/blob/0303c9e90b5b48b309a78c2ec9911db1812e6bf3/src/Mvc/Mvc/src/MvcServiceCollectionExtensions.cs
            builder.Services.AddControllers(options =>
            {
                //Apply AuthorizeFilter as global filter to all actions
                //options.Filters.Add(new AuthorizeFilter()); 

                //Like [ValidateAntiforgeryToken] attribute but dose not validatie for GET and HEAD http method
                //You can ingore validate by using [IgnoreAntiforgeryToken] attribute
                //Use this filter when use cookie 
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

                //options.UseYeKeModelBinder();
            }).AddNewtonsoftJson(option =>
            {
                option.SerializerSettings.Converters.Add(new StringEnumConverter());
                option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //option.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                //option.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });
            //builder.Services.AddSwaggerGenNewtonsoftSupport();

            #region Old way (We don't need this from ASP.NET Core 3.0 onwards)
            ////https://github.com/aspnet/Mvc/blob/release/2.2/src/Microsoft.AspNetCore.Mvc/MvcServiceCollectionExtensions.cs
            //services.AddMvcCore(options =>
            //{
            //    options.Filters.Add(new AuthorizeFilter());

            //    //Like [ValidateAntiforgeryToken] attribute but dose not validatie for GET and HEAD http method
            //    //You can ingore validate by using [IgnoreAntiforgeryToken] attribute
            //    //Use this filter when use cookie 
            //    //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

            //    //options.UseYeKeModelBinder();
            //})
            //.AddApiExplorer()
            //.AddAuthorization()
            //.AddFormatterMappings()
            //.AddDataAnnotations()
            //.AddJsonOptions(option =>
            //{
            //    //option.JsonSerializerOptions
            //})
            //.AddNewtonsoftJson(/*option =>
            //{
            //    option.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            //    option.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            //}*/)

            ////Microsoft.AspNetCore.Mvc.Formatters.Json
            ////.AddJsonFormatters(/*options =>
            ////{
            ////    options.Formatting = Newtonsoft.Json.Formatting.Indented;
            ////    options.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            ////}*/)

            //.AddCors()
            //.SetCompatibilityVersion(CompatibilityVersion.Latest); //.SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            #endregion
        }

        private static void ConfigLogging(WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddNLogWeb();
            builder.Host.UseNLog();
        }
    }
}
