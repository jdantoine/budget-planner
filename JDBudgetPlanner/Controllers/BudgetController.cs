using JDBudgetPlanner.Helpers;
using JDBudgetPlanner.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JDBudgetPlanner.Controllers
{
    [RequireHttps]
    [AuthorizeHouseholdRequired]
    public class BudgetController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Budget
        public ActionResult Index()
        {
            var householdId = User.Identity.GetHouseholdId();
            var household = db.Households.Find(householdId);
            var model = household.Budget;
            return View(model);
        }

        // GET: Add BudgetItem
        public ActionResult _AddBudgetItem(int? id)
        {
            try
            {
                return PartialView();
            }
            catch
            {
                return PartialView("_Error");
            }
        }

        //Helper function: Update account balance
        public bool UpdateBudgetAmount(bool AddAmount, decimal Amount, int Frequency, int? BudgetId)
        {
            var budget = db.Budgets.Find(BudgetId);
            budget.Amount = (AddAmount) ? budget.Amount + Amount * Frequency / 12 : budget.Amount - Amount * Frequency / 12;
            budget.Household = budget.Household;
            db.Entry(budget).State = EntityState.Modified;
            db.SaveChangesWithErrors();

            return true;
        }

        //POST: Add Budget Item
        [HttpPost]
        public ActionResult AddBudgetItem(int Frequency, decimal Amount, string CategoryName)
        {
            if (ModelState.IsValid)
            {
                var householdId = User.Identity.GetHouseholdId();
                var budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == householdId);
                var item = new BudgetItem();
                item.Created = DateTimeOffset.Now;
                item.Frequency = Frequency;
                item.Amount = Amount;
                db.BudgetItems.Add(item);
                budget.BudgetItems.Add(item);

                BudgetCategory category = new BudgetCategory();
                var budgetCategories = from i in budget.BudgetItems
                                       from c in db.BudgetCategories
                                       where i.BudgetCategoryId == c.Id
                                       select c;
                if (budgetCategories.Any(b => b.Name.Standardize() == CategoryName.Standardize()))
                {
                    TempData["Error"] = "Category already exists. Please enter a different category name.";
                    return RedirectToAction("Index");
                }
                else
                {
                    category.Name = CategoryName;
                }     
                category.BudgetItem = item;
                db.BudgetCategories.Add(category);
                db.SaveChanges();
                UpdateBudgetAmount(true, Amount, Frequency, budget.Id);
                item.BudgetCategoryId = category.Id;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        // GET: Edit BudgetItem
        public ActionResult _EditBudgetItem(int? id)
        {
            try
            {
                var model = db.BudgetItems.Find(id);
                return PartialView(model);
            }
            catch
            {
                return PartialView("_Error");
            }
        }

        //POST: Edit Budget Item
        [HttpPost]
        public ActionResult EditBudgetItem([Bind(Include = "Id, BudgetId, Created, Amount, Frequency, BudgetCategory.Id")] BudgetItem item, string CategoryName)
        {
            if (ModelState.IsValid)
            {
                var householdId = User.Identity.GetHouseholdId();
                var budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == householdId);
                var oldItem = db.BudgetItems.AsNoTracking().FirstOrDefault(m => m.Id == item.Id);
                item.Modified = DateTimeOffset.Now;
                
                budget.Amount -= oldItem.Amount * oldItem.Frequency / 12;
                budget.Amount += item.Amount * item.Frequency / 12;
                budget.Household = budget.Household;

                db.BudgetItems.Attach(item);
                db.Entry(item).Property("Amount").IsModified = true;
                db.Entry(item).Property("Frequency").IsModified = true;
                db.Entry(item).Property("Modified").IsModified = true;
                db.Budgets.Attach(budget);
                db.Entry(budget).Property("Amount").IsModified = true;
                db.SaveChanges();

                var category = db.BudgetCategories.Find(oldItem.BudgetCategory.Id);
                var oldCategoryName = oldItem.BudgetCategory.Name;
                if (oldCategoryName != CategoryName)
                {
                    var budgetCategories = from i in budget.BudgetItems
                                           from c in db.BudgetCategories
                                           where i.BudgetCategoryId == c.Id
                                           select c;
                    if (budgetCategories.Any(b => b.Name.Standardize() == CategoryName.Standardize()))
                    {
                        TempData["EditError"] = "That category already exists. Please enter a different category name.";
                        TempData["Id"] = item.Id;
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    category.Name = CategoryName;
                }           
                category.Name = CategoryName;
                db.BudgetCategories.Attach(category);
                db.Entry(category).Property("Name").IsModified = true;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Edit BudgetItem
        public ActionResult _DeleteBudgetItem(int? id)
        {
            try
            {
                var model = db.BudgetItems.Find(id);
                return PartialView(model);
            }
            catch
            {
                return PartialView("_Error");
            }
        }

        //POST: Transaction/DeleteTransaction
        [HttpPost]
        public ActionResult DeleteBudgetItem(int Id)
        {
            var category = db.BudgetCategories.Find(Id);
            var item = db.BudgetItems.Find(Id);
            UpdateBudgetAmount(false, item.Amount, item.Frequency, item.BudgetId);
            db.BudgetCategories.Remove(category);
            db.BudgetItems.Remove(item);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //POST: Edit Budget Amount
        [HttpPost]
        public ActionResult EditBudgetAmount()
        {
            return RedirectToAction("Index");
        }
    }
}