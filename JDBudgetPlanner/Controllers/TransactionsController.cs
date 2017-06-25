using JDBudgetPlanner.Helpers;
using JDBudgetPlanner.Models;
using Microsoft.AspNet.Identity;
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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transaction
        public ActionResult Index()
        {
            var householdId = User.Identity.GetHouseholdId();
            var model = db.Accounts.Where(a => a.HouseholdId == householdId).ToList();
            return View(model);
        }

        //POST: Transaction/AddAccount
        [HttpPost]
        public ActionResult AddAccount(string AccountName)
        {
            Account account = new Account();
            account.Name = AccountName;
            account.Balance = 0;
            account.ReconciledAmount = 0;
            account.Created = DateTimeOffset.Now;
            account.IsReconciled = false;
            db.Accounts.Add(account);
            db.SaveChanges();
            var householdId = User.Identity.GetHouseholdId();
            var household = db.Households.Find(householdId);
            household.Accounts.Add(account);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Transaction/Details/2
        public ActionResult Details(int? id)
        {
            var model = db.Accounts.Find(id);
            var householdId = User.Identity.GetHouseholdId();
            if (model.HouseholdId != householdId)
            {
                return RedirectToAction("Unauthorized", "Home");
            }
            try {
                return View(model);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Add Transaction
        public ActionResult _AddTransaction(int? id)
        {
            try {
                ViewBag.AccountId = id;
                var householdId = User.Identity.GetHouseholdId();
                var household = db.Households.Find(householdId);
                var categories = household.Budget.BudgetItems.Select(b => b.BudgetCategory).Distinct().ToList();
                ViewBag.BudgetCategories = categories;
                return PartialView();
            }
            catch
            {
                return PartialView("_Error");
            }
        }

        // GET: Add Transaction
        public ActionResult _ReconcileAccount(int? id)
        {
            try
            {
                ViewBag.AccountId = id;
                return PartialView();
            }
            catch
            {
                return PartialView("_Error");
            }
        }

        //Helper function: Update account balance
        public bool UpdateAccountBalance(bool IsIncome, bool IsReconciled, decimal Amount, int? AccountId)
        {
            var account = db.Accounts.Find(AccountId);
            account.Balance = (IsIncome) ? account.Balance + Amount : account.Balance - Amount;

            foreach (var transaction in account.Transactions)
            {
                if ((transaction.IsReconciled == false) && (IsIncome == false))
                {
                    account.ReconciledAmount = account.Balance - Amount;
                }
                else
                {
                    account.ReconciledAmount = account.Balance - Amount;
                }
            }

            if (IsReconciled)
            {
                account.ReconciledAmount = (IsIncome) ? account.ReconciledAmount + Amount : account.ReconciledAmount - Amount;
            }
            else
            {
                account.ReconciledAmount = account.ReconciledAmount;
            }           
            db.Accounts.Attach(account);
            db.Entry(account).Property("Balance").IsModified = true;
            db.Entry(account).Property("ReconciledAmount").IsModified = true;
            db.SaveChanges();

            return true;
        }

        public bool SetIsReconciled(bool IsReconciled, int? AccountId)
        {
            var account = db.Accounts.Find(AccountId);
            if (IsReconciled)
            {
                foreach (var transaction in account.Transactions)
                {
                    transaction.IsReconciled = IsReconciled;
                    db.Transactions.Attach(transaction);
                    db.Entry(transaction).Property("IsReconciled").IsModified = true;
                }
            }
            account.IsReconciled = IsReconciled;
            db.Accounts.Attach(account);
            db.Entry(account).Property("IsReconciled").IsModified = true;
            db.SaveChanges();

            return true;
        }

        //POST: Transaction/AddTransaction
        [HttpPost]
        public ActionResult AddTransaction([Bind(Include = "Amount, Description, TransactionType")] Transaction transaction, int? AccountId, int? BudgetCategoryId)
        {
            if (ModelState.IsValid)
            {
                transaction.Created = DateTimeOffset.Now;
                var userId = User.Identity.GetUserId();
                transaction.AuthorId = userId;
                if (transaction.TransactionType == TransactionType.Expense)
                {
                    transaction.BudgetCategoryId = BudgetCategoryId;
                    UpdateAccountBalance(false, false, transaction.Amount, AccountId);
                }
                else
                {
                    UpdateAccountBalance(true, false, transaction.Amount, AccountId);
                }
                db.Transactions.Add(transaction);
                db.SaveChanges();

                var theTransaction = db.Transactions.Find(transaction.Id);
                var account = db.Accounts.Find(AccountId);
                account.Transactions.Add(theTransaction);
                db.SaveChanges();

                SetIsReconciled(false, AccountId);
            }
            return RedirectToAction("Details", new { id = transaction.AccountId});
        }

        //POST: Transaction/ReconcileAccount
        [HttpPost]
        public ActionResult ReconcileAccount(decimal ActualBalance, int AccountId)
        {
            if (ModelState.IsValid)
            {
                var account = db.Accounts.Find(AccountId);
                if (ActualBalance > account.Balance)
                {
                    var balanceDifference = ActualBalance - account.Balance;
                    UpdateAccountBalance(true, true, balanceDifference, AccountId);
                }
                else
                {
                    var balanceDifference = account.Balance - ActualBalance;
                    UpdateAccountBalance(false, true, balanceDifference, AccountId);
                }
                SetIsReconciled(true, AccountId);
            }
            return RedirectToAction("Details", new { id = AccountId });
        }

        public ActionResult _EditTransaction(int? id)
        {
            try
            {
                var householdId = User.Identity.GetHouseholdId();
                var household = db.Households.Find(householdId);
                var categories = household.Budget.BudgetItems.Select(b => b.BudgetCategory).Distinct().ToList();
                ViewBag.BudgetCategories = categories;
                var transaction = db.Transactions.Find(id);
                return PartialView(transaction);
            }
            catch
            {
                return PartialView("_Error");
            }
        }

        //POST: Transaction/EditTransaction
        [HttpPost]
        public ActionResult EditTransaction([Bind(Include = "Id, Amount, TransactionType, AccountId, Description")] Transaction transaction, int? BudgetCategoryId)
        {
            if (ModelState.IsValid)
            {
                transaction.Modified = DateTimeOffset.Now;
                var userId = User.Identity.GetUserId();
                transaction.AuthorId = userId;
                var originalTransaction = db.Transactions.AsNoTracking().FirstOrDefault(t => t.Id == transaction.Id);
                decimal AccountAmount = transaction.Amount - originalTransaction.Amount;
                if (transaction.TransactionType == TransactionType.Expense)
                {
                    transaction.BudgetCategoryId = BudgetCategoryId;
                    UpdateAccountBalance(false, false, AccountAmount, transaction.AccountId);
                }
                else
                {
                    transaction.BudgetCategoryId = null;
                    UpdateAccountBalance(true, false, AccountAmount, transaction.AccountId);
                }
                db.Transactions.Attach(transaction);
                db.Entry(transaction).Property("Amount").IsModified = true;
                db.Entry(transaction).Property("Description").IsModified = true;
                db.Entry(transaction).Property("BudgetCategoryId").IsModified = true;
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = transaction.AccountId });
        }

        // GET: Delete Transaction
        public ActionResult _DeleteTransaction(int? id)
        {
            try
            {
                var model = db.Transactions.Find(id);
                return PartialView(model);
            }
            catch
            {
                return PartialView("_Error");
            }
        }

        //POST: Transaction/DeleteTransaction
        [HttpPost]
        public ActionResult DeleteTransaction(int Id, int AccountId)
        {
            var transaction = db.Transactions.Find(Id);
            bool AddBalance;
            AddBalance = (transaction.TransactionType == TransactionType.Expense) ?  true : false;
            UpdateAccountBalance(AddBalance, false, transaction.Amount, transaction.AccountId);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = AccountId });
        }

        // GET: Rename Account
        public ActionResult _RenameAccount(int? id)
        {
            try
            {
                var model = db.Accounts.Find(id);
                return PartialView(model);
            }
            catch
            {
                return PartialView("_Error");
            }
        }

        // POST: Rename Account
        [HttpPost]
        public ActionResult RenameAccount([Bind(Include = "Id, Name")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Accounts.Attach(account);
                db.Entry(account).Property("Name").IsModified = true;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Archive Account
        public ActionResult _ArchiveAccount(int? id)
        {
            try
            {
                var model = db.Accounts.Find(id);
                return PartialView(model);
            }
            catch
            {
                return PartialView("_Error");
            }
        }

        // POST: Archive Account
        [HttpPost]
        public ActionResult ArchiveAccount([Bind(Include = "Id")] Account account)
        {
            if (ModelState.IsValid)
            {
                account.IsArchived = true;
                db.Accounts.Attach(account);
                db.Entry(account).Property("IsArchived").IsModified = true;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // POST: Reactivate Account
        [HttpPost]
        public ActionResult ReactivateAccount(int Id)
        {
            if (ModelState.IsValid)
            {
                var account = db.Accounts.Find(Id);
                account.IsArchived = false;
                db.Accounts.Attach(account);
                db.Entry(account).Property("IsArchived").IsModified = true;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }
}