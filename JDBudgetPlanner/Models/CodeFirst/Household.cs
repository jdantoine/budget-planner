using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JDBudgetPlanner.Models
{
    public class Household
    {
        public int Id { get; set; }

        public Household()
        {
            this.Users = new HashSet<ApplicationUser>();
            this.Accounts = new HashSet<Account>();
        }

        public string Name { get; set; }
        public DateTimeOffset Create { get; set; }
        public int? BudgetId { get; set; }

        public virtual Budget Budget { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

    }
}