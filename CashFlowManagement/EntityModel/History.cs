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
    
    public partial class History
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int ActionType { get; set; }
        public Nullable<int> AssetId { get; set; }
        public Nullable<int> IncomeId { get; set; }
        public Nullable<int> LiabilityId { get; set; }
        public Nullable<int> ExpenseId { get; set; }
        public string Content { get; set; }
        public string Field { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Username { get; set; }
    
        public virtual Assets Assets { get; set; }
        public virtual Expenses Expenses { get; set; }
        public virtual Incomes Incomes { get; set; }
    }
}
