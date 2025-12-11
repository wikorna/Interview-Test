using Interview_Test.Infrastructure.DTOs;
using Interview_Test.Models;
using Interview_Test.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Interview_Test.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // ✅ ใช้ repository + mapping → ไม่ใช้ Data.Users แล้ว
    [HttpGet("GetUserById/{id}")]
    public async Task<ActionResult<UserDetailDto>> GetUserById(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("userId is required.");
        }

        // ตอนนี้ user คือ UserDetailDto? ไม่ใช่ UserModel
        var user = await _userRepository.GetByUserId(id, cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        // ไม่ต้อง ToUserDetailDto แล้ว
        return Ok(user);
    }

    [HttpPost("CreateUser")]
    public async Task<ActionResult<UserDetailDto>> CreateUser([FromBody] UserModel request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        // 1) สร้าง User ใหม่ (อย่าใช้ request.Id ให้ DB gen เอง)
        var user = new UserModel
        {
            UserId = request.UserId,
            Username = request.Username,
            UserProfile = new UserProfileModel
            {
                // ไม่ set ProfileId ถ้าเป็น identity
                FirstName = request.UserProfile.FirstName,
                LastName = request.UserProfile.LastName,
                Age = request.UserProfile.Age
            },
            UserRoleMappings = new List<UserRoleMappingModel>()
        };

        if (request.UserRoleMappings is not null)
        {
            foreach (var map in request.UserRoleMappings)
            {
                var roleReq = map.Role;
                if (roleReq is null)
                    continue;

                // ✅ สร้าง Role ใหม่จาก JSON แต่ "ไม่ใช้ RoleId จาก request"
                var role = new RoleModel
                {
                    // RoleId = 0; // ปล่อย default ให้เป็น 0 หรือไม่ set เลย
                    RoleName = roleReq.RoleName,
                    Permissions = new List<PermissionModel>()
                };

                // ✅ สร้าง Permission ใหม่จาก JSON — ห้ามใช้ PermissionId จาก request
                if (roleReq.Permissions is not null)
                {
                    foreach (var permReq in roleReq.Permissions)
                    {
                        var perm = new PermissionModel
                        {
                            // PermissionId = 0; // identity ให้ DB gen
                            Permission = permReq.Permission
                            // RoleId ไม่ต้อง set EF จะจัดการจาก nav Role
                        };

                        role.Permissions.Add(perm);
                    }
                }

                // ✅ Mapping เชื่อม User ↔ Role
                var mapping = new UserRoleMappingModel
                {
                    User = user,
                    Role = role
                    // UserId/RoleId ให้ EF fill จาก nav
                };

                user.UserRoleMappings.Add(mapping);
            }
        }

        await _userRepository.CreateUser(user, cancellationToken);

        var created = await _userRepository.GetByUserId(user.UserId, cancellationToken);
        if (created is null)
            return StatusCode(500, "User was created but could not be loaded.");

        return CreatedAtAction(nameof(GetUserById),
            new { id = created.UserId },
            created);
    }
}
