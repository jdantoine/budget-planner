using System.ComponentModel.DataAnnotations;

namespace JDBudgetPlanner.Models
{
    public enum TransactionType
    {
        [Display(Name = "Expense")]
        Expense,
        [Display(Name = "Income")]
        Income
    }
}