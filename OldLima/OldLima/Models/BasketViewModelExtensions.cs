using Merchello.Core.Models;
using Merchello.Web.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web;

namespace OldLima.Models
{
    ///
    /// Convert Merchello Product object to Displayable Product
    ///
    public static class BasketViewModelExtensions
    {
        public static BasketViewModel ToBasketViewModel(this IBasket basket)
        {
            return new BasketViewModel()
            {
                TotalPrice = basket.TotalBasketPrice,
                Items = basket.Items.Select(item => item.ToBasketViewLineItem()).OrderBy(x => x.Name).ToArray()
            };
        }

        ///
        /// Used to display extended data - for example purposes only
        ///
        private static string DictionaryToString(ExtendedDataCollection extendedData)
        {
            var extendedDataAsString = string.Empty;

            foreach (var dataItem in extendedData)
            {
                extendedDataAsString += dataItem.Key + ":" + dataItem.Value + ";";
            }

            return extendedDataAsString;
        }

        ///
        /// Utility extension to map a  to a BasketViewLine item.
        /// Notice that the ContentId is pulled from the extended data. The name can either
        /// be the Merchello product name via lineItem.Name or the Umbraco content page
        /// name with umbracoHelper.Content(contentId).Name
        ///
        ///
        ///The  to be mapped
        ///
        private static BasketViewLineItem ToBasketViewLineItem(this ILineItem lineItem)
        {

            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

            var contentId = lineItem.ExtendedData.ContainsKey("umbracoContentId") ? int.Parse(lineItem.ExtendedData["umbracoContentId"]) : 0;
            
            var umbracoName = umbracoHelper.Content(contentId).Name;
            var merchelloProductName = lineItem.Name;

            return new BasketViewLineItem()
            {

                Key = lineItem.ExtendedData.GetProductKey(),// lineItem.Key,
                ContentId = contentId,
                ExtendedData = DictionaryToString(lineItem.ExtendedData),
                Name = merchelloProductName,
                //Name = umbracoName,
                Sku = lineItem.Sku,
                UnitPrice = lineItem.Price,
                TotalPrice = lineItem.TotalPrice,
                Quantity = lineItem.Quantity,
                Images = lineItem.ExtendedData.GetValue("images")

            };
        }
    }
}