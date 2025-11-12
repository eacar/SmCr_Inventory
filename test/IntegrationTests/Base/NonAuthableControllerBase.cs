namespace IntegrationTests.Base
{
    public class NonAuthableControllerBase : SharedDataControllerBase<ApiNonAuthableFactory>, IAsyncLifetime
    {
        public NonAuthableControllerBase(ApiNonAuthableFactory factory)
            : base(factory)
        {

        }
        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task DisposeAsync()
        {
            return Factory.ResetDatabaseAsync();
        }
    }
}