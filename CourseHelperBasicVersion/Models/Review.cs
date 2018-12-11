using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseHelperBasicVersion.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public string CourseName { get; set; }

        [Required(ErrorMessage = "Please enter a Course code")]
        public string CourseCode { get; set; }

        [Required(ErrorMessage = "Please enter your semester")]
        public string Semester { get; set; }

        [Required(ErrorMessage = "Please enter your comment")]
        public string Comment { get; set; }

        public string CreatorName { get; set; }
        public DateTime CreateTime { get; set; }
        
    }
}
