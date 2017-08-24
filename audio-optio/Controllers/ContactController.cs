using System;
using System.Web.Mvc;

using audio_optio.Domain;
using audio_optio.Models;
using audio_optio.Database;
namespace audio_optio.Controllers
{
    public class ContactController : Controller
    {
        private CommentRepository comments;
        private ContactRepository contacts;

        public ContactController()
        {
            AoDbContext context = new AoDbContext();
            comments = new CommentRepository(context);
            contacts = new ContactRepository(context);
        }

        public ActionResult Contact()
        {
            ContactCommentModel m = new ContactCommentModel();

            return View(m);
        }

        [HttpPost]
        public ActionResult Create(ContactCommentModel model)
        {
            model.success = false;

            try
            {
                if (ModelState.IsValid)
                {
                    model.contact.Format();

                    Contact c = contacts.Get(model.contact.FirstName, model.contact.LastName);
                    if (c == null)
                    {
                        c = model.contact;
                        contacts.Insert(c);
                    }

                    contacts.Update(c);

                    model.comment.Contact = c;
                    comments.Insert(model.comment);

                    // Send notification
                    try
                    {
                        new EmailController().SendNotification(model);
                    }
                    catch(Exception e)
                    {
                        ModelState.AddModelError("E-mail Error", e.Message);
                    }

                    model.success = true;
                    return Update(model);
                }
                else
                {
                    return Update(model);
                }
            }
            catch(Exception e)
            {
                model.success = false;
                return Update(model);
            }
        }

        public ActionResult Update(ContactCommentModel model)
        {
            return View("Contact", model);
        }


        // GET: Order/Details/5
        public ActionResult Details(int? Id)
        {
            if (Id == null)
            {
                return View("Error");
            }

            int id = Id ?? default(int);

            Comment comment = comments.Get(id);

            return View(comment);
        }

    }
}