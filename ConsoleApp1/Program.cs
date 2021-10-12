using DAL_TestSystem;
using Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //GenericUnitOfWork work;
            //IGenericRepository<User> repoUser;
            //work = new GenericUnitOfWork(new Context(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
            //repoUser = work.Repository<User>();

            using (Context conte = new Context(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString))
            {
                Group gr = new Group() { GroupName = "22222" };
                conte.Groups.Add(gr);
                conte.SaveChanges();
            }
        }
    }
}
