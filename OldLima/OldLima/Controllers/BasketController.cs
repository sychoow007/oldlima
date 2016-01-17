using Merchello.Core;
using OldLima.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace OldLima.Controllers
{
    [PluginController("MerchelloProductListExample")]
    public class BasketController : MerchelloSurfaceContoller
    {
        // TODO These would normally be passed in or looked up so that there is not a
        // hard coded reference
        private const int BasketContentId = 1089;

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
            return RedirectToUmbracoPage(BasketContentId);
        }
    }
}