using System.Data.Entity;

namespace DAL_TestSystem
{
    public class Context : DbContext
    {
        public Context(string conStr)
           : base(conStr)
        { }
        static Context()
        {
            Database.SetInitializer<Context>(new MyContextInitializer());
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestGroup> TestGroups { get; set; }
        public DbSet<Result> Results { get; set; }
    }
}

