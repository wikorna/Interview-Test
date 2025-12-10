using Interview_Test.Infrastructure.DTOs;
using Interview_Test.Infrastructure.Interfaces;
using Interview_Test.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview_Test.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<Models.UserModel>, IUserRepository
    {
        public UserRepository(InterviewTestDbContext dbContext,ILogger<BaseRepository<Models.UserModel>> logger)
            : base(dbContext, logger)
        {
        }
/*        
 *      public dynamic GetUserById(string id)
        {
            //Todo: Implement this method
            throw new NotImplementedException();
        }*/

        public async Task<UserDetailDto?> GetByUserId(string userId,CancellationToken cancellationToken = default)
        {
            try
            {
                // ดึง User + Profile + Roles + Permissions
                var user = await _dbContext.UserTb
                    .Include(u => u.UserProfile)                // สมมติ nav property ชื่อ UserProfile
                    .Include(u => u.UserRoleMappings)           // ICollection<UserRoleMappingModel>
                        .ThenInclude(urm => urm.Role)           // nav Role
                            .ThenInclude(r => r.Permissions)    // ICollection<PermissionModel>
                    .AsNoTracking()
                    .SingleOrDefaultAsync(u => u.UserId == userId, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("User with UserId {UserId} not found.", userId);
                    return null;
                }

                _logger.LogInformation("Retrieved user with UserId {UserId} from database.", userId);

                var profile = user.UserProfile;

                // ดึง roles (ไม่ซ้ำ)
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

                // ดึง permissions flatten เป็น list<string>
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


        public int CreateUser(UserModel user)
        {
            //Todo: Implement this method
            throw new NotImplementedException();
        }


    }
}            