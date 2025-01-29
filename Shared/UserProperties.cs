using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSS.Shared
{
    public class UserProperties
    {
        public string User_Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool Blocked { get; set; }
        public bool? Verified { get; set; }
        public DateTime? Last_Login { get; set; }
        public string Provider { get; set; }
        public string? Picture { get; set; }
        public bool isBinded { get; set; } = false;
        public string? Role { get; set; }
    }

    public partial class RolesProperty
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    public class SetUserRole
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }

}
