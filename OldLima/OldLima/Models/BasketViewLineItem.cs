using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OldLima.Models
{
    public class BasketViewLineItem
    {

        // Merchello Product Guid
        public Guid Key { get; set; }

        // Umbraco Content Id - notice in BasketViewModelExtensions that it is
        // pulled from the extended properties
        public int ContentId { get; set; }

        ///
        /// Umbraco Content page name or Merchello Product Name
        ///
        public string Name { get; set; }

        ///
        /// This is a the extended properties that travel forward with the basket
        ///
        public string ExtendedData { get; set; }

        ///
        /// The sku
        ///
        public string Sku { get; set; }

        ///
        /// The price
        ///
        public decimal UnitPrice { get; set; }

        ///
        /// The total price of the line item
        ///
        public decimal TotalPrice { get; set; }

        ///
        /// The quantity to purchase
        ///
        public int Quantity { get; set; }

        public string Images { get; set; }
    }
}