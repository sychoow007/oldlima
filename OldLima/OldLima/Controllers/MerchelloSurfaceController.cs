using System;
using Merchello.Core;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Web;
using Merchello.Web.Workflow;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;
using Merchello.Core.Services;
using Merchello.Core.Gateways.Shipping;
using System.Web.Mvc;

namespace OldLima.Controllers
{
    public abstract class MerchelloSurfaceContoller : SurfaceController
    {

        private readonly IBasket _basket;

        private readonly IMerchelloContext _merchelloContext;

        private readonly ICustomerBase _currentCustomer;

        protected MerchelloSurfaceContoller(IMerchelloContext merchelloContext)
        {
            if (merchelloContext == null)
            {

                var ex = new ArgumentNullException("merchelloContext");

                LogHelper.Error<string>("The MerchelloContext was null upon instantiating the CartController.", ex);

                throw ex;
            }

            _merchelloContext = merchelloContext;

            var customerContext = new CustomerContext(UmbracoContext);
            // UmbracoContext is from SurfaceController

            _currentCustomer = customerContext.CurrentCustomer;

            _basket = _currentCustomer.Basket();

        }

        ///
        /// Gets the current customer.
        ///
        protected ICustomerBase CurrentCustomer
        {
            get { return _currentCustomer; }
        }

        ///
        /// Gets the Basket for the CurrentCustomer
        ///
        protected IBasket Basket
        {
            get { return _basket; }
        }

        ///
        /// We are going to hide the Umbraco Service Context here so 
        /// controller that sub class this controller are 
        /// "Merchello Surface Controllers"
        ///
        protected new IServiceContext Services
        {
            get { return _merchelloContext.Services; }
        }

        ///
        /// Exposes the 
        ///
        protected IPaymentContext Payment
        {

            get { return _merchelloContext.Gateways.Payment; }
        }

        ///
        /// Exposes the 
        ///
        protected IShippingContext Shipping
        {
            get { return _merchelloContext.Gateways.Shipping; }
        }
    }
}