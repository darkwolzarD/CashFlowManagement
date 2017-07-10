using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class NavigationViewModel
    {
        public int Id { get; set; }
        public string Display { get; set; }
        public string Url { get; set; }
        public bool Selected { get; set; }
        public bool Current { get; set; }
    }

    public class NavigationListViewModel
    {
        public List<NavigationViewModel> NavigationList { get; set; }
        public NavigationListViewModel()
        {
            NavigationList = new List<NavigationViewModel>();
        }
    }
}