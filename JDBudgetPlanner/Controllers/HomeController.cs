using JDBudgetPlanner.Helpers;
using JDBudgetPlanner.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JDBudgetPlanner.Controllers
{
    [RequireHttps]
    [AuthorizeHouseholdRequired]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var householdId = User.Identity.GetHouseholdId();
            var model = db.Households.Find(householdId);
            return View(model);
        }

        public ActionResult _TransactionsByMonth(int month, int year)
        {
            try
            {
                var householdId = User.Identity.GetHouseholdId();
                var household = db.Households.Find(householdId);
                var previousMonth = (month == 1) ? new DateTimeOffset(year - 1, 12, 1, 0, 0, 0, new TimeSpan(-4, 0, 0)) : new DateTimeOffset(year, month - 1, 1, 0, 0, 0, new TimeSpan(-4, 0, 0));
                var nextMonth = (month == 12) ? new DateTimeOffset(year + 1, 1, 1, 0, 0, 0, new TimeSpan(-4, 0, 0)) : new DateTimeOffset(year, month + 1, 1, 0, 0, 0, new TimeSpan(-4, 0, 0));
                var transactions = household.Accounts.SelectMany(m => m.Transactions).Where(t => t.Created.CompareTo(nextMonth) < 0 && t.Created.CompareTo(previousMonth) > 0 && t.TransactionType == TransactionType.Expense).ToList();
                if(transactions.Count() == 0)
                {
                    return PartialView("_NoData");
                }
                var categories = household.Budget.BudgetItems.Select(b => b.BudgetCategory).Distinct().ToList();
                var data = (from category in categories
                            let sum = (from item in transactions
                                       where item.BudgetCategoryId == category.Id
                                       select item.Amount).Sum()
                            select new
                            {
                                y = sum,
                                indexLabel = category.Name,
                            }).ToArray();
                return Content(JsonConvert.SerializeObject(data), "application/json");
            }
            catch
            {
                return PartialView("_Error");
            }

        }

        public ActionResult _BudgetVsTransactions(int month, int year)
        {
            try
            {
                var householdId = User.Identity.GetHouseholdId();
                var household = db.Households.Find(householdId);
                var previousMonth = (month == 1) ? new DateTimeOffset(year - 1, 12, 1, 0, 0, 0, new TimeSpan(-4, 0, 0)) : new DateTimeOffset(year, month - 1, 1, 0, 0, 0, new TimeSpan(-4, 0, 0));
                var nextMonth = (month == 12) ? new DateTimeOffset(year + 1, 1, 1, 0, 0, 0, new TimeSpan(-4, 0, 0)) : new DateTimeOffset(year, month + 1, 1, 0, 0, 0, new TimeSpan(-4, 0, 0));
                var transactions = household.Accounts.SelectMany(m => m.Transactions).Where(t => t.Created.CompareTo(nextMonth) < 0 && t.Created.CompareTo(previousMonth) > 0 && t.TransactionType == TransactionType.Expense).ToList();
                if (transactions.Count() == 0)
                {
                    return PartialView("_NoData");
                }
                var categories = household.Budget.BudgetItems.Select(b => b.BudgetCategory).Distinct().ToList();
                var transactionData = (from category in categories
                            let sum = (from item in transactions
                                       where item.BudgetCategoryId == category.Id
                                       select item.Amount).Sum()
                            select new
                            {
                                y = sum,
                                label = category.Name,
                            }).ToArray();
                var budgetItems = household.Budget.BudgetItems;
                var budgetData = (from item in budgetItems
                                       select new
                                       {
                                           y = item.Amount * item.Frequency / 12,
                                           label = item.BudgetCategory.Name,
                                       }).ToArray();
                return Content(JsonConvert.SerializeObject(new { transactionData, budgetData }), "application/json");
            }
            catch
            {
                return PartialView("_Error");
            }

        }

        public ActionResult Unauthorized()
        {
            return View();
        }

    }
}