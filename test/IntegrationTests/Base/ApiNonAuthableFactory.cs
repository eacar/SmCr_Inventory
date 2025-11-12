using IntegrationTests.Initializers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace IntegrationTests.Base
{
    public class ApiNonAuthableFactory : ApiFactoryBase
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment("Integration");

            builder.ConfigureAppConfiguration((ctx, cfg) =>
            {
                // remove AppSettings.* sources if Program.cs added them and you want to replace
                var toRemove = cfg.Sources
                    .OfType<Microsoft.Extensions.Configuration.Json.JsonConfigurationSource>()
                    .Where(s => s.Path.StartsWith("AppSettings.", StringComparison.OrdinalIgnoreCase))
                    .ToList();
                foreach (var s in toRemove) cfg.Sources.Remove(s);

                cfg.AddJsonFile("AppSettings.Integration.json", optional: false, reloadOnChange: true);

                // read snapshot
                var temp = cfg.Build();
                ConnString = temp.GetConnectionString("Main");
            });
            builder.ConfigureServices(services =>
            {
                #region Database

                services.AddTransient<ITestOutputHelper, TestOutputHelper>();

                #endregion

                /* Overriding policies and adding Test Scheme defined in TestAuthHandler */
                services.AddMvc(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes("Test")
                        .Build();

                    options.Filters.Add(new AuthorizeFilter(policy));
                });

                /* Adding Default Authentication schemes
                    This is enough to prevent unauthorized client (if you wish to have that kind of tests) */
                services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = "Test";
                        options.DefaultChallengeScheme = "Test";
                        options.DefaultScheme = "Test";
                    })
                    /* Here we're adding TestAuthentication Scheme with Handler that will Authenticate our client based on Claims */
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { }
                    );

                services.PostConfigure<AuthorizationOptions>(o =>
                {
                    o.FallbackPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes("Test")
                        .Build();
                });
            });

            return base.CreateHost(builder);
        }
    }
}