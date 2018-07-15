using System;
using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Web.Mvc;


using audio_optio.Domain;
using audio_optio.App_Data;
using audio_optio.Models;


namespace audio_optio.Services
{
    public interface IEmailService
    {
        void SendOrderEmail(PaymentModel model);
        void SendCommentEmail(ContactCommentModel model);
    }

    public class EmailService : IEmailService
    {
        private static string audioOptioEmail_;
        private static string pw_;
        private static SmtpClient client_;

        /// <summary>
        /// Constructor to configure email provider
        /// </summary>
        public EmailService()
        {
            audioOptioEmail_ = Configuration.EmailCredentials.Email;
            pw_ = Configuration.EmailCredentials.Password;

            client_ = new SmtpClient("smtp.gmail.com", 587);
            client_.UseDefaultCredentials = false;
            client_.DeliveryMethod = SmtpDeliveryMethod.Network;
            client_.EnableSsl = true;
            client_.Credentials = new System.Net.NetworkCredential(audioOptioEmail_, pw_);
        }

        /// <summary>
        /// Utility to create MailMessage object
        /// </summary>
        /// <param name="subject">Subject of e-mail</param>
        /// <param name="to">E-mail address of sender</param>
        /// <returns></returns>
        private MailMessage CreateEmail(string subject, string to)
        {
            MailMessage mail;

            try
            {
                mail = new MailMessage(audioOptioEmail_, audioOptioEmail_);
                mail.Bcc.Add(new MailAddress(to));
                mail.Subject = subject;
                mail.IsBodyHtml = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return mail;
        }

        private void SendMail(MailMessage mail)
        {
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
        /// Send contact notification
        /// </summary>
        /// <param name="model">ContactCommentModel contains contact information and comment</param>
        public void SendCommentEmail(ContactCommentModel model)
        {
            MailMessage mail = CreateEmail(Properties.Resources.ContactEmailSubject, model.contact.Email);

            string body;
            // Read the file and display it line by line.
            using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"/Templates/CommentEmail.txt"))
            {
                body = sr.ReadToEnd();
            }

            body = Regex.Replace(body, @"\t|\n|\r", "");
            mail.Body = string.Format(body, model.contact.FirstName, model.comment.Text);

            SendMail(mail);
        }

        /// <summary>
        /// Send order notification
        /// </summary>
        /// <param name="model">PaymentModel</param>
        public void SendOrderEmail(PaymentModel model)
        {
            Order order = model.contactOrder.order;
            Contact contact = model.contactOrder.contact;

            MailMessage mail = CreateEmail("", contact.Email);

            string body;
            // Read the file and display it line by line.
            using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"/Templates/OrderEmail.txt"))
            {
                body = sr.ReadToEnd();
            }

            body = Regex.Replace(body, @"\t|\n|\r", "");
            StringBuilder addressSb = new StringBuilder();


            string address = String.Format("{0}\n{1}\n{2}, {3} {4}",
                order.BillingAddress.To,
                string.IsNullOrEmpty(order.BillingAddress.AddressLine2) ? order.BillingAddress.AddressLine1 : string.Format("{0}\n{1}", order.BillingAddress.AddressLine1, order.BillingAddress.AddressLine2),
                order.BillingAddress.City,
                order.BillingAddress.State,
                order.BillingAddress.PostalCode);

            mail.Body = string.Format(body, contact.FirstName,
                order.DateSubmitted,
                contact.FirstName + " " + contact.LastName,
                contact.Email,
                contact.Phone,
                order.Song,
                order.Comments,
                Order.GetDescription(order.Size),
                model.Price.ToString("C", CultureInfo.CurrentCulture),
                address);

            SendMail(mail);
        }
    }
}