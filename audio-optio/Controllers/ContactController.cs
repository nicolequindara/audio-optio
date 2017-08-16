using System;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;

using audio_optio.App_Data;
using audio_optio.Domain;
using audio_optio.Models;
using audio_optio.Database;
namespace audio_optio.Controllers
{
    public class ContactController : Controller
    {
        private CommentRepository comments;
        private ContactRepository contacts;
        private string orderEmail = Configuration.EmailCredentials.Email;
        private string pw = Configuration.EmailCredentials.Password;

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

                    model.success = true;

                    SendCommentNotifcation(model);

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

        private void SendCommentNotifcation(ContactCommentModel model)
        {
            MailMessage mail = new MailMessage(orderEmail, orderEmail);
            mail.Bcc.Add(new MailAddress(model.contact.Email.ToString()));
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential(orderEmail, pw);

            StringBuilder body = new StringBuilder();
            body.AppendLine("Thank you for reaching out to Audio-Optio.  Ryan will be in touch with you shortly!");
            body.AppendLine();
            body.AppendLine(string.Format("Submitted:\t{0}", model.comment.DateSubmitted.ToString()));
            body.AppendLine(string.Format("Name:\t{0} {1}", model.contact.FirstName, model.contact.LastName));
            body.AppendLine(string.Format("E-mail:\t{0}", model.contact.Email));
            body.AppendLine(string.Format("Phone:\t{0}", model.contact.Phone));
            body.AppendLine(string.Format("Comments:\t{0}", model.comment.Text));
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
    }
}