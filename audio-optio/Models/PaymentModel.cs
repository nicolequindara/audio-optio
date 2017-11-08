using System;
using PayPal.Api;
using System.Collections.Generic;

namespace audio_optio.Models
{
    public class PaymentModel
    {
        public enum CardType
        {
            Visa,
            MasterCard,
            Discover,
            AmEx
        }

        public enum Month
        {
            Jan,
            Feb,
            Mar,
            Apr,
            May,
            Jun,
            Jul,
            Aug,
            Sept,
            Oct,
            Nov
        }

        public enum State
        {
            AL,
            AK,
            AZ,
            AR,
            CA,
            CO,
            CT,
            DE,
            FL,
            GA,
            HI,
            ID,
            IL,
            IA,
            KS,
            KY,
            LA,
            ME,
            MD,
            MA,
            MI,
            MN,
            MS,
            MO,
            MT,
            NE,
            NV,
            NH,
            NJ,
            NM,
            NY,
            NC,
            ND,
            OH,
            OK,
            OR,
            PA,
            RI,
            SC,
            SD,
            TN,
            TX,
            UT,
            VT,
            VA,
            WA,
            WV,
            WI,
            WY
        }

        public bool payPaypal { get; set; }
        public bool sameAddress { get; set; }
        public bool success { get; set; }
        public bool discountApplied { get; set; }

        public ContactOrderModel contactOrder { get; set; }

        public CreditCard CreditCard { get; set; }

        public decimal Price { get; set; }

        
        public PaymentModel()
        {
            discountApplied = false;
            payPaypal = true;
            sameAddress = false;
            success = false;
            contactOrder = new ContactOrderModel();
            CreditCard = new CreditCard();
        }

        public void Format()
        {
            if(sameAddress)
            {
                contactOrder.order.ShippingAddress.To = !String.IsNullOrEmpty(contactOrder.order.BillingAddress.To) ? contactOrder.order.BillingAddress.To : "";
                contactOrder.order.ShippingAddress.AddressLine1 = !String.IsNullOrEmpty(contactOrder.order.BillingAddress.AddressLine1) ? contactOrder.order.BillingAddress.AddressLine1 : "";
                contactOrder.order.ShippingAddress.AddressLine2 = !String.IsNullOrEmpty(contactOrder.order.BillingAddress.AddressLine2) ? contactOrder.order.BillingAddress.AddressLine2 : "";
                contactOrder.order.ShippingAddress.City = !String.IsNullOrEmpty(contactOrder.order.BillingAddress.City) ? contactOrder.order.BillingAddress.City : "";
                contactOrder.order.ShippingAddress.State = !String.IsNullOrEmpty(contactOrder.order.BillingAddress.State) ? contactOrder.order.BillingAddress.State : "";
                contactOrder.order.ShippingAddress.PostalCode = !String.IsNullOrEmpty(contactOrder.order.BillingAddress.PostalCode) ? contactOrder.order.BillingAddress.PostalCode : "";
            }
        }

        public List<string> Validate()
        {
            List<string> errors = new List<string>();

            if (String.IsNullOrEmpty(contactOrder.order.BillingAddress.AddressLine1))
            {
                errors.Add("Billing address missing street address.");
            }

            if (String.IsNullOrEmpty(contactOrder.order.BillingAddress.City))
            {
                errors.Add("Billing address missing city.");
            }

            if (String.IsNullOrEmpty(contactOrder.order.BillingAddress.State))
            {
                errors.Add("Billing address missing state.");
            }

            if (String.IsNullOrEmpty(contactOrder.order.BillingAddress.PostalCode))
            {

                errors.Add("Billing address missing postal code.");
            }

            if (String.IsNullOrEmpty(contactOrder.order.ShippingAddress.AddressLine1))
            {
                errors.Add("Shipping address missing street address.");
            }

            if (String.IsNullOrEmpty(contactOrder.order.ShippingAddress.City))
            {
                errors.Add("Shipping address missing city.");
            }

            if (String.IsNullOrEmpty(contactOrder.order.ShippingAddress.State))
            {
                errors.Add("Shipping address missing state.");
            }

            if (String.IsNullOrEmpty(contactOrder.order.ShippingAddress.PostalCode))
            {

                errors.Add("Shipping address missing postal code.");
            }

            return errors;
        }
    }
}