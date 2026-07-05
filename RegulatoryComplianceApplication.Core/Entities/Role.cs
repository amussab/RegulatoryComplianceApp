using System;
using System.Collections.Generic;
using System.Text;

namespace RegulatoryComplianceApplication.Core.Entities
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
