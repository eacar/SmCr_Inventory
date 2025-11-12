namespace IntegrationTests.Base
{
    public partial class SharedDataControllerBase<TFactory> where TFactory : ApiFactoryBase
    {
        protected TFactory Factory { get; }
        protected SharedDataControllerBase(TFactory factory) { Factory = factory; }
    }
}
