using System;
using System.Collections;
using System.Collections.Generic;

namespace JDBudgetPlanner.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public decimal Amount { get; set; }
        public int? AccountId { get; set; }
        public int? BudgetCategoryId { get; set; }
        public bool IsReconciled { get; set; }
        public string Description { get; set; }
        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }
        public virtual Account Account { get; set; }
        public virtual TransactionType TransactionType { get; set; }
        public virtual BudgetCategory BudgetCategory { get; set; }
    }
}