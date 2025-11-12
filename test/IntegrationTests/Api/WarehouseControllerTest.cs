using IntegrationTests.Base;
using Inv.Api.Requests.Warehouses;
using Inv.Application.Warehouses.Responses;
using Inv.Domain.Warehouses;
using Inv.Infrastructure.Database.Context;
using Inv.Infrastructure.Extensions;
using System.Net;

namespace IntegrationTests.Api
{
    [Collection(nameof(ApiNonAuthableFactory))]
    public class WarehouseControllerTest : NonAuthableControllerBase
    {

        private readonly HttpClient _client;

        public WarehouseControllerTest(ApiNonAuthableFactory factory) 
            : base(factory)
        {
            _client = Factory.HttpClient;
        }

        [Fact]
        public async Task Get_Should_ReturnData()
        {
            #region Setups

            var db = GetService<AppDbContext>();
            var entity = await AddWarehouseAsync(db);
            await SaveAsync(db);
            
            #endregion

            #region Acts

            var req = Factory.GenerateHttpRequestMessage(HttpMethod.Get, $"/v1/warehouses/{entity.Id}");
            var rsp = await _client.SendAsync(req);
            var jsonResult = await rsp.Content.ReadAsStringAsync();
            var result = jsonResult.FromJson<WarehouseDetailResponse>();
               
            #endregion

            #region Asserts

            Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Name, result.Name);
            Assert.Equal(entity.WarehouseStatus, result.WarehouseStatus);
            Assert.Null(entity.ModifiedOn);

            #endregion
        }


        [Fact]
        public async Task Put_Should_Success()
        {
            #region Setups

            var db = GetService<AppDbContext>();
            var entity = await AddWarehouseAsync(db, warehouseStatus: WarehouseStatus.Maintenance);
            await SaveAsync(db);

            var postReq = new WarehouseUpdateRequest
            {
                Name = $"{entity.Name}_new",
                WarehouseStatus = WarehouseStatus.Active
            };

            #endregion

            #region Acts

            var req = Factory.GenerateHttpRequestMessage(HttpMethod.Put, $"/v1/warehouses/{entity.Id}", fromBody: postReq);
            var rsp = await _client.SendAsync(req);

            #endregion

            #region Asserts

            Assert.Equal(HttpStatusCode.NoContent, rsp.StatusCode);
            db = GetService<AppDbContext>(); //Get a new instance of Database to test it accurately
            var updated = db.Warehouses.FirstOrDefault(c => c.Id == entity.Id);
            Assert.NotNull(updated);
            Assert.Equal(postReq.Name, updated.Name);
            Assert.Equal(postReq.WarehouseStatus, updated.WarehouseStatus);
            Assert.NotNull(updated.ModifiedOn);
            Assert.True(updated.ModifiedOn > DateTime.UtcNow.AddSeconds(-1));

            #endregion
        }
        
        //Others will be like this
    }
}
