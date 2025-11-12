using Inv.Api;
using Inv.Infrastructure.Database.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using System.Net;
using System.Text;
using System.Text.Json;

namespace IntegrationTests.Base
{
    public abstract class ApiFactoryBase : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private Respawner _respawner = null!;

        public HttpClient HttpClient { get;  }
        public string ConnString { get; set; }

        public ApiFactoryBase()
        {
            HttpClient = CreateClient();
        }
        public async Task InitializeAsync()
        {
            using (var scope = base.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
                await dbContext.Database.EnsureDeletedAsync();
                await dbContext.Database.MigrateAsync();

                _respawner = await Respawner.CreateAsync(ConnString, new RespawnerOptions
                {
                    DbAdapter = DbAdapter.SqlServer
                });

                await _respawner.ResetAsync(ConnString);
            }
        }

        public async Task ResetDatabaseAsync()
        {
            await _respawner.ResetAsync(ConnString);
        }

        public new async Task DisposeAsync()
        {
            using (var scope = base.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
                await dbContext.Database.EnsureDeletedAsync();

                await base.DisposeAsync();
            }
        }
        public virtual HttpRequestMessage GenerateHttpRequestMessage(
            HttpMethod httpMethod,
            string url,
            string? fromQuery = null,
            object? fromBody = null,
            object? fromForm = null)
        {
            var request = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = new Uri(
                    $"http://localhst:5000/api{url}" +
                    (fromQuery is not null ? $"?{fromQuery}" : string.Empty)
                ),
                Headers =
                {
                    { HttpRequestHeader.CacheControl.ToString(), "no-cache" },
                    { HttpRequestHeader.Accept.ToString(), "*/*" },
                    { HttpRequestHeader.ContentType.ToString(), "application/json" },
                    { HttpRequestHeader.AcceptEncoding.ToString(), "gzip, deflate, br" },
                    { HttpRequestHeader.Connection.ToString(), "keep-alive" }
                }
            };

            // Body vs. multipart form
            if (fromForm is MultipartFormDataContent formContent)
                request.Content = formContent;
            else if (fromBody is not null)
                request.Content = new StringContent(
                    JsonSerializer.Serialize(fromBody),
                    Encoding.UTF8,
                    "application/json"
                );

            return request;
        }
    }
}
