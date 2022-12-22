using System;
using System.Collections.Generic;

namespace TODOService.Models
{
    public partial class User
    {
        public User()
        {
            Tasks = new HashSet<Task>();
        }

        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Active { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }
    }
}
