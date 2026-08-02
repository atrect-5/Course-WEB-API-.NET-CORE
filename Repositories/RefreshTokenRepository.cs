using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;
using Microsoft.Extensions.Configuration;

namespace Repositories
{
    public class RefreshTokenRepository(ProjectDBContext context, IConfiguration configuration) : IRefreshTokenService
    {
        private readonly ProjectDBContext _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        private string GetSecretKey()
        {
            var key = _configuration.GetSection("RefreshTokenSettings")["SecretKey"];
            if (string.IsNullOrEmpty(key))
                throw new InvalidOperationException("RefreshTokenSettings:SecretKey is not configured.");
            return key;
        }

        public async Task<(string Token, DateTime ExpiresAt)> CreateRefreshTokenAsync(int userId, string createdByIp, string? userAgent, int validDays)
        {
            if (validDays <= 0) throw new ArgumentOutOfRangeException(nameof(validDays));

            var (token, expires) = RefreshTokenUtils.GenerateRefreshToken(validDays: validDays);
            var hash = RefreshTokenUtils.HashTokenHmacSha256(token, GetSecretKey());

            var entity = new RefreshToken
            {
                UserId = userId,
                TokenHash = hash,
                ExpiresAt = expires,
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = createdByIp,
                UserAgent = userAgent
            };

            await _dbContext.RefreshTokens.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return (token, expires);
        }

        public async Task<RefreshToken?> GetByRawTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;
            var hash = RefreshTokenUtils.HashTokenHmacSha256(token, GetSecretKey());
            return await _dbContext.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.TokenHash == hash);
        }

        public async Task RevokeAsync(RefreshToken tokenRecord, string? replacedByTokenHash = null)
        {
            ArgumentNullException.ThrowIfNull(tokenRecord);
            if (tokenRecord.RevokedAt != null) return; // already revoked

            tokenRecord.RevokedAt = DateTime.UtcNow;
            tokenRecord.ReplacedByTokenHash = replacedByTokenHash;
            _dbContext.RefreshTokens.Update(tokenRecord);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<(string NewToken, DateTime ExpiresAt)> RotateAsync(RefreshToken existingToken, string createdByIp, string? userAgent, int validDays)
        {
            ArgumentNullException.ThrowIfNull(existingToken);
            if (validDays <= 0) throw new ArgumentOutOfRangeException(nameof(validDays));

            // generate new token
            var (newToken, newExpires) = RefreshTokenUtils.GenerateRefreshToken(validDays: validDays);
            var newHash = RefreshTokenUtils.HashTokenHmacSha256(newToken, GetSecretKey());

            // revoke old token and link to new
            existingToken.RevokedAt = DateTime.UtcNow;
            existingToken.ReplacedByTokenHash = newHash;
            _dbContext.RefreshTokens.Update(existingToken);

            var newEntity = new RefreshToken
            {
                UserId = existingToken.UserId,
                TokenHash = newHash,
                ExpiresAt = newExpires,
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = createdByIp,
                UserAgent = userAgent
            };

            await _dbContext.RefreshTokens.AddAsync(newEntity);
            await _dbContext.SaveChangesAsync();

            return (newToken, newExpires);
        }

        public async Task RevokeAllForUserAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var tokens = await _dbContext.RefreshTokens
                .Where(r => r.UserId == userId && r.RevokedAt == null && r.ExpiresAt > now)
                .ToListAsync();

            if (tokens.Count == 0) return;

            foreach (var t in tokens)
            {
                t.RevokedAt = now;
            }

            _dbContext.RefreshTokens.UpdateRange(tokens);
            await _dbContext.SaveChangesAsync();
        }
    }
}
