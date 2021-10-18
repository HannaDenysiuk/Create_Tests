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
        public double? Mark { get; set; }

        public List<string> ListOfGroups { get; set; }
        public string Group { get; set; }
        public int? IdTest { get; set; }
        public Dictionary<int,string> Questions { get; set; }//Questions for pass//  int - Id
        public Dictionary<int, List<string>> Answers { get; set; }//answers for pass// int - IdQuestion
        public List<string> UserAnswers { get; set; }
        public byte[] Buffer { get; set; }//DataSet table
        public bool? IsRegistered { get; set; }
        public string Password { get; set; }
        public string Login { get; set; }
        public string Lname { get; set; }
        public string Fname { get; set; }
        public int? UserId { get; set; }
        public string Msg { get; set; }
    }
}
