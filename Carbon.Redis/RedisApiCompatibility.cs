using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Carbon.Redis
{
    public partial class RedisDatabase
    {
        RedisValue StackExchange.Redis.IDatabase.HashFieldGetAndDelete(RedisKey key, RedisValue hashField, CommandFlags flags) => default;
        RedisValue[] StackExchange.Redis.IDatabase.HashFieldGetAndDelete(RedisKey key, RedisValue[] hashFields, CommandFlags flags) => default;
        RedisValue StackExchange.Redis.IDatabase.HashFieldGetAndSetExpiry(RedisKey key, RedisValue hashField, DateTime expiry, CommandFlags flags) => default;
        RedisValue StackExchange.Redis.IDatabase.HashFieldGetAndSetExpiry(RedisKey key, RedisValue hashField, TimeSpan? expiry, bool persist, CommandFlags flags) => default;
        RedisValue[] StackExchange.Redis.IDatabase.HashFieldGetAndSetExpiry(RedisKey key, RedisValue[] hashFields, DateTime expiry, CommandFlags flags) => default;
        RedisValue[] StackExchange.Redis.IDatabase.HashFieldGetAndSetExpiry(RedisKey key, RedisValue[] hashFields, TimeSpan? expiry, bool persist, CommandFlags flags) => default;
        Lease<byte> StackExchange.Redis.IDatabase.HashFieldGetLeaseAndDelete(RedisKey key, RedisValue hashField, CommandFlags flags) => default;
        Lease<byte> StackExchange.Redis.IDatabase.HashFieldGetLeaseAndSetExpiry(RedisKey key, RedisValue hashField, DateTime expiry, CommandFlags flags) => default;
        Lease<byte> StackExchange.Redis.IDatabase.HashFieldGetLeaseAndSetExpiry(RedisKey key, RedisValue hashField, TimeSpan? expiry, bool persist, CommandFlags flags) => default;
        RedisValue StackExchange.Redis.IDatabase.HashFieldSetAndSetExpiry(RedisKey key, HashEntry[] hashFields, DateTime expiry, When when, CommandFlags flags) => default;
        RedisValue StackExchange.Redis.IDatabase.HashFieldSetAndSetExpiry(RedisKey key, HashEntry[] hashFields, TimeSpan? expiry, bool keepTtl, When when, CommandFlags flags) => default;
        RedisValue StackExchange.Redis.IDatabase.HashFieldSetAndSetExpiry(RedisKey key, RedisValue field, RedisValue value, DateTime expiry, When when, CommandFlags flags) => default;
        RedisValue StackExchange.Redis.IDatabase.HashFieldSetAndSetExpiry(RedisKey key, RedisValue field, RedisValue value, TimeSpan? expiry, bool keepTtl, When when, CommandFlags flags) => default;
        StreamTrimResult StackExchange.Redis.IDatabase.StreamAcknowledgeAndDelete(RedisKey key, RedisValue groupName, StreamTrimMode mode, RedisValue messageId, CommandFlags flags) => default;
        StreamTrimResult[] StackExchange.Redis.IDatabase.StreamAcknowledgeAndDelete(RedisKey key, RedisValue groupName, StreamTrimMode mode, RedisValue[] messageIds, CommandFlags flags) => default;
        RedisValue StackExchange.Redis.IDatabase.StreamAdd(RedisKey key, NameValueEntry[] streamPairs, RedisValue? messageId, long? maxLength, bool useApproximateMaxLength, long? limit, StreamTrimMode trimMode, CommandFlags flags) => default;
        RedisValue StackExchange.Redis.IDatabase.StreamAdd(RedisKey key, RedisValue streamField, RedisValue streamValue, RedisValue? messageId, long? maxLength, bool useApproximateMaxLength, long? limit, StreamTrimMode trimMode, CommandFlags flags) => default;
        StreamTrimResult[] StackExchange.Redis.IDatabase.StreamDelete(RedisKey key, RedisValue[] messageIds, StreamTrimMode mode, CommandFlags flags) => default;
        StreamPendingMessageInfo[] StackExchange.Redis.IDatabase.StreamPendingMessages(RedisKey key, RedisValue groupName, int count, RedisValue consumerName, RedisValue? minId, RedisValue? maxId, long? minIdleTimeInMs, CommandFlags flags) => default;
        RedisStream[] StackExchange.Redis.IDatabase.StreamReadGroup(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream, bool noAck, TimeSpan? claimMinIdleTime, CommandFlags flags) => default;
        StreamEntry[] StackExchange.Redis.IDatabase.StreamReadGroup(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position, int? count, bool noAck, TimeSpan? claimMinIdleTime, CommandFlags flags) => default;
        long StackExchange.Redis.IDatabase.StreamTrim(RedisKey key, long maxLength, bool useApproximateMaxLength, long? limit, StreamTrimMode mode, CommandFlags flags) => default;
        long StackExchange.Redis.IDatabase.StreamTrimByMinId(RedisKey key, RedisValue minId, bool useApproximateMaxLength, long? limit, StreamTrimMode mode, CommandFlags flags) => default;
        bool StackExchange.Redis.IDatabase.StringDelete(RedisKey key, ValueCondition when, CommandFlags flags) => default;
        ValueCondition? StackExchange.Redis.IDatabase.StringDigest(RedisKey key, CommandFlags flags) => default;
        bool StackExchange.Redis.IDatabase.StringSet(RedisKey key, RedisValue value, Expiration expiry, ValueCondition when, CommandFlags flags) => default;
        bool StackExchange.Redis.IDatabase.StringSet(KeyValuePair<RedisKey, RedisValue>[] values, When when, Expiration expiry, CommandFlags flags) => default;
#pragma warning disable SER001
        bool StackExchange.Redis.IDatabase.VectorSetAdd(RedisKey key, VectorSetAddRequest request, CommandFlags flags) => default;
        bool StackExchange.Redis.IDatabase.VectorSetContains(RedisKey key, RedisValue member, CommandFlags flags) => default;
        int StackExchange.Redis.IDatabase.VectorSetDimension(RedisKey key, CommandFlags flags) => default;
        Lease<float> StackExchange.Redis.IDatabase.VectorSetGetApproximateVector(RedisKey key, RedisValue member, CommandFlags flags) => default;
        string StackExchange.Redis.IDatabase.VectorSetGetAttributesJson(RedisKey key, RedisValue member, CommandFlags flags) => default;
        Lease<RedisValue> StackExchange.Redis.IDatabase.VectorSetGetLinks(RedisKey key, RedisValue member, CommandFlags flags) => default;
        Lease<VectorSetLink> StackExchange.Redis.IDatabase.VectorSetGetLinksWithScores(RedisKey key, RedisValue member, CommandFlags flags) => default;
        VectorSetInfo? StackExchange.Redis.IDatabase.VectorSetInfo(RedisKey key, CommandFlags flags) => default;
        long StackExchange.Redis.IDatabase.VectorSetLength(RedisKey key, CommandFlags flags) => default;
        RedisValue StackExchange.Redis.IDatabase.VectorSetRandomMember(RedisKey key, CommandFlags flags) => default;
        RedisValue[] StackExchange.Redis.IDatabase.VectorSetRandomMembers(RedisKey key, long count, CommandFlags flags) => default;
        bool StackExchange.Redis.IDatabase.VectorSetRemove(RedisKey key, RedisValue member, CommandFlags flags) => default;
        bool StackExchange.Redis.IDatabase.VectorSetSetAttributesJson(RedisKey key, RedisValue member, string attributesJson, CommandFlags flags) => default;
        Lease<VectorSetSimilaritySearchResult> StackExchange.Redis.IDatabase.VectorSetSimilaritySearch(RedisKey key, VectorSetSimilaritySearchRequest query, CommandFlags flags) => default;
#pragma warning restore SER001

        Task<RedisValue[]> StackExchange.Redis.IDatabaseAsync.HashFieldGetAndDeleteAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags) => default;
        Task<RedisValue> StackExchange.Redis.IDatabaseAsync.HashFieldGetAndDeleteAsync(RedisKey key, RedisValue hashField, CommandFlags flags) => default;
        Task<RedisValue[]> StackExchange.Redis.IDatabaseAsync.HashFieldGetAndSetExpiryAsync(RedisKey key, RedisValue[] hashFields, DateTime expiry, CommandFlags flags) => default;
        Task<RedisValue[]> StackExchange.Redis.IDatabaseAsync.HashFieldGetAndSetExpiryAsync(RedisKey key, RedisValue[] hashFields, TimeSpan? expiry, bool persist, CommandFlags flags) => default;
        Task<RedisValue> StackExchange.Redis.IDatabaseAsync.HashFieldGetAndSetExpiryAsync(RedisKey key, RedisValue hashField, DateTime expiry, CommandFlags flags) => default;
        Task<RedisValue> StackExchange.Redis.IDatabaseAsync.HashFieldGetAndSetExpiryAsync(RedisKey key, RedisValue hashField, TimeSpan? expiry, bool persist, CommandFlags flags) => default;
        Task<Lease<byte>> StackExchange.Redis.IDatabaseAsync.HashFieldGetLeaseAndDeleteAsync(RedisKey key, RedisValue hashField, CommandFlags flags) => default;
        Task<Lease<byte>> StackExchange.Redis.IDatabaseAsync.HashFieldGetLeaseAndSetExpiryAsync(RedisKey key, RedisValue hashField, DateTime expiry, CommandFlags flags) => default;
        Task<Lease<byte>> StackExchange.Redis.IDatabaseAsync.HashFieldGetLeaseAndSetExpiryAsync(RedisKey key, RedisValue hashField, TimeSpan? expiry, bool persist, CommandFlags flags) => default;
        Task<RedisValue> StackExchange.Redis.IDatabaseAsync.HashFieldSetAndSetExpiryAsync(RedisKey key, HashEntry[] hashFields, DateTime expiry, When when, CommandFlags flags) => default;
        Task<RedisValue> StackExchange.Redis.IDatabaseAsync.HashFieldSetAndSetExpiryAsync(RedisKey key, HashEntry[] hashFields, TimeSpan? expiry, bool keepTtl, When when, CommandFlags flags) => default;
        Task<RedisValue> StackExchange.Redis.IDatabaseAsync.HashFieldSetAndSetExpiryAsync(RedisKey key, RedisValue field, RedisValue value, DateTime expiry, When when, CommandFlags flags) => default;
        Task<RedisValue> StackExchange.Redis.IDatabaseAsync.HashFieldSetAndSetExpiryAsync(RedisKey key, RedisValue field, RedisValue value, TimeSpan? expiry, bool keepTtl, When when, CommandFlags flags) => default;
        Task<StreamTrimResult[]> StackExchange.Redis.IDatabaseAsync.StreamAcknowledgeAndDeleteAsync(RedisKey key, RedisValue groupName, StreamTrimMode mode, RedisValue[] messageIds, CommandFlags flags) => default;
        Task<StreamTrimResult> StackExchange.Redis.IDatabaseAsync.StreamAcknowledgeAndDeleteAsync(RedisKey key, RedisValue groupName, StreamTrimMode mode, RedisValue messageId, CommandFlags flags) => default;
        Task<RedisValue> StackExchange.Redis.IDatabaseAsync.StreamAddAsync(RedisKey key, NameValueEntry[] streamPairs, RedisValue? messageId, long? maxLength, bool useApproximateMaxLength, long? limit, StreamTrimMode trimMode, CommandFlags flags) => default;
        Task<RedisValue> StackExchange.Redis.IDatabaseAsync.StreamAddAsync(RedisKey key, RedisValue streamField, RedisValue streamValue, RedisValue? messageId, long? maxLength, bool useApproximateMaxLength, long? limit, StreamTrimMode trimMode, CommandFlags flags) => default;
        Task<StreamTrimResult[]> StackExchange.Redis.IDatabaseAsync.StreamDeleteAsync(RedisKey key, RedisValue[] messageIds, StreamTrimMode mode, CommandFlags flags) => default;
        Task<StreamPendingMessageInfo[]> StackExchange.Redis.IDatabaseAsync.StreamPendingMessagesAsync(RedisKey key, RedisValue groupName, int count, RedisValue consumerName, RedisValue? minId, RedisValue? maxId, long? minIdleTimeInMs, CommandFlags flags) => default;
        Task<RedisStream[]> StackExchange.Redis.IDatabaseAsync.StreamReadGroupAsync(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream, bool noAck, TimeSpan? claimMinIdleTime, CommandFlags flags) => default;
        Task<StreamEntry[]> StackExchange.Redis.IDatabaseAsync.StreamReadGroupAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position, int? count, bool noAck, TimeSpan? claimMinIdleTime, CommandFlags flags) => default;
        Task<long> StackExchange.Redis.IDatabaseAsync.StreamTrimAsync(RedisKey key, long maxLength, bool useApproximateMaxLength, long? limit, StreamTrimMode mode, CommandFlags flags) => default;
        Task<long> StackExchange.Redis.IDatabaseAsync.StreamTrimByMinIdAsync(RedisKey key, RedisValue minId, bool useApproximateMaxLength, long? limit, StreamTrimMode mode, CommandFlags flags) => default;
        Task<bool> StackExchange.Redis.IDatabaseAsync.StringDeleteAsync(RedisKey key, ValueCondition when, CommandFlags flags) => default;
        Task<ValueCondition?> StackExchange.Redis.IDatabaseAsync.StringDigestAsync(RedisKey key, CommandFlags flags) => default;
        Task<bool> StackExchange.Redis.IDatabaseAsync.StringSetAsync(RedisKey key, RedisValue value, Expiration expiry, ValueCondition when, CommandFlags flags) => default;
        Task<bool> StackExchange.Redis.IDatabaseAsync.StringSetAsync(KeyValuePair<RedisKey, RedisValue>[] values, When when, Expiration expiry, CommandFlags flags) => default;
#pragma warning disable SER001
        Task<bool> StackExchange.Redis.IDatabaseAsync.VectorSetAddAsync(RedisKey key, VectorSetAddRequest request, CommandFlags flags) => default;
        Task<bool> StackExchange.Redis.IDatabaseAsync.VectorSetContainsAsync(RedisKey key, RedisValue member, CommandFlags flags) => default;
        Task<int> StackExchange.Redis.IDatabaseAsync.VectorSetDimensionAsync(RedisKey key, CommandFlags flags) => default;
        Task<Lease<float>> StackExchange.Redis.IDatabaseAsync.VectorSetGetApproximateVectorAsync(RedisKey key, RedisValue member, CommandFlags flags) => default;
        Task<string> StackExchange.Redis.IDatabaseAsync.VectorSetGetAttributesJsonAsync(RedisKey key, RedisValue member, CommandFlags flags) => default;
        Task<Lease<RedisValue>> StackExchange.Redis.IDatabaseAsync.VectorSetGetLinksAsync(RedisKey key, RedisValue member, CommandFlags flags) => default;
        Task<Lease<VectorSetLink>> StackExchange.Redis.IDatabaseAsync.VectorSetGetLinksWithScoresAsync(RedisKey key, RedisValue member, CommandFlags flags) => default;
        Task<VectorSetInfo?> StackExchange.Redis.IDatabaseAsync.VectorSetInfoAsync(RedisKey key, CommandFlags flags) => default;
        Task<long> StackExchange.Redis.IDatabaseAsync.VectorSetLengthAsync(RedisKey key, CommandFlags flags) => default;
        Task<RedisValue> StackExchange.Redis.IDatabaseAsync.VectorSetRandomMemberAsync(RedisKey key, CommandFlags flags) => default;
        Task<RedisValue[]> StackExchange.Redis.IDatabaseAsync.VectorSetRandomMembersAsync(RedisKey key, long count, CommandFlags flags) => default;
        Task<bool> StackExchange.Redis.IDatabaseAsync.VectorSetRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags) => default;
        Task<bool> StackExchange.Redis.IDatabaseAsync.VectorSetSetAttributesJsonAsync(RedisKey key, RedisValue member, string attributesJson, CommandFlags flags) => default;
        Task<Lease<VectorSetSimilaritySearchResult>> StackExchange.Redis.IDatabaseAsync.VectorSetSimilaritySearchAsync(RedisKey key, VectorSetSimilaritySearchRequest query, CommandFlags flags) => default;
#pragma warning restore SER001

        ExpireResult[] StackExchange.Redis.IDatabase.HashFieldExpire(RedisKey key, RedisValue[] hashFields, TimeSpan expiry, ExpireWhen when, CommandFlags flags) => default;
        ExpireResult[] StackExchange.Redis.IDatabase.HashFieldExpire(RedisKey key, RedisValue[] hashFields, DateTime expiry, ExpireWhen when, CommandFlags flags) => default;
        PersistResult[] StackExchange.Redis.IDatabase.HashFieldPersist(RedisKey key, RedisValue[] hashFields, CommandFlags flags) => default;
        Task<ExpireResult[]> StackExchange.Redis.IDatabaseAsync.HashFieldExpireAsync(RedisKey key, RedisValue[] hashFields, TimeSpan expiry, ExpireWhen when, CommandFlags flags) => default;
        Task<ExpireResult[]> StackExchange.Redis.IDatabaseAsync.HashFieldExpireAsync(RedisKey key, RedisValue[] hashFields, DateTime expiry, ExpireWhen when, CommandFlags flags) => default;
        Task<PersistResult[]> StackExchange.Redis.IDatabaseAsync.HashFieldPersistAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags) => default;
    }

    public partial class DummyConnectionMultiplexer
    {
        IServer StackExchange.Redis.IConnectionMultiplexer.GetServer(RedisKey key, object asyncState, CommandFlags flags) => new DummyRedisServer();
    }

    public partial class DummyRedisServer
    {
        RedisResult StackExchange.Redis.IServer.Execute(int? database, string command, ICollection<object> args, CommandFlags flags) => default;
        Task<RedisResult> StackExchange.Redis.IServer.ExecuteAsync(int? database, string command, ICollection<object> args, CommandFlags flags) => default;
    }
}