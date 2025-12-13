using Interview_Test.Infrastructure;
using Interview_Test.Infrastructure.DTOs;
using Interview_Test.Infrastructure.Interfaces;
using Interview_Test.Infrastructure.Repositories;
using Interview_Test.Models;
using Interview_Test.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Interview_Test.Repositories;

public class UserRepository : BaseRepository<Models.UserModel>, IUserRepository
{
    public UserRepository(InterviewTestDbContext dbContext, ILogger<BaseRepository<Models.UserModel>> logger)
        : base(dbContext, logger)
    {
    }

    public async Task<UserDetailDto?> GetByUserId(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _dbContext.UserTb
                .Include(u => u.UserProfile)
                .Include(u => u.UserRoleMappings)
                    .ThenInclude(urm => urm.Role)
                        .ThenInclude(r => r.Permissions)
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.UserId == userId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User with UserId {UserId} not found.", userId);
                return null;
            }

            _logger.LogInformation("Retrieved user with UserId {UserId} from database.", userId);

            var profile = user.UserProfile;
            var roles = user.UserRoleMappings
                .Where(urm => urm.Role != null)
                .Select(urm => urm.Role!)
                .GroupBy(r => r.RoleId)
                .Select(g => g.First())
                .OrderBy(r => r.RoleId)
                .Select(r => new UserRoleDto
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                })
                .ToList();

            var permissions = user.UserRoleMappings
                .Where(urm => urm.Role != null)
                .SelectMany(urm => urm.Role!.Permissions)
                .Select(p => p.Permission)
                .Distinct()
                .OrderBy(p => p)
                .ToList();

            var dto = new UserDetailDto
            {
                Id = user.Id,
                UserId = user.UserId,
                Username = user.Username,
                FirstName = profile?.FirstName ?? string.Empty,
                LastName = profile?.LastName ?? string.Empty,
                Age = profile?.Age,
                Roles = roles,
                Permissions = permissions
            };

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching user with UserId {UserId}.", userId);
            throw;
        }
    }
    public async Task<int> CreateUser(UserModel _ /*ignored*/, CancellationToken token = default)
    {
        var seedUsers = Data.Users;
        
        var seedUserIds = seedUsers.Select(u => u.UserId).ToList();
        var existing = await _dbContext.UserTb
            .Where(u => seedUserIds.Contains(u.UserId))
            .Select(u => u.UserId)
            .ToListAsync(token);

        var toInsert = seedUsers.Where(u => !existing.Contains(u.UserId)).ToList();
        if (toInsert.Count == 0) return 0;

        foreach (var src in toInsert)
        {
            var roleIds = src.UserRoleMappings
                .Where(m => m.Role != null)
                .Select(m => m.Role!.RoleId)
                .Where(rid => rid > 0)
                .Distinct()
                .ToList();

            if (roleIds.Count == 0)
                throw new InvalidOperationException($"Seed user '{src.UserId}' has no valid roleId.");

            var dbRoleIds = await _dbContext.RoleTb
                .Where(r => roleIds.Contains(r.RoleId))
                .Select(r => r.RoleId)
                .ToListAsync(token);

            var missing = roleIds.Except(dbRoleIds).ToList();
            if (missing.Count > 0)
                throw new InvalidOperationException($"Seed user '{src.UserId}' has missing RoleId(s): {string.Join(",", missing)}");

            var user = new UserModel
            {
                Id = src.Id,
                UserId = src.UserId,
                Username = src.Username,
                UserProfile = new UserProfileModel
                {
                    FirstName = src.UserProfile.FirstName,
                    LastName = src.UserProfile.LastName,
                    Age = src.UserProfile.Age,
                    User = null 
                },
                UserRoleMappings = roleIds.Select(rid => new UserRoleMappingModel
                {
                    UserRoleMappingId = Guid.NewGuid(),
                    RoleId = rid,
                    User = null,
                    Role = null
                }).ToList()
            };

            _dbContext.UserTb.Add(user);
        }

        return await _dbContext.SaveChangesAsync(token);
    }
}
