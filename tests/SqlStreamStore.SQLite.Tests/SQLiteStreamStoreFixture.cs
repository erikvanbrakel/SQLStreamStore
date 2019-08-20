namespace SqlStreamStore
{
    using System.Data.SQLite;
    using SqlStreamStore.Infrastructure;

    public class SQLiteStreamStoreFixture : IStreamStoreFixture
    {
        private SQLiteStreamStoreSettings _settings;
        private SQLiteConnection _connection;

        public SQLiteStreamStoreFixture()
        {
            var connectionStringBuilder = new SQLiteConnectionStringBuilder
            {
                FullUri = "FullUri=file::memory:?cache=shared"
            };
            _settings = new SQLiteStreamStoreSettings(connectionStringBuilder.ConnectionString);
            Store = new SQLiteStreamStore(_settings);

            using(var connection = _settings.ConnectionFactory(_settings.ConnectionString).OpenAndReturn())
            using(var command = connection.CreateCommand())
            {
                command.CommandText = @"
CREATE TABLE IF NOT EXISTS streams
(
    id          CHAR(42)        NOT NULL,
    id_original NVARCHAR(1000)  NOT NULL,
    id_internal INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    version     INT             NOT NULL DEFAULT -1,
    position    BIGINT          NOT NULL DEFAULT -1,
    max_age     INT             NULL,
    max_count   INT             NULL,
    CONSTRAINT uq_streams_id UNIQUE (id),
    CONSTRAINT ck_version_gte_negative_one CHECK (version >= -1)
)";
                command.ExecuteNonQuery();
            }
        }
        
        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

        IStreamStore IStreamStoreFixture.Store => Store;

        public SQLiteStreamStore Store { get; private set; }

        public GetUtcNow GetUtcNow { get; set; } = SystemClock.GetUtcNow;
        public long MinPosition { get; set; } = 0;
        public int MaxSubscriptionCount { get; set; } = 100;
        public bool DisableDeletionTracking { get; set; }
    }
}