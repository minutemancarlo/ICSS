using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using ICSS.Server.Logger;
using ICSS.Server.Repository;
using ICSS.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace ICSS.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]    
    public class UserManagementController : ControllerBase
    {
        private readonly IManagementApiClient _managementApiClient;
        private readonly UserRepository _userRepository;

        public UserManagementController(IManagementApiClient managementApiClient,UserRepository userRepository)
        {
            _managementApiClient = managementApiClient;
            _userRepository = userRepository;
        }

        [HttpGet("UsersList")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserProperties>>> UsersList()
        {
            try
            {
                var users = await _managementApiClient.Users.GetAllAsync(new GetUsersRequest(), new PaginationInfo());
                
                List<UserProperties> userslist = new List<UserProperties>();

                foreach (var user in users)
                {
                    var roles = await _managementApiClient.Users.GetRolesAsync(user.UserId, new PaginationInfo());
                    var role = roles.FirstOrDefault()?.Name;
                    var department = await _userRepository.CheckUserDepartment(user.UserId);
                    userslist.Add(new UserProperties
                    {
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Blocked = user.Blocked ?? false,
                        User_Id = user.UserId,
                        Verified = user.EmailVerified,
                        Last_Login = user.LastLogin,
                        Provider = user.UserId.ToLower().Contains("auth0") ? "Email-Password Auth" :
                                   user.UserId.ToLower().Contains("facebook") ? "Facebook" :
                                   user.UserId.ToLower().Contains("windows") ? "Microsoft" :
                                   user.UserId.ToLower().Contains("google-oauth2") ? "Google" :
                                   "Unknown Provider",
                        Picture = user.Picture,
                        Role = role,
                        Departments = department
                    });
                }

                return Ok(userslist);
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception Occurred: {ex.Message}");
            }
        }

        [HttpGet("GetAllRoles")]
        public async Task<ActionResult<List<RolesProperty>>> GetAllRoles()
        {
            try
            {
                var roles = await _managementApiClient.Roles.GetAllAsync(new GetRolesRequest());
                List<RolesProperty> rolesList = roles.Select(x => new RolesProperty
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToList();
                
                return Ok(rolesList);

            }
            catch (Exception ex)
            {                
                return BadRequest($"Exception Occured: {ex.Message}");
            }
        }

        [HttpPatch("update-block-status/{userId}")]
        public async Task<IActionResult> UpdateBlockStatus(string userId, [FromQuery] bool block)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required.");

            try
            {
                var updateRequest = new UserUpdateRequest
                {
                    Blocked = block
                };

                await _managementApiClient.Users.UpdateAsync(userId, updateRequest);

                var status = block ? "blocked" : "unblocked";
                return Ok(new { Message = $"User with ID '{userId}' has been successfully {status}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the user's block status.", Error = ex.Message });
            }
        }

        [HttpPost("check-and-insert")]
        public async Task<IActionResult> CheckAndInsertSystemId([FromBody] InternalUser model)
        {
            if (model == null || string.IsNullOrEmpty(model.SystemId))
                return BadRequest("SystemId is required.");

            await _userRepository.CheckAndInsertSystemIdAsync(model.SystemId, model.Name ?? string.Empty, model.Email ?? string.Empty);
            return Ok(new { Message = "Operation completed successfully." });
        }

        [HttpGet("active-users-count")]
        public async Task<ActionResult<int>> GetActiveUsersCount()
        {
            try
            {              
                var activeUsersCount = await _managementApiClient.Stats.GetActiveUsersAsync();             
                return Ok(activeUsersCount);
            }
            catch (Exception ex)
            {                
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("SetUserRole")]
        public async Task<ActionResult<string>> SetUserRole([FromBody] SetUserRole user)
        {
            try
            {
                var currentUserRole = await GetUserRole(user.UserId);
                if (!string.IsNullOrEmpty(currentUserRole))
                {
                    var oldData = new AssignRolesRequest
                    {
                        Roles = new string[] { currentUserRole }
                    };

                    await _managementApiClient.Users.RemoveRolesAsync(user.UserId, oldData);
                }
                var currentData = new AssignRolesRequest
                {
                    Roles = new string[] { user.RoleId }
                };
                   await _managementApiClient.Users.AssignRolesAsync(user.UserId, currentData);

               _ = await _userRepository.CheckAndInsertSystemIdAsync(user.UserId, user.Name, user.Email);
                

               _ = await _userRepository.UpdateAdminDepartmentAsync(user.DepartmentId, user.UserId);
                return Ok(new { Message = "Operation completed successfully." });
            }
            catch (Exception ex)
            {                                
                return BadRequest($"Exception Occured: {ex.Message}");
            }
        }


        [HttpGet("GetUserById/{Id}")]        
        public async Task<ActionResult<InternalUser>> GetUserById(string Id)
        {
            try
            {
                var users = await _managementApiClient.Users.GetAsync(Id);
                var usersList = new InternalUser
                {
                    SystemId = users.UserId,
                    Name = string.IsNullOrWhiteSpace(users.FullName) ? users.NickName : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(users.FullName.ToLower()),
                    Email = users.Email,
                };            

                return Ok(usersList);
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception Occurred: {ex.Message}");
            }
        }


        [HttpGet("GetUserDepartment/{Id}")]
        public async Task<ActionResult<Departments>> GetUserDepartment(string Id)
        {
            try
            {
                var department = await _userRepository.CheckUserDepartment(Id);

                if (department is null)
                {
                    return NoContent();
                }

                return Ok(department);
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception Occurred: {ex.Message}");
            }
        }





        private async Task<string> GetUserRole(string userid)
        {
            var result = await _managementApiClient.Users.GetRolesAsync(userid);
            var userRole = result.FirstOrDefault()?.Id;
            return userRole;
        }


    }
}
