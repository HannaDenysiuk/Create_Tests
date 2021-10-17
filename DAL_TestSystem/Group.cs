using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL_TestSystem
{
    public class Group
    {
        public int Id { get; set; }
        [Required, MaxLength(50), MinLength(2), Index(IsUnique = true)]
        public string GroupName { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public Group()
        {
            Users = new List<User>();
        }
    }
}

