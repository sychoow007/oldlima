using System;
using System.Linq;
using System.Web.Mvc;
using OldLima.Models;
using Merchello.Core;
using Merchello.Core.Models;
using Umbraco.Core.Logging;
using System.Globalization;

namespace OldLima.Controllers
{
    //[PluginController("MerchelloProductListExample")]
    public class BasketController : MerchelloSurfaceContoller
    {
        // TODO These would normally be passed in or looked up so that there is not a
        // hard coded reference
        private const int BasketContentId = 1465;

        public BasketController()
            : this(MerchelloContext.Current)
        {
        }
        public BasketController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
        }
        // GET: Basket
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Display_BuyButton(AddItemModel product)
        {
            return PartialView("BuyButton", product);
        }

        [HttpPost]
        public ActionResult AddToBasket(AddItemModel model)
        {

            // Add to Logical Basket
            // add Umbraco content id to Merchello Product extended data
            var extendedData = new ExtendedDataCollection();

            extendedData.SetValue("umbracoContentId", model.ContentId.ToString(CultureInfo.InvariantCulture));

            // get Merchello product
            var product = Services.ProductService.GetByKey(model.ProductKey);

            // add a single item of the Product to the logical Basket
            Basket.AddItem(product, product.Name, model.OrderAmount, extendedData);

            // Save to Database tables: merchItemCache, merchItemCacheItem
            Basket.Save();

            return RedirectToUmbracoPage(BasketContentId);
        }

        [ChildActionOnly]
        public ActionResult DisplayBasket()
        {
            return PartialView("Basket", GetBasketViewModel());
        }

        ///
        /// Responsible for updating the quantities of items in the basket.
        ///
        /// Redirects to the current Umbraco page (the basket page)
        [HttpPost]
        public ActionResult UpdateBasket(BasketViewModel model)
        {
            if (ModelState.IsValid)
            {
                // The only thing that can be updated in this basket is the quantity
                foreach (var item in model.Items)
                {
                    if (Basket.Items.First(x => x.ExtendedData.GetProductKey() == item.Key).Quantity != item.Quantity)
                    {
                        Basket.UpdateQuantity(item.Key, item.Quantity);
                    }
                }

                // Tidbit - Everytime "Save()" is called on the Basket, a new VersionKey (Guid) is generated.
                // This is used to validate the SalePreparationBase (BasketCheckoutPrepartion in this case),
                // asserting that any previously saved information (rate quotes, shipments ...) correspond to the Basket version.

                // If the versions do not match, the the checkout workflow is essentially reset - meaning
                // you have to start the checkout process all over
                Basket.Save();
            }
            return RedirectToUmbracoPage(BasketContentId);
        }

        ///
        /// Removes an item from the basket
        ///
        [HttpGet]
        public ActionResult RemoveItemFromBasket(Guid lineItemKey)
        {
            if (Basket.Items.FirstOrDefault(x => x.ExtendedData.GetProductKey() == lineItemKey) == null)
            {
                var exception = new InvalidOperationException("Attempt to delete an item from a basket that does not match the CurrentUser");

                LogHelper.Error<InvalidOperationException>("RemoveItemFromBasket failed.", exception);

                throw exception;
            }

            // remove product 
            Basket.RemoveItem(lineItemKey);

            Basket.Save();

            return RedirectToUmbracoPage(BasketContentId);
        }

        private BasketViewModel GetBasketViewModel()
        {

            // ToBasketViewModel is an extension that
            // translates the IBasket to a local view model which
            // can be submitted via a form.
            var basketViewModel = Basket.ToBasketViewModel();

            // grab customer id - for example only
            // regardless if anon or known customer
            // stored in merchAnonymousCustomer table
            // or merchCustomer table
            //if (CurrentCustomer.IsAnonymous)
            //{
            //    basketViewModel.Customer = "Anonymous Customer";
            //}
            //else
            //{
            //    basketViewModel.Customer = "Friend";
            //}
            return basketViewModel;
        }

        [HttpGet]
        public ActionResult DisplayCustomerBasket()
        {
            return PartialView("Customer", GetBasketViewModel());
        }
    }
}