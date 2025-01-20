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

                List<UserProperties> userslist = users.Select(x => new UserProperties
                {
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Blocked = x.Blocked ?? false,
                    User_Id = x.UserId,
                    Verified = x.EmailVerified,
                    Last_Login = x.LastLogin,
                    Provider = x.UserId.ToLower().Contains("auth0") ? "Email-Password Auth" :
                               x.UserId.ToLower().Contains("facebook") ? "Facebook" :
                               x.UserId.ToLower().Contains("google-oauth2") ? "Google" :
                               "Unknown Provider",
                    Picture = x.Picture
                }).ToList();
                
                return Ok(userslist);
            }
            catch (Exception ex)
            {                
                return BadRequest($"Exception Occured: {ex.Message}");
            }

        }

    }
}
