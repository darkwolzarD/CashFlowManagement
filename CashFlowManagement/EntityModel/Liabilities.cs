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
    
    public partial class Liabilities
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Liabilities()
        {
            this.Expenses = new HashSet<Expenses>();
            this.Liabilities1 = new HashSet<Liabilities>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public double InterestRate { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int LiabilityType { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> DisabledDate { get; set; }
        public string CreatedBy { get; set; }
        public string DisabledBy { get; set; }
        public Nullable<int> AssetId { get; set; }
        public Nullable<int> ParentLiabilityId { get; set; }
        public string Username { get; set; }
        public Nullable<int> InterestType { get; set; }
    
        public virtual Assets Assets { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Expenses> Expenses { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Liabilities> Liabilities1 { get; set; }
        public virtual Liabilities Liabilities2 { get; set; }
        public virtual Users Users { get; set; }
    }
}