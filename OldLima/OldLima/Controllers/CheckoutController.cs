using System;
using System.Web.Mvc;
using Merchello.Core;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Web;
using OldLima.Models;
using Umbraco.Core;

namespace OldLima.Controllers
{
    public class CheckoutController : MerchelloSurfaceContoller
    {
        /// <summary>
        /// Summary description for CheckoutController
        /// 
        /// Workflow
        /// 1. Display Basket Items http://website/checkout
        /// 2. Get Address information http://website/checkout/address
        /// 3. Get Payment information http://website/checkout/payment
        /// 4. Show Invoice http://website/checkout/invoice?inv=[machineEncryptedKey]
        /// </summary>
            private const int BasketPageId = 1483;
            private const int PaymentInfoId = 1485;
            private const int ReceiptId = 1486;

            /// <summary>
            /// Merchello audit log to capture the error events
            /// 
            /// </summary>
            private AuditLogService _log = new AuditLogService();

            /// <summary>
            /// Constructor without current context
            /// </summary>
            public CheckoutController()
                : this(MerchelloContext.Current)
            { }

            /// <summary>
            /// Constructor with current context
            /// </summary>
            /// <param name="merchelloContext"></param>
            public CheckoutController(IMerchelloContext merchelloContext)
                : base(merchelloContext)
            { }

            /// <summary>
            /// Display address form - for this example shipping and billing are the same
            /// </summary>
            /// <param name="addressType"></param>
            /// <returns></returns>
            [ChildActionOnly]
            public ActionResult RenderAddressForm(AddressType addressType)
            {
                ViewBag.AddressType = addressType;

                return PartialView("Address");
            }

            /// <summary>
            /// Save address to both billing and shipping
            /// The anonymous customer address is stored in extended column
            /// on merchAnonymousCustomer
            /// 
            /// I save shipping address as an example. It isn't used anywhere since
            /// there isn't anything in the Order which is considered the Shippable
            /// piece of the process.
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            [HttpPost]
            public ActionResult SaveAddress(AddressModel model)
            {
                var address = model.ToAddress();

                // address saved to extended data on table merchAnonymousCustomer
                Basket.SalePreparation().SaveBillToAddress(address);
                Basket.SalePreparation().SaveShipToAddress(address);

                // go to payment page - only the cash payment is installed
                return RedirectToUmbracoPage(PaymentInfoId);
            }

            /// <summary>
            /// Save payment is really process payment in this cash system. 
            /// As this system doesn't take credit cards, add tax, or ship, those
            /// pieces are purposely not shown.
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            [HttpPost]
            public ActionResult SavePayment(PaymentInformationModel model)
            {
                // has error occurred
                bool error = false;

                // do we raise events
                bool raiseEvents = false;

                // payment attempt result information
                IPaymentResult attempt = null;

                // get payment method
                var paymentMethod = Payment.GetPaymentGatewayMethodByKey(model.PaymentMethodKey).PaymentMethod;

                // get customer, items
                var preparation = base.Basket.SalePreparation();

                // Save the payment method selection
                preparation.SavePaymentMethod(paymentMethod);

                // make sure there is a billing address - it can be empty - it just has to exist
                if (!preparation.IsReadyToInvoice()) return RedirectToUmbracoPage(BasketPageId);

                // AuthorizePayment will save the invoice with an Invoice Number.
                attempt = preparation.AuthorizePayment(paymentMethod.Key);

                // if payment isn't successful, grab some information
                if (!attempt.Payment.Success)
                {
                    error = true;

                    // TBD - Not in Merchello yet
                    // Notification.Trigger("OrderConfirmationFailure", attempt, new[] { preparation.GetBillToAddress().Email });

                    _log.CreateAuditLogWithKey("Checkout failed - attempt payment failure", preparation.Customer.ExtendedData);
                }
                else
                {
                    // trigger the order notification confirmation
                    Notification.Trigger("OrderConfirmation", attempt, new[] { preparation.GetBillToAddress().Email });
                }

                // grab final content page
                var receipt = Umbraco.TypedContent(ReceiptId);

                // redirect so that url has invoice number (encrypted) in address bar
                // this feels clunky and unsafe but illustrative all the same
                return
                    Redirect(string.Format("{0}?inv={1}", receipt.Url,
                                           attempt.Invoice.Key.ToString().EncryptWithMachineKey()));
            }

            /// <summary>
            /// Render Receipt - sale has been processed
            /// </summary>
            /// <param name="invoiceKey"></param>
            /// <returns></returns>
            [ChildActionOnly]
            public ActionResult RenderReceipt(string invoiceKey)
            {
                Guid key;
                if (Guid.TryParse(invoiceKey, out key))
                {
                    var invoice = Services.InvoiceService.GetByKey(key);
                    return PartialView("Invoice", invoice);
                }
                throw new InvalidOperationException();
            }

            /// <summary>
            /// Render Invoice (name, address, items, total)
            /// </summary>
            /// <param name="invoice"></param>
            /// <returns></returns>
            private ActionResult RenderInvoice(IInvoice invoice)
            {
                return PartialView("Invoice", invoice);
            }

            /// <summary>
            /// When the payment is successful (i.e. Sale is complete)
            /// the basket is empty again. So decide if we are getting 
            /// information from basket or invoice
            /// </summary>
            /// <returns></returns>
            public ActionResult RenderInvoiceSummary(bool dataFromInvoice, Guid invoiceKey)
            {
                if (!dataFromInvoice)
                {
                    return RenderIncompleteInvoiceSummary();
                }
                else
                {
                    return RenderCompleteInvoiceSummary(invoiceKey);
                }

            }

            /// <summary>
            /// Renders checkout information in process so
            /// gets everything from basket
            /// </summary>
            /// <returns></returns>
            private ActionResult RenderIncompleteInvoiceSummary()
            {

                var model = new CheckoutViewModel();

                if ((base.Basket != null) &&
                    (base.Basket.SalePreparation() != null))
                {
                    if (base.Basket.SalePreparation().GetBillToAddress() != null)
                    {
                        model.CustomerName = base.Basket.SalePreparation().GetBillToAddress().Name ?? "";

                        model.CustomerAddress = new AddressModel();

                        model.CustomerAddress.Email = base.Basket.SalePreparation().GetBillToAddress().Email ?? "";
                        model.CustomerAddress.Address1 = base.Basket.SalePreparation().GetBillToAddress().Address1 ?? "";
                        model.CustomerAddress.Locality = base.Basket.SalePreparation().GetBillToAddress().Locality ?? "";
                        model.CustomerAddress.CountryCode = base.Basket.SalePreparation().GetBillToAddress().CountryCode ?? "";
                        model.CustomerAddress.PostalCode = base.Basket.SalePreparation().GetBillToAddress().PostalCode ?? "";
                        model.CustomerAddress.Region = base.Basket.SalePreparation().GetBillToAddress().Region ?? "";
                    }

                    if (base.Basket.SalePreparation().GetPaymentMethod() != null)
                    {
                        model.PaymentType = base.Basket.SalePreparation().GetPaymentMethod().Name ?? "";
                    }

                    if (base.Basket.Items != null)
                    {
                        model.Items = base.Basket.Items;
                    }

                    model.TotalBasketPrice = base.Basket.TotalBasketPrice;
                }


                return PartialView("CheckoutSummary", model);
            }
            /// <summary>
            /// Renders checkout information after sale is done, so gets
            /// everything from invoice. Basket is now empty once again.
            /// </summary>
            /// <param name="invoiceKey"></param>
            /// <returns></returns>
            private ActionResult RenderCompleteInvoiceSummary(Guid invoiceKey)
            {

                var invoice = Services.InvoiceService.GetByKey(invoiceKey);

                var model = new CheckoutViewModel();

                // init objects
                model.CustomerAddress = new AddressModel();
                model.Items = new LineItemCollection();

                var customeraddress = new AddressModel();

                model.CustomerName = invoice.BillToName ?? "";

                customeraddress.Email = invoice.BillToEmail ?? "";
                customeraddress.Address1 = invoice.BillToAddress1 ?? "";
                customeraddress.Locality = invoice.BillToLocality ?? "";
                customeraddress.CountryCode = invoice.BillToCountryCode ?? "";
                customeraddress.PostalCode = invoice.BillToPostalCode ?? "";
                customeraddress.Region = invoice.BillToRegion ?? "";

                model.CustomerAddress = customeraddress;

                //model.PaymentType = invoice.;

                model.Items = invoice.Items;
                model.TotalBasketPrice = invoice.Total;

                return PartialView("CheckoutSummary", model);
            }
    }
}