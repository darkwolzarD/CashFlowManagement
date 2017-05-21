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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StockTransactions()
        {
            this.Liabilities = new HashSet<Liabilities>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public int TransactionType { get; set; }
        public int NumberOfShares { get; set; }
        public double SpotPrice { get; set; }
        public double AveragePrice { get; set; }
        public double BrokerFee { get; set; }
        public double Value { get; set; }
        public double ExpectedDividend { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> DisabledDate { get; set; }
        public string DisabledBy { get; set; }
        public string Username { get; set; }
        public string Note { get; set; }
        public int AssetId { get; set; }
        public Nullable<int> CashId { get; set; }
    
        public virtual Assets Assets { get; set; }
        public virtual Assets Assets1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Liabilities> Liabilities { get; set; }
    }
}
