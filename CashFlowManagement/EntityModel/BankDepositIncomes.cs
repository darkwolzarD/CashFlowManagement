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
    
    public partial class BankDepositIncomes
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public double CapitalValue { get; set; }
        public double InterestYield { get; set; }
        public string ParticipantBank { get; set; }
        public string Note { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string Username { get; set; }
    
        public virtual Users Users { get; set; }
    }
}
