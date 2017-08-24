using System;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Web.Mvc;


using audio_optio.Domain;
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

        private MailMessage CreateEmail(string subject, string to)
        {
            MailMessage mail = new MailMessage(orderEmail_, orderEmail_);
            mail.Bcc.Add(new MailAddress(to));
            mail.Subject = subject;
            mail.IsBodyHtml = true;

            return mail;
        }

        /// <summary>
        /// Send contact notification
        /// </summary>
        /// <param name="model">ContactCommentModel contains contact information and comment</param>
        public void SendNotification(ContactCommentModel model)
        {
            MailMessage mail = CreateEmail("Thank you for contacting Audio Optio", model.contact.Email);

            string body;
            // Read the file and display it line by line.
            using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"/Templates/CommentEmail.txt"))
            {
                body = sr.ReadToEnd();
            }

            body = Regex.Replace(body, @"\t|\n|\r", "");
            mail.Body = string.Format(body, model.contact.FirstName, model.comment.Text, AppDomain.CurrentDomain.BaseDirectory + "Images\\");
            
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
            MailMessage mail = CreateEmail("Thank you for contacting Audio Optio", model.contact.Email);

            string body;
            // Read the file and display it line by line.
            using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"/Templates/OrderEmail.txt"))
            {
                body = sr.ReadToEnd();
            }

            body = Regex.Replace(body, @"\t|\n|\r", "");
            StringBuilder addressSb = new StringBuilder();
            
            mail.Body = string.Format(body, model.contact.FirstName, 
                model.order.DateSubmitted,
                model.contact.FirstName + " " + model.contact.LastName,
                model.contact.Email,
                model.contact.Phone,
                model.order.YoutubeLink,
                model.order.Comments,
                Order.GetCanvasDescription(model.order.Size),
                
                AppDomain.CurrentDomain.BaseDirectory + "Images\\");

            try
            {
                client_.Send(mail);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }

}