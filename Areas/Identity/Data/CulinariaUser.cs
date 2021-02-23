using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Culinaria.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the CulinariaUser class
    public class CulinariaUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
