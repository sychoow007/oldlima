using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OldLima.Models
{
    public class AddItemModel
    {
        ///
        /// Content Id of the ProductDetail page
        ///
        public int ContentId { get; set; }

        ///
        /// The Product Key (pk) of the product to be added to the cart.
        ///
        public Guid ProductKey { get; set; }
    }
}