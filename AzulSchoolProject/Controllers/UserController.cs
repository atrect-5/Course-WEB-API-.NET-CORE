using Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace AzulSchoolProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _userService.GetUserById(id);
            if (user is null)
                return NotFound(); // Return 404 Not Found if the user does not exist
            
            return Ok(user); // Return 200 OK with the user data
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(users); // Return 200 OK with the list of users
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var newUser = _userService.Add(createUserDto);

            // Return status 201 Created.
            // This includes the URI of the newly created resource in the Location header
            // Also includes the created user in the response body
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] CreateUserDto createUserDto)
        {
            var updatedUser = _userService.Update(id, createUserDto);
            if (updatedUser is null)
                return NotFound();

            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id) 
        {
            bool isDeleted = _userService.Delete(id);
            if (!isDeleted)
                return NotFound();
            return NoContent(); // 204 No Content
        }
    }
}
