namespace SqlStreamStore
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Threading;
    using System.Threading.Tasks;
    using SqlStreamStore.Infrastructure;
    using SqlStreamStore.Streams;
    using SqlStreamStore.Subscriptions;


    /// <inheritdoc />
    /// <summary>
    ///     Represents a SQLite stream store implementation.
    /// </summary>
    public class SQLiteStreamStore : StreamStoreBase

    {
        private SQLiteStreamStoreSettings _settings;

        public SQLiteStreamStore(SQLiteStreamStoreSettings settings)
            : base(settings.GetUtcNow, settings.LogName)
        {
            _settings = settings;
        }

        public SQLiteStreamStore(TimeSpan metadataMaxAgeCacheExpiry, int metadataMaxAgeCacheMaxSize, GetUtcNow getUtcNow, string logName) : base(metadataMaxAgeCacheExpiry, metadataMaxAgeCacheMaxSize, getUtcNow, logName)
        { }

        public SQLiteStreamStore(GetUtcNow getUtcNow, string logName) : base(getUtcNow, logName)
        { }

        protected override Task<ReadAllPage> ReadAllForwardsInternal(
            long fromPositionExclusive,
            int maxCount,
            bool prefetch,
            ReadNextAllPage readNext,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<ReadAllPage> ReadAllBackwardsInternal(
            long fromPositionExclusive,
            int maxCount,
            bool prefetch,
            ReadNextAllPage readNext,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<ReadStreamPage> ReadStreamForwardsInternal(
            string streamId,
            int start,
            int count,
            bool prefetch,
            ReadNextStreamPage readNext,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<ReadStreamPage> ReadStreamBackwardsInternal(
            string streamId,
            int fromVersionInclusive,
            int count,
            bool prefetch,
            ReadNextStreamPage readNext,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<long> ReadHeadPositionInternal(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override IStreamSubscription SubscribeToStreamInternal(
            string streamId,
            int? startVersion,
            StreamMessageReceived streamMessageReceived,
            SubscriptionDropped subscriptionDropped,
            HasCaughtUp hasCaughtUp,
            bool prefetchJsonData,
            string name)
        {
            throw new NotImplementedException();
        }

        protected override IAllStreamSubscription SubscribeToAllInternal(
            long? fromPosition,
            AllStreamMessageReceived streamMessageReceived,
            AllSubscriptionDropped subscriptionDropped,
            HasCaughtUp hasCaughtUp,
            bool prefetchJsonData,
            string name)
        {
            throw new NotImplementedException();
        }

        protected override Task<StreamMetadataResult> GetStreamMetadataInternal(string streamId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override async Task<ListStreamsPage> ListStreamsInternal(
            Pattern pattern,
            int maxCount,
            string continuationToken,
            ListNextStreamsPage listNextStreamsPage,
            CancellationToken cancellationToken)
        {
            if(!int.TryParse(continuationToken, out var afterIdInternal))
            {
                afterIdInternal = -1;
            }

            var streamIds = new List<string>();
            using(var connection = OpenConnection())
            using(var transaction = connection.BeginTransaction())
            using(var command = GetListStreamsCommand(pattern, maxCount, afterIdInternal, transaction))
            using(var reader = await command.ExecuteReaderAsync(cancellationToken).NotOnCapturedContext())
            {
                while(await reader.ReadAsync(cancellationToken).NotOnCapturedContext())
                {
                    streamIds.Add(reader.GetString(0));
                    afterIdInternal = reader.GetInt32(1);
                }
            }
            
            return new ListStreamsPage(afterIdInternal.ToString(), streamIds.ToArray(), listNextStreamsPage);
        }

        private SQLiteCommand GetListStreamsCommand(Pattern pattern, int maxCount, int afterIdInternal, SQLiteTransaction transaction)
        {
            var command = transaction.Connection.CreateCommand();
            command.Parameters.AddWithValue("@max_count", maxCount);
            command.Parameters.AddWithValue("@after_id_internal", afterIdInternal);
            var patternString = "%";

            switch(pattern) 
            {
                case Pattern.EndingWith p:
                    patternString = "%" + p.Value;
                    break;
                case Pattern.StartingWith p:
                    patternString = p.Value + "%";
                    break;
            }

            command.Parameters.AddWithValue("@pattern", patternString);
            command.CommandText = @"
  SELECT streams.id_original, streams.id_internal
  FROM streams
  WHERE streams.id_internal > @after_id_internal
  AND streams.id_original LIKE @pattern
  ORDER BY streams.id_internal ASC
  LIMIT @max_count";
            return command;
        }

        protected override Task<AppendResult> AppendToStreamInternal(
            string streamId,
            int expectedVersion,
            NewStreamMessage[] messages,
            CancellationToken cancellationToken)
        {
            switch(expectedVersion)
            {
                case ExpectedVersion.NoStream:
                    return AppendToStreamInternalNoStream(streamId, expectedVersion, messages, cancellationToken);
                
            }
            throw new NotImplementedException();
        }

        private Task<AppendResult> AppendToStreamInternalNoStream(
            string streamId,
            int expectedVersion,
            NewStreamMessage[] messages,
            CancellationToken cancellationToken)
        {
            using(var connection = OpenConnection())
            using(var transaction = connection.BeginTransaction())
            {
                using(var command = connection.CreateCommand())
                {
                    // create stream if not exists
                    command.CommandText = @"
        INSERT INTO streams (id, id_original)
        VALUES (@stream_id, @stream_id_original);";
                    command.Parameters.AddWithValue("@stream_id", streamId);
                    command.Parameters.AddWithValue("@stream_id_original", streamId);
                    // append messages
                    
                    transaction.Commit();
                }
            }
            
            return Task.FromResult(new AppendResult(messages.Length, messages.Length));
        }

        private SQLiteConnection OpenConnection() =>
            _settings.ConnectionFactory(_settings.ConnectionString).OpenAndReturn();

        protected override Task DeleteStreamInternal(string streamId, int expectedVersion, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task DeleteEventInternal(string streamId, Guid eventId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<SetStreamMetadataResult> SetStreamMetadataInternal(
            string streamId,
            int expectedStreamMetadataVersion,
            int? maxAge,
            int? maxCount,
            string metadataJson,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}