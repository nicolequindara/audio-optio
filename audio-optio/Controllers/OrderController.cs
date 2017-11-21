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

using Address = audio_optio.Domain.Address;
using Order = audio_optio.Domain.Order;

namespace audio_optio.Controllers
{
    public class OrderController : Controller
    {
        private const string discountCode = "black20friday";
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


        public ActionResult Checkout()
        {
            ContactOrderModel m = new ContactOrderModel();

            return View(m);
        }

        // GET: Order
        public ActionResult Index()
        {
            return View(orders.Get() as List<Order>);
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

            if (ModelState.IsValid)
            {
                model.contact.Format();
                
                if (!String.IsNullOrEmpty(model.order.DiscountCode) && !model.order.DiscountCode.Equals(discountCode))
                {
                    model.success = false;
                    ModelState.AddModelError("Discount Code", "Discount code is invalid or expired.");
                    return View("Order", model);
                }

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
                model.success = false;
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
            m.Price = m.contactOrder == null ? 0.0m : audio_optio.Domain.Order.GetPrice(m.contactOrder.order.Size);
            
            // Evaluate discount code
            if (!String.IsNullOrEmpty(contactOrder.order.DiscountCode) && contactOrder.order.DiscountCode.ToLower().Equals(discountCode))
            {
                m.discountApplied = true;
                m.Price *= .8m;
            }

            m.contactOrder.order.OrderStatus = audio_optio.Domain.Order.Status.Pending;

            // Braintree gateway
            var gateway = Configuration.GetGateway();
            var clientToken = gateway.ClientToken.Generate();
            ViewBag.ClientToken = clientToken;
            
            return View("Pay", m);
        }
    }
}
