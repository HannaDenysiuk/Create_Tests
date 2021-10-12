using System;
using System.ComponentModel.DataAnnotations;

namespace DAL_TestSystem
{
    public class TestGroup
    {
        public int Id { get; set; }
        public int? TestId { get; set; }
        public Test Test { get; set; }
        public int? GroupId { get; set; }
        public Group Group { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }
}

