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

        private void SendOrderNotification(ContactOrderModel model)
        {
            MailMessage mail = new MailMessage(orderEmail, orderEmail);
            mail.Bcc.Add(new MailAddress(model.contact.Email.ToString()));
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential(orderEmail, pw);

            StringBuilder body = new StringBuilder();
            body.AppendLine(string.Format("Thank you for your order.  Please confirm your order details and contact {0} with any corrections.", orderEmail));
            body.AppendLine();
            body.AppendLine(string.Format("Submitted:\t{0}", model.order.DateSubmitted.ToString()));
            body.AppendLine(string.Format("Name:\t{0} {1}", model.contact.FirstName, model.contact.LastName));
            body.AppendLine(string.Format("E-mail:\t{0}", model.contact.Email));
            body.AppendLine(string.Format("Phone:\t{0}", model.contact.Phone));
            body.AppendLine(string.Format("Youtube Link:\t{0}", model.order.YoutubeLink));
            body.AppendLine(string.Format("Comments:\t{0}", model.order.Comments));
            body.AppendLine(string.Format("Size:\t{0}", audio_optio.Domain.Order.GetCanvasDescription(model.order.Size)));
            body.AppendLine();
            body.AppendLine("Audio-Optio");
            body.AppendLine("https://audio-optio.science");
            body.AppendLine(orderEmail);

            mail.Subject = "Audio-Optio Order Confirmation";
            mail.Body = body.ToString();

            try
            {
                client.Send(mail);
            }
            catch (Exception e)
            {
                throw e;
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
            //create an PayPalOrder for which you are taking payment
            //if you need to add more PayPalOrders in the list
            //Then you will need to create multiple PayPalOrder objects or use some loop to instantiate object
            Item item = new Item();
            item.name = String.Format("Audio Optio Canvas Art for {0} {1}", model.contactOrder.contact.FirstName, model.contactOrder.contact.LastName);
            item.currency = "USD";
            item.price = model.Price.ToString();
            item.quantity = "1";
            //item.sku = "sku";

            //Now make a List of Item and add the above item to it
            //you can create as many items as you want and add to this list
            ItemList itemList = new ItemList();
            itemList.items = new List<Item>();
            itemList.items.Add(item);

            //Address for the payment
            Address billingAddress = new Address();
            billingAddress = model.BillingAddress;

            Address shippingAddress = new Address();
            if (model.sameAddress)
            {
                shippingAddress = model.BillingAddress;
            }
            else
            {
                shippingAddress = model.ShippingAddress;
            }

            //Now Create an object of credit card and add above details to it
            //Please replace your credit card details over here which you got from paypal
            CreditCard crdtCard = new CreditCard();
            crdtCard.billing_address = billingAddress;
            crdtCard.cvv2 = "874";  //card cvv2 number
            crdtCard.expire_month = 1; //card expire date
            crdtCard.expire_year = 2020; //card expire year
            crdtCard.first_name = "Aman";
            crdtCard.last_name = "Thakur";
            crdtCard.number = "1234567890123456"; //enter your credit card number here
            crdtCard.type = "visa"; //credit card type here paypal allows 4 types

            // Specify details of your payment amount.
            Details details = new Details();
            details.shipping = "1";
            details.subtotal = "5";
            details.tax = "1";

            // Specify your total payment amount and assign the details object
            Amount amnt = new Amount();
            amnt.currency = "USD";
            // Total = shipping tax + subtotal.
            amnt.total = "7";
            amnt.details = details;

            // Now make a transaction object and assign the Amount object
            Transaction tran = new Transaction();
            tran.amount = amnt;
            tran.description = "Description about the payment amount.";
            tran.item_list = itemList;
            tran.invoice_number = "your invoice number which you are generating";

            // Now, we have to make a list of transaction and add the transactions object
            // to this list. You can create one or more object as per your requirements

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(tran);

            // Now we need to specify the FundingInstrument of the Payer
            // for credit card payments, set the CreditCard which we made above

            FundingInstrument fundInstrument = new FundingInstrument();
            fundInstrument.credit_card = crdtCard;

            // The Payment creation API requires a list of FundingIntrument

            List<FundingInstrument> fundingInstrumentList = new List<FundingInstrument>();
            fundingInstrumentList.Add(fundInstrument);

            // Now create Payer object and assign the fundinginstrument list to the object
            Payer payr = new Payer();
            payr.funding_instruments = fundingInstrumentList;
            payr.payment_method = "credit_card";

            // finally create the payment object and assign the payer object & transaction list to it
            Payment pymnt = new Payment();
            pymnt.intent = "sale";
            pymnt.payer = payr;
            pymnt.transactions = transactions;

            try
            {
                //getting context from the paypal
                //basically we are sending the clientID and clientSecret key in this function
                //to the get the context from the paypal API to make the payment
                //for which we have created the object above.

                //Basically, apiContext object has a accesstoken which is sent by the paypal
                //to authenticate the payment to facilitator account.
                //An access token could be an alphanumeric string

                APIContext apiContext = Configuration.GetAPIContext();

                //Create is a Payment class function which actually sends the payment details
                //to the paypal API for the payment. The function is passed with the ApiContext
                //which we received above.

                Payment createdPayment = pymnt.Create(apiContext);

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
            SendOrderNotification(cachedModel);

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

            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };

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

            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            // Create a payment using a APIContext
            Payment p;
            try
            {
                p = this.payment.Create(apiContext);
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
