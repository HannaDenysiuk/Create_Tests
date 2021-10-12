using System.Data.Entity;

namespace DAL_TestSystem
{
    class MyContextInitializer : DropCreateDatabaseIfModelChanges<Context>
    {
        protected override void Seed(Context context)
        {

            User u1 = new User() { FName = "Ivan", LName = "Rum", Login = "user2", Password = "00002", IsAdmin = false };
            User u2 = new User() { FName = "Oksana", LName = "Fisher",  Login = "sa", Password = "0001", IsAdmin = true };
            User u3 = new User() { FName = "Hanna", LName = "Fesenko", Login = "user3", Password = "00003", IsAdmin = false };
            context.Users.Add(u1);
            context.Users.Add(u2);
            context.Users.Add(u3);

            context.SaveChanges();

        }
    }
}

