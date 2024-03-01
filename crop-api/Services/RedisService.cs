using CROP.API.Models;
using Redis.OM;
using StackExchange.Redis;
using System;

namespace CROP.API.Services
{
    public static class RedisService
    {
        public class IndexCreationService(RedisConnectionProvider provider) : IHostedService
        {
            private readonly RedisConnectionProvider _provider = provider;

            public async Task StartAsync(CancellationToken cancellationToken)
            {
                await _provider.Connection.CreateIndexAsync(typeof(GraphData));
                await _provider.Connection.CreateIndexAsync(typeof(GraphDataRealTime));
                await _provider.Connection.CreateIndexAsync(typeof(AlarmData));
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}
