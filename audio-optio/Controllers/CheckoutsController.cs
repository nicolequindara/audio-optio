using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using audio_optio.Models;

namespace audio_optio.Controllers
{
    public class CheckoutsController : Controller
    {
        public static readonly TransactionStatus[] transactionSuccessStatuses = {
                                                                                    TransactionStatus.AUTHORIZED,
                                                                                    TransactionStatus.AUTHORIZING,
                                                                                    TransactionStatus.SETTLED,
                                                                                    TransactionStatus.SETTLING,
                                                                                    TransactionStatus.SETTLEMENT_CONFIRMED,
                                                                                    TransactionStatus.SETTLEMENT_PENDING,
                                                                                    TransactionStatus.SUBMITTED_FOR_SETTLEMENT
                                                                                };

        //public ActionResult New()
        //{
        //    var gateway = audio_optio.App_Data.Configuration.GetGateway();
        //    var clientToken = gateway.ClientToken.generate();
        //    ViewBag.ClientToken = clientToken;
        //    return View();
        //}

        public ActionResult Create(PaymentModel model)
        {
            var gateway = audio_optio.App_Data.Configuration.GetGateway();

            var nonce = Request["payment_method_nonce"];
            var request = new TransactionRequest
            {
                Amount = Convert.ToDecimal(model.Price),
                PaymentMethodNonce = nonce,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                },
                BillingAddress = new AddressRequest
                {
                    FirstName = model.BillingName,
                    StreetAddress = model.BillingAddress.line1,
                    ExtendedAddress = model.BillingAddress.line2,
                    PostalCode = model.BillingAddress.postal_code,
                    CountryCodeAlpha2 = "US"
                },
                Customer = new CustomerRequest
                {
                    FirstName = model.contactOrder.contact.FirstName,
                    LastName = model.contactOrder.contact.LastName,
                    Email = model.contactOrder.contact.Email,
                    Phone = model.contactOrder.contact.Phone
                },
                ShippingAddress = new AddressRequest
                {
                    FirstName = model.ShippingName,
                    StreetAddress = model.ShippingAddress.line1,
                    ExtendedAddress = model.ShippingAddress.line2,
                    PostalCode = model.ShippingAddress.postal_code,
                    CountryCodeAlpha2 = "US"
                },
                OrderId = model.contactOrder.order.Id.ToString()
            };

            Result<Transaction> result = gateway.Transaction.Sale(request);
            if (result.IsSuccess())
            {
                Transaction transaction = result.Target;

                // Send notification
                new EmailController().SendNotification(model.contactOrder);

                return RedirectToAction("ShowDetails", new { id = transaction.Id });

            }
            else if (result.Transaction != null)
            {
                return RedirectToAction("ShowDetails", new { id = result.Transaction.Id });
            }
            else
            {
                List<String> errorMessages = new List<String>();

                foreach (ValidationError error in result.Errors.DeepAll())
                {
                    errorMessages.Add("Error: " + (int)error.Code + " - " + error.Message);
                }
                TempData["Flash"] = errorMessages;
                return View("FailView", errorMessages);
            }
        }

        public ActionResult ShowDetails(String id)
        {
            var gateway = audio_optio.App_Data.Configuration.GetGateway();
            Transaction transaction = gateway.Transaction.Find(id);


            ViewBag.Transaction = transaction;
            return View();
        }
    }
}
