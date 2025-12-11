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
            if (user == null) throw new ArgumentNullException(nameof(user));

            // Map จาก user (Data.cs) → entity ใหม่ เพื่อเลี่ยง graph ซับซ้อน/dup
            var newUser = new UserModel
            {
                UserId = user.UserId,
                Username = user.Username,
                UserProfile = user.UserProfile == null
                    ? null
                    : new UserProfileModel
                    {
                        FirstName = user.UserProfile.FirstName,
                        LastName = user.UserProfile.LastName,
                        Age = user.UserProfile.Age
                    },
                UserRoleMappings = new List<UserRoleMappingModel>()
            };

            if (user.UserRoleMappings != null)
            {
                foreach (var mapping in user.UserRoleMappings)
                {
                    if (mapping.Role == null) continue;

                    var newRole = new RoleModel
                    {
                        RoleName = mapping.Role.RoleName,
                        Permissions = mapping.Role.Permissions?
                            .Select(p => new PermissionModel
                            {
                                Permission = p.Permission
                            })
                            .ToList() ?? new List<PermissionModel>()
                    };

                    var newMapping = new UserRoleMappingModel
                    {
                        User = newUser,
                        Role = newRole
                    };

                    newUser.UserRoleMappings.Add(newMapping);
                }
            }

            _dbContext.UserTb.Add(newUser);
            // SaveChanges จะเซฟ User + Profile + Roles + Permissions + Mapping
            var affectedRows = _dbContext.SaveChanges();
            return affectedRows;
        }

    }
}            