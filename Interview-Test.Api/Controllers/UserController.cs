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

    [HttpGet("GetUserById/{id}")]
    public async Task<ActionResult<UserDetailDto>> GetUserById(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("userId is required.");
        }

        var user = await _userRepository.GetByUserId(id, cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost("CreateUser")]
    public async Task<ActionResult<UserDetailDto>> CreateUser([FromBody] UserModel request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);
        
        if (request.UserRoleMappings is null || request.UserRoleMappings.Count == 0)
            return BadRequest("At least one role is required.");
        
        var user = new UserModel
        {
            Id = request.Id,
            UserId = request.UserId,
            Username = request.Username,
            UserProfile = new UserProfileModel
            {
                //ProfileId = request.UserProfile.ProfileId, //"00000000-0000-0000-0000-000000000000"
                FirstName = request.UserProfile.FirstName,
                LastName = request.UserProfile.LastName,
                Age = request.UserProfile.Age
            },
            UserRoleMappings = new List<UserRoleMappingModel>()
        };

        /*        if (request.UserRoleMappings is not null)
                {
                    foreach (var map in request.UserRoleMappings)
                    {
                        var roleReq = map.Role;
                        if (roleReq is null)
                            continue;
                        var role = new RoleModel
                        {
                            //RoleId= roleReq.RoleId,
                            RoleName = roleReq.RoleName,
                            Permissions = new List<PermissionModel>()
                        };
                        if (roleReq.Permissions is not null)
                        {
                            foreach (var permReq in roleReq.Permissions)
                            {
                                role.Permissions.Add(new PermissionModel
                                {
                                    Permission = permReq.Permission
                                });
                            }
                        }
                        user.UserRoleMappings.Add(new UserRoleMappingModel
                        {
                            //UserRoleMappingId = map.UserRoleMappingId,
                            //UserId = user.Id,
                            //RoleId = role.RoleId,
                            User = user,
                            Role = role
                        });
                    }
                }*/


        foreach (var map in request.UserRoleMappings ?? Enumerable.Empty<UserRoleMappingModel>())
        {
            user.UserRoleMappings.Add(new UserRoleMappingModel
            {
                User = user,
                RoleId = map.RoleId,
                UserId = user.Id
            });
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
