using System;
using System.Net.Mail;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using audio_optio.App_Data;

using audio_optio.Models;

namespace audio_optio.Controllers
{
    public class EmailController : Controller
    {
        private static string orderEmail_;
        private static string pw_;
        private static SmtpClient client_;

        /// <summary>
        /// Constructor 
        /// </summary>
        public EmailController()
        {
            orderEmail_ = Configuration.EmailCredentials.Email;
            pw_ = Configuration.EmailCredentials.Password;

            client_ = new SmtpClient("smtp.gmail.com", 587);
            client_.UseDefaultCredentials = false;
            client_.DeliveryMethod = SmtpDeliveryMethod.Network;
            client_.EnableSsl = true;
            client_.Credentials = new System.Net.NetworkCredential(orderEmail_, pw_);
        }

        /// <summary>
        /// Send contact notification
        /// </summary>
        /// <param name="model">ContactCommentModel contains contact information and comment</param>
        public void SendNotification(ContactCommentModel model)
        {
            // Send to/from business e-mail
            MailMessage mail = new MailMessage(orderEmail_, orderEmail_);
            // Blind copy contacter
            mail.Bcc.Add(new MailAddress(model.contact.Email));
            
            mail.Subject = "Thank you for contacting Audio-Optio";

            StringBuilder body = new StringBuilder();
            body.AppendLine("Thank you sincerely for visiting Audio-Optio. We appreciate your comments and questions.  Ryan will be in touch with you shortly!");
            body.AppendLine();
            body.AppendLine(string.Format("Submitted:\t{0}", model.comment.DateSubmitted.ToString()));
            body.AppendLine(string.Format("Name:\t{0} {1}", model.contact.FirstName, model.contact.LastName));
            body.AppendLine(string.Format("E-mail:\t{0}", model.contact.Email));
            body.AppendLine(string.Format("Phone:\t{0}", model.contact.Phone));
            body.AppendLine(string.Format("Comments:\t{0}", model.comment.Text));

            createFooter(ref body);

            mail.Body = body.ToString();

            try
            {
                client_.Send(mail);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Send order notification
        /// </summary>
        /// <param name="model"></param>
        public void SendNotification(ContactOrderModel model)
        {
            MailMessage mail = new MailMessage(orderEmail_, orderEmail_);
            mail.Bcc.Add(new MailAddress(model.contact.Email));
            mail.Subject = "Audio-Optio Order Confirmation";

            StringBuilder body = new StringBuilder();
            body.AppendLine(string.Format("Thank you for your order.  Please confirm your order details and contact {0} with any corrections.", orderEmail_));
            body.AppendLine();
            body.AppendLine(string.Format("Submitted:\t{0}", model.order.DateSubmitted.ToString()));
            body.AppendLine(string.Format("Name:\t{0} {1}", model.contact.FirstName, model.contact.LastName));
            body.AppendLine(string.Format("E-mail:\t{0}", model.contact.Email));
            body.AppendLine(string.Format("Phone:\t{0}", model.contact.Phone));
            body.AppendLine(string.Format("Youtube Link:\t{0}", model.order.YoutubeLink));
            body.AppendLine(string.Format("Comments:\t{0}", model.order.Comments));
            body.AppendLine(string.Format("Size:\t{0}", audio_optio.Domain.Order.GetCanvasDescription(model.order.Size)));

            createFooter(ref body);

            mail.Body = body.ToString();

            try
            {
                client_.Send(mail);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// Create footer
        /// </summary>
        /// <param name="sb">Body text as StringBuilder</param>
        private void createFooter(ref StringBuilder sb)
        {
            sb.AppendLine();
            sb.AppendLine("Audio-Optio");
            sb.AppendLine("www.audio-optio.science");
            sb.AppendLine(orderEmail_);
        }
    }

}