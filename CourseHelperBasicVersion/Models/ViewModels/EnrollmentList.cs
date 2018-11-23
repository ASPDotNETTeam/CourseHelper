using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseHelperBasicVersion.Models.ViewModels
{
    public class EnrollmentList
    {
        public int CourseID { get; set; }
        public IList<long> EnrollQueue { get; set; }
    }
}
