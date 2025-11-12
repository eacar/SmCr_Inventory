using IntegrationTests.Base;

namespace IntegrationTests.Initializers
{
    [CollectionDefinition(nameof(ApiNonAuthableFactory))]
    public class SharedRequestTestCollection :
        ICollectionFixture<ApiNonAuthableFactory>
    {
    }
}