using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JDBudgetPlanner.Models
{
    public class BudgetItem
    {
        public int Id { get; set; }
        public int BudgetId { get; set; }
        public int? BudgetCategoryId { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Modified { get; set; }

        public decimal Amount { get; set; }
        public int Frequency { get; set; }

        public virtual Budget Budget { get; set; }
        public virtual BudgetCategory BudgetCategory { get; set; }

    }
}