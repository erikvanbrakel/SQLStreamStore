namespace SqlStreamStore
{
    using System;
    using System.Data.SQLite;
    using SqlStreamStore.Imports.Ensure.That;
    using SqlStreamStore.Infrastructure;

    public class SQLiteStreamStoreSettings
    {
        public SQLiteStreamStoreSettings(string connectionString)
        {
            Ensure.That(connectionString, nameof(connectionString)).IsNotNullOrWhiteSpace();

            ConnectionString = connectionString;

        }
        
        
        /// <summary>
        ///     The log name used for any of the log messages.
        /// </summary>
        public string LogName { get; } = nameof(SQLiteStreamStore);

        /// <summary>
        ///     Gets the connection string.
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        ///     A delegate to return the current UTC now. Used in testing to
        ///     control timestamps and time related operations.
        /// </summary>
        public GetUtcNow GetUtcNow { get; set; }

        public Func<string, SQLiteConnection> ConnectionFactory { get; set; } = connectionString => new SQLiteConnection(connectionString);
    }
}