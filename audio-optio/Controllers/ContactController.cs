using System;
using System.Web.Mvc;

using audio_optio.Domain;
using audio_optio.Models;
using audio_optio.Database;
using audio_optio.Services;

namespace audio_optio.Controllers
{
    public class ContactController : Controller
    {
        private CommentRepository commentRepo_;
        private ContactRepository contactRepo_;
        private IEmailService emailService_;


        public ContactController(IEmailService emailService)
        {
            AoDbContext context = new AoDbContext();
            commentRepo_ = new CommentRepository(context);
            contactRepo_ = new ContactRepository(context);

            emailService_ = emailService;
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

            if (ModelState.IsValid)
            {
                model.contact.Format();

                Contact c = contactRepo_.Get(model.contact.FirstName, model.contact.LastName);
                if (c == null)
                {
                    c = model.contact;
                    contactRepo_.Insert(c);
                }

                contactRepo_.Update(c);

                model.comment.Contact = c;
                commentRepo_.Insert(model.comment);
                model.success = true;

                // Send notification
                try
                {
                    emailService_.SendCommentEmail(model);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("E-mail Error", e.Message);
                    model.success = false;
                }

                return Update(model);
            }
            else
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

            Comment comment = commentRepo_.Get(id);

            return View(comment);
        }

    }
}