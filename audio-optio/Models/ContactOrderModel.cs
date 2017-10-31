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

        
        public ContactOrderModel()
        {
            if (labels.Count > 0)
            {
                return;
            }

            foreach(Order.CanvasSize size in Enum.GetValues(typeof(Order.CanvasSize)))
            {
                labels.Add(size, string.Format("{0} - ${1}", Order.GetDescription(size), Order.GetPrice(size).ToString(".##")));
            }
        }
    }
}