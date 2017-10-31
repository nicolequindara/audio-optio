
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace audio_optio.Domain
{
    public class Order
    {

        public Order()
        {
            OrderStatus = Status.Submitted;

            DateTime now = DateTime.Now;
            DateCompleted = now;
            DateSubmitted = now;
            DatePending = now;
        }

        public enum Status
        {
            Submitted,
            Pending,
            Completed
        }

        public static string GetDescription(CanvasSize size)
        {
            switch (size)
            {
                case CanvasSize.Digital_Image:
                    return "Digital Image";
                case CanvasSize.Twelve_by_Sixteen:
                    return "12\" x 16\"";
                case CanvasSize.Fourteen_by_Fourteen:
                    return "14\" x 14\"";
                case CanvasSize.Sixteen_by_Twenty:
                    return "16\" x 20\"";
                case CanvasSize.Eighteen_by_TwentyFour:
                    return "18\" x 24\"";
                case CanvasSize.Twenty_by_Thirty:
                    return "20\" x 30\"";
                case CanvasSize.TwentyFour_by_ThirtyTwo:
                    return "24\" x 32\"";
                case CanvasSize.Sixteen_by_FortyEight:
                    return "16\" x 48\"";
                case CanvasSize.Thirty_by_Forty:
                    return "30\" x 40\"";
                case CanvasSize.Forty_by_Sixty:
                    return "40\" x 60\"";
                default:
                    return "Unrecognized Size";
            }

        }


        public enum CanvasSize
        {
            [Description("Digital Image")]
            Digital_Image,
            [Description("12\" by 16\"")]
            Twelve_by_Sixteen,
            [Description("14\" by 14\"")]
            Fourteen_by_Fourteen,
            [Description("16\" by 20\"")]
            Sixteen_by_Twenty,
            [Description("18\" by 24\"")]
            Eighteen_by_TwentyFour,
            [Description("20\" by 30\"")]
            Twenty_by_Thirty,
            [Description("24\" by 32\"")]
            TwentyFour_by_ThirtyTwo,
            [Description("16\" by 48\"")]
            Sixteen_by_FortyEight,
            [Description("30\" by 40\"")]
            Thirty_by_Forty,
            [Description("40\" by 60\"")]
            Forty_by_Sixty
        }

        [Key]
        public int Id { get; set; }

        public DateTime DateSubmitted { get; set; }
        public DateTime DatePending { get; set; }

        public DateTime DateCompleted { get; set; }
        
        public Contact Contact { get; set; }

        [Required]
        public string Song { get; set; }

        public string Comments { get; set; }

        public Status OrderStatus { get; set; }
        
        public string DiscountCode { get; set; }

        [Required]
        public CanvasSize Size { get; set; }

        public static decimal GetPrice(Order.CanvasSize size)
        {
            decimal price;

            switch (size)
            {
                case CanvasSize.Digital_Image:
                    price = 50m;
                    break;
                case CanvasSize.Twelve_by_Sixteen:
                    price = 102m;
                    break;
                case CanvasSize.Fourteen_by_Fourteen:
                    price = 104m;
                    break;
                case CanvasSize.Sixteen_by_Twenty:
                    price = 166m;
                    break;
                case CanvasSize.Eighteen_by_TwentyFour:
                    price = 194m;
                    break;
                case CanvasSize.Twenty_by_Thirty:
                    price = 294m;
                    break;
                case CanvasSize.TwentyFour_by_ThirtyTwo:
                    price = 363m;
                    break;
                case CanvasSize.Sixteen_by_FortyEight:
                    price = 363m;
                    break;
                case CanvasSize.Thirty_by_Forty:
                    price = 516m;
                    break;
                case CanvasSize.Forty_by_Sixty:
                    price = 744m;
                    break;
                default:
                    throw new ArgumentException("Unrecognized size");
            }

            return price;
        }
    }
}