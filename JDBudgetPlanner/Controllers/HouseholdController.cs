using JDBudgetPlanner.Helpers;
using JDBudgetPlanner.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JDBudgetPlanner.Controllers
{
    [RequireHttps]
    public class HouseholdController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [AuthorizeHouseholdRequired]
        // GET: Household
        public ActionResult Index()
        {
            var householdId = User.Identity.GetHouseholdId();
            Household household = db.Households.Find(householdId);
            ViewBag.Message = TempData["Message"];
            ViewBag.Error = TempData["Error"];
            return View(household);
        }

        // GET: Household/JoinHousehold
        public ActionResult JoinHousehold()
        {
            return View();
        }

        // POST: Household/JoinHousehold
        [HttpPost]
        public async Task<ActionResult> JoinHousehold(Guid? inviteCode)
        {
            var invite = db.HouseholdInvitations.FirstOrDefault(i => i.InviteCode == inviteCode);
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            if (invite != null && invite.Expired == false)
            {
                invite.Expired = true;
                db.HouseholdInvitations.Attach(invite);
                db.Entry(invite).Property("Expired").IsModified = true;
                db.SaveChanges();
                var household = db.Households.Find(invite.HouseholdId);
                if (household != null)
                {
                    household.Users.Add(user);
                    db.SaveChanges();
                    await ControllerContext.HttpContext.RefreshAuthentication(user);
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Message = "That invitation is invalid or has expired.";
            return View();
        }

        // POST: Household/CreateHousehold
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateHousehold([Bind(Include = "Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                //Create household
                household.Create = System.DateTimeOffset.Now;
                db.Households.Add(household);
                db.SaveChanges();
                //Add user to household
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                var thisHousehold = db.Households.Find(household.Id);
                thisHousehold.Users.Add(user);
                //Create budget for household
                Budget budget = new Budget();
                budget.Created = System.DateTimeOffset.Now;
                budget.Amount = 0;
                budget.HouseholdId = thisHousehold.Id;
                budget.Household = thisHousehold;
                db.Budgets.Add(budget);
                //Update household to include budget
                thisHousehold.Budget = budget;
                thisHousehold.BudgetId = budget.Id;
                db.Households.Attach(thisHousehold);
                db.Entry(thisHousehold).Property("BudgetId").IsModified = true;
                db.SaveChanges();
                //Refresh cookies to add new household Id
                await ControllerContext.HttpContext.RefreshAuthentication(user);
                return RedirectToAction("Index");
            }

            return View();
        }

        // POST: Household/InviteUser/5
        [HttpPost]
        public async Task<ActionResult> InviteUser(string inviteEmail)
        {
            if (!string.IsNullOrWhiteSpace(inviteEmail))
            {
                int? householdId = User.Identity.GetHouseholdId();
                string userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                HouseholdInvitation invitation = new HouseholdInvitation();
                invitation.InviteCode = Guid.NewGuid();
                invitation.Email = inviteEmail;
                invitation.HouseholdId = householdId;
                db.HouseholdInvitations.Add(invitation);
                db.SaveChanges();

                var svc = new EmailService();
                var msg = new IdentityMessage();
                msg.Destination = inviteEmail;
                msg.Subject = user.FullName + " has invited you to join JD Budget Planner";
                msg.Body = user.FullName + " has invited you to join their household on JD Budget Planner! JD Budget Planner is an application for easily managing your transactions and financial budgeting. To join " + user.FullName + "'s household, visit jantoine.budgeter.azurewebsites.net and enter the following invitation code: " + invitation.InviteCode;
                await svc.SendAsync(msg);
                TempData["Message"] = "Your invitation has been sent!";
            }

            return RedirectToAction("Index");
        }

        // POST: Household/LeaveHousehold/5
        [HttpPost]
        public async Task<ActionResult> LeaveHousehold(bool? confirmLeaveHousehold)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var householdId = User.Identity.GetHouseholdId();
            var household = db.Households.Find(householdId);

            if (confirmLeaveHousehold != null && household.Users.Contains(user))
            {
                household.Users.Remove(user);
                db.SaveChanges();
                await ControllerContext.HttpContext.RefreshAuthentication(user);
                return RedirectToAction("JoinHousehold");
            }

            TempData["Error"] = "Please confirm you want to leave this household.";
            return RedirectToAction("Index");
        }
    }
}
