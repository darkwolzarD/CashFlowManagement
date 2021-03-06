//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Users
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Users()
        {
            this.Assets = new HashSet<Assets>();
            this.Expenses = new HashSet<Expenses>();
            this.Incomes = new HashSet<Incomes>();
            this.Liabilities = new HashSet<Liabilities>();
            this.Log = new HashSet<Log>();
        }
    
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public int Sex { get; set; }
        public string Job { get; set; }
        public int NumberOfChildren { get; set; }
        public bool IncomeInitialized { get; set; }
        public bool RealEstateInitialized { get; set; }
        public bool BusinessInitialized { get; set; }
        public bool BankDepositInitialized { get; set; }
        public bool StockInitialized { get; set; }
        public bool InsuranceInitialized { get; set; }
        public bool OtherAssetInitialized { get; set; }
        public bool CarLiabilityInitialized { get; set; }
        public bool CreditCardInitialized { get; set; }
        public bool OtherLiabilityInitialized { get; set; }
        public bool FamilyExpenseInitialized { get; set; }
        public bool OtherExpenseInitialized { get; set; }
        public bool AvailableMoneyInitialized { get; set; }
        public bool CompleteInitialization { get; set; }
        public System.DateTime CreatedDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Assets> Assets { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Expenses> Expenses { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Incomes> Incomes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Liabilities> Liabilities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Log> Log { get; set; }
    }
}
