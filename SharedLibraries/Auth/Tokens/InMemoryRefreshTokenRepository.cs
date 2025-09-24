using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Library.Tokens
{
    public class InMemoryRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ConcurrentDictionary<string, RefreshTokenDto> _store = new();

        public Task AddAsync(RefreshTokenDto token)
        {
            _store[token.Token] = token;
            return Task.CompletedTask;
        }

        public Task<RefreshTokenDto?> GetByTokenAsync(string token)
        {
            _store.TryGetValue(token, out var rt);
            return Task.FromResult(rt);
        }

        public Task<IEnumerable<RefreshTokenDto>> GetByUserIdAsync(string userId)
        {
            var list = _store.Values.Where(x => x.UserId == userId).ToArray();
            return Task.FromResult<IEnumerable<RefreshTokenDto>>(list);
        }

        public Task UpdateAsync(RefreshTokenDto token)
        {
            _store[token.Token] = token;
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string token)
        {
            _store.TryRemove(token, out _);
            return Task.CompletedTask;
        }
    }
}
