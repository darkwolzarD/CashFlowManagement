﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CashFlowManagement.EntityModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CashFlowManagementEntities : DbContext
    {
        public CashFlowManagementEntities()
            : base("name=CashFlowManagementEntities")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BankDepositIncomes> BankDepositIncomes { get; set; }
        public virtual DbSet<BusinessIncomes> BusinessIncomes { get; set; }
        public virtual DbSet<Expenses> Expenses { get; set; }
        public virtual DbSet<FinancialStatus> FinancialStatus { get; set; }
        public virtual DbSet<Loans> Loans { get; set; }
        public virtual DbSet<RealEstateIncomes> RealEstateIncomes { get; set; }
        public virtual DbSet<Salary> Salary { get; set; }
        public virtual DbSet<StockCodes> StockCodes { get; set; }
        public virtual DbSet<StockTransactions> StockTransactions { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<TransactionFee> TransactionFee { get; set; }
    }
}
