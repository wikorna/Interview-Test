using Interview_Test.Infrastructure.DTOs;
using Interview_Test.Models;
using Interview_Test.Infrastructure.Interfaces;
using Interview_Test.Repositories;
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
    public ActionResult GetUserById(string id)
    {
        //Todo: Implement this method
        return Ok(Data.Users);
    }

    /*[HttpGet("GetUserById/{id}")]
    public async Task<ActionResult<UserDetailDto>> GetUserById(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("userId is required.");
        }

        var result = await _userRepository.GetByUserId(id, cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }*/
    
    [HttpPost("CreateUser")]
    public ActionResult CreateUser([FromBody] UserModel user)
    {
        // ตรงนี้ ModelState อาจจะ Invalid แต่จะไม่โดนยิง 400 อัตโนมัติแล้ว
        // ถ้าอยากดู error ก็ทำได้
        if (!ModelState.IsValid)
        {
            // คุณจะเลือก ignore error เฉพาะ navigation ก็ได้
            // หรือ log แล้วไปต่อ
            // แต่ในเคสนี้เรา ignore error ของ UserProfile.User / UserRoleMappings.User / Permission.Role ไปเลย
        }

        // จากนั้นผูก navigation ให้ครบ
        if (user.Id == Guid.Empty)
            user.Id = Guid.NewGuid();

        if (user.UserProfile != null)
            user.UserProfile.User = user;

        if (user.UserRoleMappings != null)
        {
            foreach (var map in user.UserRoleMappings)
            {
                map.User = user;
                map.UserId = user.Id;

                if (map.Role != null)
                {
                    foreach (var perm in map.Role.Permissions)
                    {
                        perm.Role = map.Role;
                        perm.RoleId = map.Role.RoleId;
                    }
                }
            }
        }

        _userRepository.CreateUser(user);

        return Ok(user);
    }

}