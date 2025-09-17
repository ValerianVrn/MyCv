// Copyright (c) Fives Syleps. All rights reserved.
// See License.txt in the project root for license information.

using MyCv.UI.Dtos;

namespace MyCv.UI.Services
{
    /// <inheritdoc/>
    internal class FakeInsightService : IInsightService
    {
        /// <summary>
        /// Per scope (visitor's page) in-memory intents.
        /// </summary>
        private readonly Dictionary<string, int> _intents = [];


        /// <summary>
        /// Per scope (visitor's page) in-memory domain events.
        /// </summary>
        private readonly List<DomainEvent> _domainEvents = [];

        /// <inheritdoc/>
        public async Task<ResponseResult> AddIntent(string visitorId, string intentId, int priority)
        {
            // Update other intents priority.
            foreach (var otherIntentId in _intents.Where(i => i.Value >= priority).Select(i => i.Key))
            {
                _intents[otherIntentId] += 1;
            }

            // Add the intent.
            _intents.Add(intentId, priority);
            _domainEvents.Add(new(DateTime.Now, DomainEvent.IntentAdded, intentId, priority));
            return await Task.FromResult(ResponseResult.Success());
        }

        /// <inheritdoc/>
        public async Task<ResponseResult> RemoveIntent(string visitorId, string intentId)
        {
            // Update other intents priority.
            var intentPriority = _intents[intentId];
            foreach (var otherIntentId in _intents.Where(i => i.Value >= intentPriority).Select(i => i.Key))
            {
                _intents[otherIntentId] -= 1;
            }

            // Remove the intent.
            _intents.Remove(intentId);
            _domainEvents.Add(new(DateTime.Now, DomainEvent.IntentRemoved, intentId, null));

            return await Task.FromResult(ResponseResult.Success());
        }

        /// <inheritdoc/>
        public async Task<ResponseResult> ChangePriority(string visitorId, string intentId, int newPriority)
        {
            var currentPriority = _intents[intentId];

            if (currentPriority > newPriority)
            {
                foreach (var otherIntentId in _intents.Where(d => d.Key != intentId && d.Value < currentPriority && d.Value >= newPriority).Select(i => i.Key).ToList())
                {
                    _intents[otherIntentId] += 1;
                }
            }
            else if (newPriority > currentPriority)
            {
                foreach (var otherIntentId in _intents.Where(d => d.Key != intentId && d.Value > currentPriority && d.Value <= newPriority).Select(i => i.Key).ToList())
                {
                    _intents[otherIntentId] -= 1;
                }
            }

            _intents[intentId] = newPriority;
            _domainEvents.Add(new(DateTime.Now, DomainEvent.PriorityChanged, intentId, newPriority));

            return await Task.FromResult(ResponseResult.Success());
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DomainEvent>> GetDomainEvents(string visitorId)
        {
            return await Task.FromResult<IEnumerable<DomainEvent>>(_domainEvents);
        }

        /// <inheritdoc/>
        public async Task<IDictionary<int, string>> GetPodium(string visitorId)
        {
            return await Task.FromResult<IDictionary<int, string>>(_intents.OrderBy(i => i.Value).Take(3).ToDictionary(i => i.Value, i => i.Key));
        }
    }
}
