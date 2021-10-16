using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    [Serializable]
    public class Info
    {
        public object ListOfTests { get; set; }
        public int IdTest { get; set; }
        public DAL_TestSystem.Test Test { get; set; }
        public DAL_TestSystem.Answer Answers { get; set; }
        public bool? IsRegistered { get; set; }
        public DAL_TestSystem.User User { get; set; }
        public string Msg { get; set; }
    }
}
