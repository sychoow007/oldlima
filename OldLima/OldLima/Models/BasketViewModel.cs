using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OldLima.Models
{// simple basket view model
    public class BasketViewModel
    {
        public decimal TotalPrice { get; set; }
        public BasketViewLineItem[] Items { get; set; }
    }
}