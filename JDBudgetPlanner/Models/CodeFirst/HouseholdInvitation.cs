using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JDBudgetPlanner.Models
{
    public class HouseholdInvitation
    {
        public int Id { get; set; }
        public Guid InviteCode { get; set; }
        public int? HouseholdId { get; set; }
        public string Email { get; set; }
        public bool Expired { get; set; }
    }
}