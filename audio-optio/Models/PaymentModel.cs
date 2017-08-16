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

        public ContactOrderModel contactOrder { get; set; }

        public Address ShippingAddress { get; set; }

        public Address BillingAddress { get; set; }

        public CreditCard CreditCard { get; set; }

        public float Price { get; set; }

        public PaymentModel()
        {
            payPaypal = true;
            sameAddress = false;
            success = false;
            contactOrder = new ContactOrderModel();
            ShippingAddress = new Address();
            BillingAddress = new Address();
            CreditCard = new CreditCard();
        }

        public void Format()
        {
            if(sameAddress)
            {
                ShippingAddress.line1 = String.IsNullOrEmpty(BillingAddress.line1) ? BillingAddress.line1 : "";
                ShippingAddress.line2 = String.IsNullOrEmpty(BillingAddress.line1) ? BillingAddress.line1 : "";
                ShippingAddress.city = String.IsNullOrEmpty(BillingAddress.city) ? BillingAddress.city : "";
                ShippingAddress.state = String.IsNullOrEmpty(BillingAddress.state) ? BillingAddress.state : "";
                ShippingAddress.postal_code = String.IsNullOrEmpty(BillingAddress.postal_code) ? BillingAddress.postal_code : "";
            }
        }

        public List<string> Validate()
        {
            List<string> errors = new List<string>();

            if (String.IsNullOrEmpty(BillingAddress.line1))
            {
                errors.Add("Billing address missing street address.");
            }

            if (String.IsNullOrEmpty(BillingAddress.city))
            {
                errors.Add("Billing address missing city.");
            }

            if (String.IsNullOrEmpty(BillingAddress.state))
            {
                errors.Add("Billing address missing state.");
            }

            if (String.IsNullOrEmpty(BillingAddress.postal_code))
            {

                errors.Add("Billing address missing postal code.");
            }

            if (sameAddress)
            {
                ShippingAddress = BillingAddress;
            }

            if (String.IsNullOrEmpty(ShippingAddress.line1))
            {
                errors.Add("Shipping address missing street address.");
            }

            if (String.IsNullOrEmpty(ShippingAddress.city))
            {
                errors.Add("Shipping address missing city.");
            }

            if (String.IsNullOrEmpty(ShippingAddress.state))
            {
                errors.Add("Shipping address missing state.");
            }

            if (String.IsNullOrEmpty(ShippingAddress.postal_code))
            {

                errors.Add("Shipping address missing postal code.");
            }

            if (!payPaypal)
            {
                if (String.IsNullOrEmpty(CreditCard.cvv2) || String.IsNullOrEmpty(CreditCard.number))
                {
                    errors.Add("Invalid credit card.");
                }
            }

            return errors;
        }
    }
}