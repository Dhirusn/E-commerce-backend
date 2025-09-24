namespace Shared.Library.Tokens
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshTokenDto token);
        Task<RefreshTokenDto?> GetByTokenAsync(string token);
        Task<IEnumerable<RefreshTokenDto>> GetByUserIdAsync(string userId);
        Task UpdateAsync(RefreshTokenDto token);
        Task RemoveAsync(string token);
    }
}
