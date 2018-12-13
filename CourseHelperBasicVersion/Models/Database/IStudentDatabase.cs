using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseHelperBasicVersion.Models.Database
{
    public interface IStudentDatabase
    {
        IQueryable<Student> Students { get; }
        void SaveStudent(Student student);
        Student DeleteStudent(int id);
        void DeleteStudent(Student student);
    }

}
