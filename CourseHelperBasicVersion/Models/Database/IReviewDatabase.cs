using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseHelperBasicVersion.Models.Database
{
    public interface IReviewDatabase
    {
        IQueryable<Review> Reviews { get; }
        void SavaReview(Review review);
        void DeleteReview(int reviewId);

    }
}
