using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseHelperBasicVersion.Models.Database
{
    public class EFReviewDatabase : IReviewDatabase
    {
        private CourseHelperDBContext dbContext;
        public EFReviewDatabase(CourseHelperDBContext context)
        {
            dbContext = context;
        }

        public IQueryable<Review> Reviews
        {
            get { return dbContext.Reviews; }
        }
        
        public void SavaReview(Review review)
        {
            if (review.ReviewId == 0)
            {
                dbContext.Reviews.Add(review);
            }
            else
            {
                Review dbReview = dbContext.Reviews.FirstOrDefault(r => r.ReviewId == review.ReviewId);
                if (dbReview != null)
                {
                    dbReview.CourseCode = review.CourseCode;
                    dbReview.CourseName = review.CourseName;
                    dbReview.Semester = review.Semester;
                    dbReview.Comment = review.Comment;
                    dbReview.CreatorName = review.CreatorName;
                    dbReview.CreateTime = review.CreateTime;
                }
            }
            dbContext.SaveChanges();
        }
        public void DeleteReview(int reviewId)
        {
            Review dbReview = dbContext.Reviews.FirstOrDefault(r => r.ReviewId == reviewId);
            if(dbReview != null)
            {
                dbContext.Reviews.Remove(dbReview);
                dbContext.SaveChanges();
            }
        }
    }
}
