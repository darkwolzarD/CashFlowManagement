using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CashFlowManagement.EntityModel
{
    [MetadataType(typeof(IncomeMetadata))]
    public partial class Incomes
    {
    }

    [MetadataType(typeof(ExpenseMetadata))]
    public partial class Expense
    {
    }
}