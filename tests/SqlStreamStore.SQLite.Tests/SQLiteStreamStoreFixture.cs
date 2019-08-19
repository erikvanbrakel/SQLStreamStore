namespace SqlStreamStore
{
    using SqlStreamStore.Infrastructure;

    public class SQLiteStreamStoreFixture : IStreamStoreFixture
    {
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        IStreamStore IStreamStoreFixture.Store => Store;
        
        public SQLiteStreamStore Store { get; private set; }

        public GetUtcNow GetUtcNow { get; set; } = SystemClock.GetUtcNow;
        public long MinPosition { get; set; } = 0;
        public int MaxSubscriptionCount { get; set; } = 100;
        public bool DisableDeletionTracking { get; set; }
    }
}