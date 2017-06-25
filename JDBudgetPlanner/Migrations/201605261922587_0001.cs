namespace KeepSaving.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0001 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Budgets", "Id", "dbo.Households");
            DropForeignKey("dbo.BudgetCategories", "Id", "dbo.BudgetItems");
            DropForeignKey("dbo.BudgetItems", "BudgetId", "dbo.Budgets");
            DropForeignKey("dbo.Transactions", "BudgetCategoryId", "dbo.BudgetCategories");
            DropIndex("dbo.BudgetCategories", new[] { "Id" });
            DropColumn("dbo.Budgets", "HouseholdId");
            RenameColumn(table: "dbo.Budgets", name: "Id", newName: "HouseholdId");
            RenameIndex(table: "dbo.Budgets", name: "IX_Id", newName: "IX_HouseholdId");
            DropPrimaryKey("dbo.Budgets");
            DropPrimaryKey("dbo.BudgetCategories");
            AddColumn("dbo.BudgetItems", "BudgetCategoryId", c => c.Int());
            AlterColumn("dbo.Budgets", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.BudgetCategories", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Budgets", "Id");
            AddPrimaryKey("dbo.BudgetCategories", "Id");
            CreateIndex("dbo.BudgetItems", "BudgetCategoryId");
            AddForeignKey("dbo.Budgets", "HouseholdId", "dbo.Households", "Id", cascadeDelete: true);
            AddForeignKey("dbo.BudgetItems", "BudgetCategoryId", "dbo.BudgetCategories", "Id");
            AddForeignKey("dbo.BudgetItems", "BudgetId", "dbo.Budgets", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Transactions", "BudgetCategoryId", "dbo.BudgetCategories", "Id");
            DropColumn("dbo.Households", "BudgetId");
            DropColumn("dbo.BudgetCategories", "BudgetItemId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BudgetCategories", "BudgetItemId", c => c.Int(nullable: false));
            AddColumn("dbo.Households", "BudgetId", c => c.Int());
            DropForeignKey("dbo.Transactions", "BudgetCategoryId", "dbo.BudgetCategories");
            DropForeignKey("dbo.BudgetItems", "BudgetId", "dbo.Budgets");
            DropForeignKey("dbo.BudgetItems", "BudgetCategoryId", "dbo.BudgetCategories");
            DropForeignKey("dbo.Budgets", "HouseholdId", "dbo.Households");
            DropIndex("dbo.BudgetItems", new[] { "BudgetCategoryId" });
            DropPrimaryKey("dbo.BudgetCategories");
            DropPrimaryKey("dbo.Budgets");
            AlterColumn("dbo.BudgetCategories", "Id", c => c.Int(nullable: false));
            AlterColumn("dbo.Budgets", "Id", c => c.Int(nullable: false));
            DropColumn("dbo.BudgetItems", "BudgetCategoryId");
            AddPrimaryKey("dbo.BudgetCategories", "Id");
            AddPrimaryKey("dbo.Budgets", "Id");
            RenameIndex(table: "dbo.Budgets", name: "IX_HouseholdId", newName: "IX_Id");
            RenameColumn(table: "dbo.Budgets", name: "HouseholdId", newName: "Id");
            AddColumn("dbo.Budgets", "HouseholdId", c => c.Int(nullable: false));
            CreateIndex("dbo.BudgetCategories", "Id");
            AddForeignKey("dbo.Transactions", "BudgetCategoryId", "dbo.BudgetCategories", "Id");
            AddForeignKey("dbo.BudgetItems", "BudgetId", "dbo.Budgets", "Id", cascadeDelete: true);
            AddForeignKey("dbo.BudgetCategories", "Id", "dbo.BudgetItems", "Id");
            AddForeignKey("dbo.Budgets", "Id", "dbo.Households", "Id");
        }
    }
}
