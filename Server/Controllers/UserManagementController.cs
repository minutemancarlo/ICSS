using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using ICSS.Server.Logger;
using ICSS.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ICSS.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]    
    public class UserManagementController : ControllerBase
    {
        private readonly IManagementApiClient _managementApiClient;

        public UserManagementController(IManagementApiClient managementApiClient)
        {
            _managementApiClient = managementApiClient;            
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
                        Role = role 
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


    }
}
