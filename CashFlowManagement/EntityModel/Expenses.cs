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
    
    public partial class Expenses
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public int ExpenseType { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> DisabledDate { get; set; }
        public string CreatedBy { get; set; }
        public string DisabledBy { get; set; }
        public Nullable<int> LiabilityId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public Nullable<int> ExpenseDay { get; set; }
    
        public virtual Liabilities Liabilities { get; set; }
        public virtual Users Users { get; set; }
    }
}
