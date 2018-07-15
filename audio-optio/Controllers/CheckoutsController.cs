using Braintree;

using System;
using System.Collections.Generic;
using System.Web.Mvc;

using audio_optio.Database;
using audio_optio.Models;
using audio_optio.Services;

namespace audio_optio.Controllers
{
    public class CheckoutsController : Controller
    {
        private IEmailService emailService_;
        private AddressRepository addressRepo_;

        public CheckoutsController(IEmailService emailService)
        {
            AoDbContext context = new AoDbContext();
            addressRepo_ = new AddressRepository(context);

            emailService_ = emailService;
        }


        public ActionResult Update(PaymentModel model)
        {
            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Create(PaymentModel model)
        {
            // Gateway token in view
            var gateway = audio_optio.App_Data.Configuration.GetGateway();
            var nonce = Request["payment_method_nonce"];

            model.Format();
            foreach (String error in model.Validate())
            {
                ModelState.AddModelError("error", error);
            }
            
            if (ModelState.IsValid)
            {
                // Save addresses
                model.contactOrder.order.ShippingAddress.Contact = model.contactOrder.contact;
                model.contactOrder.order.BillingAddress.Contact = model.contactOrder.contact;
                addressRepo_.Insert(model.contactOrder.order.ShippingAddress);
                addressRepo_.Insert(model.contactOrder.order.BillingAddress);
            }
            else
            {
                // Recreate gateway token for view 
                var clientToken = gateway.ClientToken.Generate();
                ViewBag.ClientToken = clientToken;
                return View("Pay", model);
            }

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
                    FirstName = model.contactOrder.order.BillingAddress.To,
                    StreetAddress = model.contactOrder.order.BillingAddress.AddressLine1,
                    ExtendedAddress = model.contactOrder.order.BillingAddress.AddressLine2,
                    PostalCode = model.contactOrder.order.BillingAddress.PostalCode,
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
                    FirstName = model.contactOrder.order.ShippingAddress.To,
                    StreetAddress = model.contactOrder.order.ShippingAddress.AddressLine1,
                    ExtendedAddress = model.contactOrder.order.ShippingAddress.AddressLine2,
                    PostalCode = model.contactOrder.order.ShippingAddress.PostalCode,
                    CountryCodeAlpha2 = "US"
                },
                OrderId = model.contactOrder.order.Id.ToString()
            };

            Result<Transaction> result = gateway.Transaction.Sale(request);
            if (result.IsSuccess())
            {
                Transaction transaction = result.Target;

                // Send notification
                emailService_.SendOrderEmail(model);

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
