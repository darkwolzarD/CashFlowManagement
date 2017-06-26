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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Incomes()
        {
            this.Assets = new HashSet<Assets>();
            this.History = new HashSet<History>();
            this.Incomes1 = new HashSet<Incomes>();
        }
    
        public int Id { get; set; }
        public double Value { get; set; }
        public Nullable<int> IncomeDay { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> DisabledDate { get; set; }
        public string CreatedBy { get; set; }
        public string DisabledBy { get; set; }
        public int IncomeType { get; set; }
        public Nullable<int> AssetId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public Nullable<int> ParentId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Assets> Assets { get; set; }
        public virtual Assets Assets1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<History> History { get; set; }
        public virtual Users Users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Incomes> Incomes1 { get; set; }
        public virtual Incomes Incomes2 { get; set; }
    }
}
