// Copyright (c) Fives Syleps. All rights reserved.
// See License.txt in the project root for license information.

using MyCv.UI.Components.Pages;
using MyCv.UI.Dtos;

namespace MyCv.UI.Services
{
    /// <inheritdoc/>
    internal class FakeInsightService : IInsightService
    {
        /// <inheritdoc/>
        public async Task<ResponseResult> AddIntent(string visitorId, string intentId, int priority)
        {
            return await Task.FromResult(ResponseResult.Success());
        }

        /// <inheritdoc/>
        public async Task<ResponseResult> RemoveIntent(string visitorId, string intentId)
        {
            return await Task.FromResult(ResponseResult.Success());
        }

        /// <inheritdoc/>
        public async Task<ResponseResult> ChangePriority(string visitorId, string intentId, int newPriority)
        {
            return await Task.FromResult(ResponseResult.Success());
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DomainEvent>> GetDomainEvents(string visitorId)
        {
            return await Task.FromResult<IEnumerable<DomainEvent>>([
                new(DateTime.Now.AddSeconds(-30), DomainEvent.IntentAdded, "CURIOUS", 0),
                new(DateTime.Now.AddSeconds(-25), DomainEvent.IntentAdded, "HELPING", 1),
                new(DateTime.Now.AddSeconds(-20), DomainEvent.IntentAdded, "NETWORKING", 1),
                new(DateTime.Now.AddSeconds(-15), DomainEvent.IntentAdded, "FREELANCE", 2),
                new(DateTime.Now.AddSeconds(-10), DomainEvent.PriorityChanged, "NETWORKING", 2),
                new(DateTime.Now.AddSeconds(-5), DomainEvent.IntentRemoved, "CURIOUS", null),
                ]);
        }

        /// <inheritdoc/>
        public async Task<IDictionary<int, string>> GetPodium(string visitorId)
        {
            return await Task.FromResult<IDictionary<int, string>>(new Dictionary<int, string>()
            {
                { 0,  "CURIOUS" },
                { 1,  "BROWSING"},
                { 2,  "REPUTATION"}
            });
        }
    }
}
