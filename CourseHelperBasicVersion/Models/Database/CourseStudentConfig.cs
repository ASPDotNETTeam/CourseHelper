using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseHelperBasicVersion.Models
{
    public class CourseStudentConfig : IEntityTypeConfiguration<CourseStudent>
    {
        public void Configure(EntityTypeBuilder<CourseStudent> builder)
        {
            builder.ToTable("CourseStudent");
            builder.HasKey(cs => new { cs.CourseId, cs.StudentId });
            builder.HasOne(cs => cs.Course)
                    .WithMany(cs => cs.CourseStudents)
                    .HasForeignKey(cs => cs.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(cs => cs.Student)
                .WithMany(s => s.CourseStudents)
                .HasForeignKey(sc => sc.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
