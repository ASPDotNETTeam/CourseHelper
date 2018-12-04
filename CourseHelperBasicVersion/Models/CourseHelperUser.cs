using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseHelperBasicVersion.Models
{
    public class CourseHelperUser : IdentityUser
    {
        public long StudentNumber { get;  set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
