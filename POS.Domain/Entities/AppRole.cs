using Microsoft.AspNetCore.Identity;
using POS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Entities
{
    public class AppRole : IdentityRole
    {
        public RoleGroup? Group { get; set; }
    }
}
