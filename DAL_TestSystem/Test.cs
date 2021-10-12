using System.Collections.Generic;

namespace DAL_TestSystem
{
    public class Test
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string TestName { get; set; }
        public int QuestionCount { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public Test()
        {
            Questions = new List<Question>();
        }
    }
}

