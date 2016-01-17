using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OldLima.Models
{
    public class PaymentInformationModel : AddressModel
    {
        public Guid PaymentMethodKey { get; set; }
    }
}