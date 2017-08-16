using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using audio_optio.Models;
using audio_optio.Domain;
using audio_optio.Database;
namespace audio_optio.Controllers
{
    public class DashboardController : Controller
    {
        ContactRepository contacts;
        OrderRepository orders;

        public DashboardController()
        {
            AoDbContext context = new AoDbContext();
            contacts = new ContactRepository(context);
            orders = new OrderRepository(context);
        }
        
        // GET: Dashboard
        public ActionResult Index()
        {
            List<Order> listOrders = orders.Get() as List<Order>;
            List<Contact> listContacts = contacts.Get() as List<Contact>;

            OrdersModel model = new OrdersModel { orders = listOrders, contacts = listContacts };

            return View(model);
        }

        // GET: Dashboard/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Dashboard/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Dashboard/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Dashboard/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Dashboard/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Dashboard/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
