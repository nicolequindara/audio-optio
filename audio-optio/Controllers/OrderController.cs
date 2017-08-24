using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;

using audio_optio.App_Data;
using audio_optio.Domain;
using audio_optio.Models;
using audio_optio.Database;

using PayPal.Api;

using Order = audio_optio.Domain.Order;

namespace audio_optio.Controllers
{
    public class OrderController : Controller
    {

        private Payment payment;

        private string orderEmail = Configuration.EmailCredentials.Email;
        private string pw = Configuration.EmailCredentials.Password;

        private ContactRepository contacts;
        private OrderRepository orders;

        public OrderController()
        {
            AoDbContext context = new AoDbContext();
            contacts = new ContactRepository(context);
            orders = new OrderRepository(context);
        }


        public ActionResult Order()
        {
            ContactOrderModel m = new ContactOrderModel();

            return View(m);
        }

        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        // GET: Order/Details/5
        [Route("Order/Details/{id}")]
        public ActionResult Details(int id)
        {
            Order order = orders.Get(id);

            if(order == null)
            {
                return RedirectToAction("Order");
            }

            return View(order);
        }

        // POST: Order/Create
        [HttpPost]
        public ActionResult Create(ContactOrderModel model)
        {
            model.success = false;

            try
            {

                if (ModelState.IsValid)
                {
                    model.contact.Format();

                    model.success = true;

                    Contact c = contacts.Get(model.contact.FirstName, model.contact.LastName);
                    if (c == null)
                    {
                        c = model.contact;
                        contacts.Insert(c);
                    }

                    contacts.Update(c);

                    model.order.Contact = c;
                    model.order.DateSubmitted = DateTime.Now;
                    orders.Insert(model.order);

                    return CreatePayment(model);
                }
                else
                {
                    throw new Exception();
                }

            }
            catch (Exception e)
            {
                model.success = false;
                ModelState.AddModelError("UnhandledException", e.Message);
                return Update(model);
            }
        }
        
        public ActionResult Update(ContactOrderModel model)
        {
            return View("Order", model);
        }


        public ActionResult CreatePayment(ContactOrderModel contactOrder)
        {
            PaymentModel m = new PaymentModel { contactOrder = contactOrder };
            m.Price = m.contactOrder == null ? 0.0f : ContactOrderModel.getPrice(m.contactOrder.order.Size);
            m.contactOrder.order.OrderStatus = audio_optio.Domain.Order.Status.Pending;
            return View("Pay", m);
        }


        [HttpPost]
        public ActionResult Pay(PaymentModel model)
        {
            model.Format();

            if (ModelState.IsValid)
            {
                List<String> errors = model.Validate();
                foreach (String error in errors)
                {
                    ModelState.AddModelError("Model Error", error);
                }
                model.success = errors.Count == 0;

                if (model.success)
                {
                    if (model.payPaypal)
                    {
                        return PaymentWithPaypal(model);
                    }
                    else
                    {
                        return PaymentWithCreditCard(model);
                    }
                }
                else
                {
                    return View("Pay", model);
                }
            }
            else
            {
                return View("Pay", model);
            }
        }

        public ActionResult PaymentWithCreditCard(PaymentModel model)
        {

            //Now Create an object of credit card and add above details to it
            //Please replace your credit card details over here which you got from paypal
            CreditCard cc = model.CreditCard;
            cc.billing_address = model.BillingAddress;
      
            // Now we need to specify the FundingInstrument of the Payer
            // for credit card payments, set the CreditCard which we made above
            FundingInstrument fundInstrument = new FundingInstrument();
            fundInstrument.credit_card = cc;

            // The Payment creation API requires a list of FundingIntrument

            List<FundingInstrument> fundingInstrumentList = new List<FundingInstrument>();
            fundingInstrumentList.Add(fundInstrument);

            // Now create Payer object and assign the fundinginstrument list to the object
            Payer payer = new Payer();
            payer.funding_instruments = fundingInstrumentList;
            payer.payment_method = "credit_card";
            
            try
            {

                //Basically, apiContext object has a accesstoken which is sent by the paypal
                //to authenticate the payment to facilitator account.
                //An access token could be an alphanumeric string

                APIContext apiContext = Configuration.GetAPIContext();

                // Creating a payment
                // baseURL is the url on which paypal sendsback the data.
                // So we have provided URL of this controller only
                string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority +
                            "/Order/SuccessView?";

                //guid we are generating for storing the paymentID received in session
                //after calling the create function and it is used in the payment execution

                var guid = Convert.ToString((new Random()).Next(100000));

                //CreatePayment function gives us the payment approval url
                //on which payer is redirected for paypal account payment

                // finally create the payment object and assign the payer object & transaction list to it
                payment = CreatePayment(model, apiContext, baseURI + "guid=" + guid);
                payment.intent = "sale";
                payment.payer = payer;

                //Create is a Payment class function which actually sends the payment details
                //to the paypal API for the payment. The function is passed with the ApiContext
                //which we received above.
                Payment createdPayment = payment.Create(apiContext);

                //if the createdPayment.state is "approved" it means the payment was successful else not

                if (createdPayment.state.ToLower() != "approved")
                {
                    return View("FailureView");
                }
            }
            catch (PayPal.PayPalException ex)
            {
                return View("FailureView");
            }
            catch(Exception ex)
            {
                return View("FailureView");
            }

            return View("SuccessView");
        }

        public ActionResult SuccessView()
        {
            //getting the apiContext as earlier
            APIContext apiContext = Configuration.GetAPIContext();

            string payerId = Request.Params["PayerID"];
            if (string.IsNullOrEmpty(payerId))
            {
                return View("FailureView");
            }

            // This section is executed when we have received all the payments parameters
            // from the previous call to the function Create
            // Executing a payment

            var guid = Request.Params["guid"];
            Payment executedPayment;

            try
            {
                executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

            }
            catch (PayPal.Exception.PayPalException ex)
            {
                ModelState.AddModelError("PayPal Error", ex);
                return View("FailureView");
            }

            if (executedPayment.state.ToLower() != "approved")
            {
                return View("FailureView");
            }

            ContactOrderModel cachedModel = (TempData["paymentModel"] as PaymentModel).contactOrder;
            try
            {
                new EmailController().SendNotification(cachedModel);
            }
            catch(Exception e)
            {
                ModelState.AddModelError("E-mail Error", e.Message);
            }

            return RedirectToAction("Details", new { id = cachedModel.order.Id });
        }

        public ActionResult PaymentWithPaypal(PaymentModel model)
        {
            //getting the apiContext as earlier
            APIContext apiContext = Configuration.GetAPIContext();

            try
            {
                //this section will be executed first because PayerID doesn't exist
                //it is returned by the create function call of the payment class

                // Creating a payment
                // baseURL is the url on which paypal sendsback the data.
                // So we have provided URL of this controller only
                string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority +
                            "/Order/SuccessView?";

                //guid we are generating for storing the paymentID received in session
                //after calling the create function and it is used in the payment execution

                var guid = Convert.ToString((new Random()).Next(100000));

                //CreatePayment function gives us the payment approval url
                //on which payer is redirected for paypal account payment

                var createdPayment = this.CreatePayment(model, apiContext, baseURI + "guid=" + guid);

                //get links returned from paypal in response to Create function call

                var links = createdPayment.links.GetEnumerator();

                string paypalRedirectUrl = null;

                while (links.MoveNext())
                {
                    Links lnk = links.Current;

                    if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                    {
                        //saving the payapalredirect URL to which user will be redirected for payment
                        paypalRedirectUrl = lnk.href;
                    }
                }

                // saving the paymentID in the key guid
                Session.Add(guid, createdPayment.id);

                TempData["paymentModel"] = model;

                return Redirect(paypalRedirectUrl);
            }
            catch (PayPal.Exception.PayPalException ex)
            {
                ModelState.AddModelError("Exception", HttpContext.Response.ToString());
                return View("FailureView");
            }
        }

        private Payment CreatePayment(PaymentModel model, APIContext apiContext, string redirectUrl)
        {

            //similar to credit card create itemlist and add item objects to it
            var itemList = new ItemList() { items = new List<Item>() };

            itemList.items.Add(new Item()
            {
                name = string.Format("Size: {0}, Youtube: {1}, Comments: {2}", audio_optio.Domain.Order.GetCanvasDescription(model.contactOrder.order.Size), model.contactOrder.order.YoutubeLink, model.contactOrder.order.Comments),
                currency = "USD",
                price = model.Price.ToString(),
                quantity = "1",
                sku = "sku"
            });

            var payer = new Payer() { payment_method = "paypal" };

            // similar as we did for credit card, do here and create details object

            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = model.Price.ToString()
            };

            // similar as we did for credit card, do here and create amount object
            var amount = new Amount()
            {
                currency = "USD",
                total = model.Price.ToString(), // Total must be equal to sum of shipping, tax and subtotal.
                details = details
            };

            var transactionList = new List<Transaction>();

            transactionList.Add(new Transaction()
            {
                description =
                    string.Format("{0} {1} - ${2} USD - {3} {4}",
                    model.contactOrder.contact.FirstName,
                    model.contactOrder.contact.LastName,
                    model.Price,
                    DateTime.Now.ToLongDateString(),
                    DateTime.Now.ToLongTimeString()),
                invoice_number = model.contactOrder.order.Id.ToString(),
                amount = amount,
                item_list = itemList
            });

            payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList
            };

            // Configure Redirect Urls here with RedirectUrls object
            RedirectUrls redirUrls;
            if (!String.IsNullOrEmpty(redirectUrl))
            {
                redirUrls = new RedirectUrls()
                {
                    cancel_url = redirectUrl,
                    return_url = redirectUrl
                };
                payment.redirect_urls = redirUrls;
            }
            
            // Create a payment using a APIContext
            Payment p;
            try
            {
                p = payment.Create(apiContext);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return p;
        }

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            try
            {
                var paymentExecution = new PaymentExecution() { payer_id = payerId };
                this.payment = new Payment() { id = paymentId };
                return this.payment.Execute(apiContext, paymentExecution);
            }
            catch(PayPal.Exception.PayPalException ex)
            {
                throw ex;
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
