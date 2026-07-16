using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.DTOs.ViewModels;
using SilentMoon.Application.Interfaces.Caching;
using SilentMoon.Application.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Onboarding.Queries.GetTopics
{
    public class GetTopicsQuery : IQuery<List<TopicViewModel>>
    {
    }

    public class GetTopicsQueryHandler : IQueryHandler<GetTopicsQuery, List<TopicViewModel>>
    {
        public const string CacheKey = "topics:active";

        private readonly IUow _uow;
        private readonly ICacheService _cacheService;

        public GetTopicsQueryHandler(IUow uow, ICacheService cacheService)
        {
            _uow = uow;
            _cacheService = cacheService;
        }

        public async Task<Result<List<TopicViewModel>>> Handle(GetTopicsQuery query, CancellationToken ct)
        {
            var topics = await _cacheService.GetOrAddAsync(CacheKey, async () =>
            {
                var entities = await _uow.TopicRepository.GetActiveTopicsAsync(ct);
                return entities.Select(t => t.ToTopicViewModel()).ToList();
            }, System.TimeSpan.FromHours(6));

            return topics;
        }
    }
}
