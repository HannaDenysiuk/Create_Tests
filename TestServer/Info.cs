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
        public double Mark { get; set; }

        public List<string> ListOfTests { get; set; }
        public int IdTest { get; set; }
        public byte[] Buffer { get; set; }
        // public Xml2CSharp.Test Test { get; set; }
        // public List<DAL_TestSystem.Answer> Answers { get; set; }
        public bool? IsRegistered { get; set; }
        public string Password { get; set; }
        public string Login { get; set; }
        public int? UserId { get; set; }
        //public DAL_TestSystem.User User { get; set; }
        public string Msg { get; set; }
    }
}
