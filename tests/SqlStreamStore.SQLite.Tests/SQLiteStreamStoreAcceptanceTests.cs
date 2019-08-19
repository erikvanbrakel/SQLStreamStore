namespace SqlStreamStore
{
    using System.Threading.Tasks;
    using Xunit.Abstractions;

    public class SQLiteStreamStoreAcceptanceTests : AcceptanceTests
    {
        public SQLiteStreamStoreAcceptanceTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        { }

        protected override Task<IStreamStoreFixture> CreateFixture()
        {
            throw new System.NotImplementedException();
        }
    }
}