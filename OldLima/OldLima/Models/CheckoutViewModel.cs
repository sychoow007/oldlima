using Merchello.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OldLima.Models
{
    public class CheckoutViewModel
    {
        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AddressModel CustomerAddress { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public LineItemCollection Items { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PaymentType { get; set; }

        /// <summary>
        /// Gets or sets the total basket price.
        /// </summary>
        public decimal TotalBasketPrice { get; set; }

        /// <summary>
        /// Gets or sets the receipt url.
        /// </summary>
        public string ReceiptUrl { get; set; }
    }
}