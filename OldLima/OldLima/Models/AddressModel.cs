using Merchello.Core;
using Merchello.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OldLima.Models
{
    public class AddressModel
    {
        // for anon or mem customer
        public Guid CustomerKey { get; set; }

        // address line 1
        public string Address1 { get; set; }

        // address line 2
        public string Address2 { get; set; }

        // country code - any string will do 
        // apply your own code system or use
        // a standard mail system
        public string CountryCode { get; set; }

        // email (no regex format enforced)
        public string Email { get; set; }

        // commerical customers
        public bool IsCommercial { get; set; }

        // city, town, village
        public string Locality { get; set; }

        // customer name
        public string Name { get; set; }

        // company name
        public string Organization { get; set; }

        // phone
        public string Phone { get; set; }

        // postal code
        public string PostalCode { get; set; }

        // state, province, etc
        public string Region { get; set; }

        // billing or shipping
        public AddressType AddressType { get; set; }
    }

    public static class AddressModelExtensions
    {
        public static IAddress ToAddress(this AddressModel address)
        {
            return new Address()
            {
                Address1 = address.Address1,
                Address2 = address.Address2,
                CountryCode = address.CountryCode,
                Email = address.Email,
                IsCommercial = address.IsCommercial,
                Locality = address.Locality,
                Name = address.Name,
                Organization = address.Organization,
                Phone = address.Phone,
                PostalCode = address.PostalCode,
                Region = address.Region
            };
        }
    }
}