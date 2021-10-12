using System;

namespace DAL_TestSystem
{
    public class Result
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int? TestId { get; set; }
        public Test Test { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public double Rate { get; set; }
    }
}

