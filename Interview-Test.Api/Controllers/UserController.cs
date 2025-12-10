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

    /*[HttpGet("GetUserById/{id}")]
    public ActionResult GetUserById(string id)
    {
        //Todo: Implement this method
        return Ok(Data.Users);
    }*/

    [HttpGet("GetUserById/{id}")]
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
    }
    [HttpPost("CreateUser")]
    public ActionResult CreateUser(UserModel user)
    {
        //Todo: Implement this method
        return Ok();
    }
}