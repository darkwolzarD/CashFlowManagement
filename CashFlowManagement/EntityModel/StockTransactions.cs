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
    
    public partial class StockTransactions
    {
        public int Id { get; set; }
        public int TransactionType { get; set; }
        public System.DateTime TradeDay { get; set; }
        public int NumberOfShares { get; set; }
        public double SpotPrice { get; set; }
        public double ExpectedDiviend { get; set; }
        public double MortgageValue { get; set; }
        public double ExpenseInterest { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string Note { get; set; }
        public int StockId { get; set; }
    
        public virtual StockCodes StockCodes { get; set; }
    }
}
