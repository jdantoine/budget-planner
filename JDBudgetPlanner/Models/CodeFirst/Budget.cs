using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JDBudgetPlanner.Models
{
    public class Budget
    {
        public int Id { get; set; }

        public Budget()
        {
            this.BudgetItems = new HashSet<BudgetItem>();
        }

        public decimal Amount { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Modified { get; set; }

        public int HouseholdId { get; set; }

        [Required]
        public virtual Household Household { get; set; }
        public virtual ICollection<BudgetItem> BudgetItems { get; set; }
    }
}