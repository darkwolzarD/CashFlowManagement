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
    
    public partial class FinancialStatus
    {
        public int Id { get; set; }
        public double FinancialFreedom { get; set; }
        public double MonthlyCashflow { get; set; }
        public double PassiveIncome { get; set; }
        public string Username { get; set; }
    
        public virtual Users Users { get; set; }
    }
}