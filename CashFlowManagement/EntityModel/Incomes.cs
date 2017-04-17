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
    
    public partial class Incomes
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> DisabledDate { get; set; }
        public string CreatedBy { get; set; }
        public string DisabledBy { get; set; }
        public int IncomeType { get; set; }
        public Nullable<int> AssetId { get; set; }
        public string Username { get; set; }
    
        public virtual Assets Assets { get; set; }
        public virtual Users Users { get; set; }
    }
}