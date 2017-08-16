using System.Collections.Generic;

using audio_optio.Domain;

namespace audio_optio.Models
{
    public class OrdersModel
    {
        public List<Contact> contacts { get; set; }
        public List<Order> orders { get; set; }
    }
}