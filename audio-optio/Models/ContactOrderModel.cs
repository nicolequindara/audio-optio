using System;
using System.Collections.Generic;
using System.Web.Mvc;
using audio_optio.Domain;

namespace audio_optio.Models
{
    public class ContactOrderModel
    {
        public Contact contact { get; set; }
        public Order order { get; set; }
        public bool success = false;

        public SelectList dropDownLabels = new SelectList(labels, "Key", "Value");
        private static Dictionary<Order.CanvasSize, string> labels = new Dictionary<Order.CanvasSize, string>();

        public static float getPrice(Order.CanvasSize size)
        {
            float price = 559.98f;
            switch (size)
            {
                case Order.CanvasSize.Twelve_by_Sixteen:
                    price = 102f;
                    break;
                case Order.CanvasSize.Fourteen_by_Fourteen:
                    price = 104f;
                    break;
                case Order.CanvasSize.Sixteen_by_Twenty:
                    price = 166f;
                    break;
                case Order.CanvasSize.Eighteen_by_TwentyFour:
                    price = 194f;
                    break;
                case Order.CanvasSize.Twenty_by_Thirty:
                    price = 294f;
                    break;
                case Order.CanvasSize.TwentyFour_by_ThirtyTwo:
                    price = 363f;
                    break;
                case Order.CanvasSize.Sixteen_by_FortyEight:
                    price = 363f;
                    break;
                case Order.CanvasSize.Thirty_by_Forty:
                    price = 516f;
                    break;
                case Order.CanvasSize.Forty_by_Sixty:
                    price = 744f;
                    break;
                default:
                    break;
            }

            return price;
        }
        public ContactOrderModel()
        {
            if(labels.Count > 0)
            {
                return;
            }

            labels.Add(Order.CanvasSize.Twelve_by_Sixteen, "12 x 16 - $102.00");
            labels.Add(Order.CanvasSize.Fourteen_by_Fourteen, "14 x 14 - $104.00");
            labels.Add(Order.CanvasSize.Sixteen_by_Twenty, "16 x 20 - $166.00");
            labels.Add(Order.CanvasSize.Eighteen_by_TwentyFour, "18 x 24 - $194.00");
            labels.Add(Order.CanvasSize.Twenty_by_Thirty, "20 x 30 - $294.00");
            labels.Add(Order.CanvasSize.TwentyFour_by_ThirtyTwo, "24 x 32 - $363.00");
            labels.Add(Order.CanvasSize.Sixteen_by_FortyEight, "16 x 48 - $363.00");
            labels.Add(Order.CanvasSize.Thirty_by_Forty, "30 x 40 - $516.00");
            labels.Add(Order.CanvasSize.Forty_by_Sixty, "40 x 60 - $744.00");
        }


    }


}