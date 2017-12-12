using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using audio_optio.Domain;

namespace audio_optio.test
{
    [TestClass]
    public class OrderTest
    {
        /// <summary>
        /// Declaring constructor object should set OrderStatus, DateCompleted, DateSubmitted, DatePending Fields
        /// </summary>
        [TestMethod]
        public void Constructor()
        {
            // Act
            Order order = new Order();

            // Assert
            Assert.IsNotNull(order.DateCompleted);
            Assert.IsNotNull(order.DateSubmitted);
            Assert.IsNotNull(order.DatePending);

            // Equal by transitive property
            Assert.AreEqual(order.DateCompleted, order.DateSubmitted);
            Assert.AreEqual(order.DateCompleted, order.DatePending);

            Assert.AreEqual(Order.Status.Submitted, order.OrderStatus);
            Assert.AreEqual(Order.CanvasSize.Digital_Image, order.Size);
            Assert.IsNull(order.Comments);
            Assert.IsNull(order.Contact);
            Assert.IsNull(order.DiscountCode);
            Assert.AreEqual(0, order.Id);
        }

        [TestMethod]
        public void RequiredSongAttribute()
        {             
            // Arrange
            var propertyInfo = typeof(Order).GetProperty("Song");

            // Act
            var attributes = propertyInfo.GetCustomAttributes(typeof(RequiredAttribute), true);

            // Assert
            Assert.AreEqual(attributes.Length, 1);
            Assert.IsInstanceOfType(attributes.FirstOrDefault(), typeof(RequiredAttribute));         
        }

        [TestMethod]
        public void RequiredCanvasSizeAttribute()
        {
            // Arrange
            var propertyInfo = typeof(Order).GetProperty("Size");

            // Act
            var attributes = propertyInfo.GetCustomAttributes(typeof(RequiredAttribute), true);

            // Assert
            Assert.AreEqual(attributes.Length, 1);
            Assert.IsInstanceOfType(attributes.FirstOrDefault(), typeof(RequiredAttribute));
        }

        [TestMethod]
        public void KeyIdAttribute()
        {
            // Arrange
            var propertyInfo = typeof(Order).GetProperty("Id");

            // Act
            var attributes = propertyInfo.GetCustomAttributes(typeof(KeyAttribute), true);

            // Assert
            Assert.AreEqual(attributes.Length, 1);
            Assert.IsInstanceOfType(attributes.FirstOrDefault(), typeof(KeyAttribute));
        }

        /// <summary>
        /// Strings returned by Canvas.GetDescription method
        /// </summary>
        [TestMethod]
        public void GetDescriptionKnownCanvasSize()
        {
            // Arrange
            string description = null;

            // Act
            description = Order.GetDescription((Order.CanvasSize)0);
            // Assert
            Assert.AreEqual("Digital Image", description);

            // Act
            description = Order.GetDescription((Order.CanvasSize)1);
            // Assert
            Assert.AreEqual("12\" x 16\"", description);

            // Act
            description = Order.GetDescription((Order.CanvasSize)2);
            // Assert
            Assert.AreEqual("14\" x 14\"", description);
            
            // Act
            description = Order.GetDescription((Order.CanvasSize)3);
            // Assert
            Assert.AreEqual("16\" x 20\"", description);

            // Act
            description = Order.GetDescription((Order.CanvasSize)4);
            // Assert
            Assert.AreEqual("18\" x 24\"", description);

            // Act
            description = Order.GetDescription((Order.CanvasSize)5);
            // Assert
            Assert.AreEqual("20\" x 30\"", description);

            // Act
            description = Order.GetDescription((Order.CanvasSize)6);
            // Assert
            Assert.AreEqual("24\" x 32\"", description);

            // Act
            description = Order.GetDescription((Order.CanvasSize)7);
            // Assert
            Assert.AreEqual("16\" x 48\"", description);

            // Act
            description = Order.GetDescription((Order.CanvasSize)8);
            // Assert
            Assert.AreEqual("30\" x 40\"", description);

            // Act
            description = Order.GetDescription((Order.CanvasSize)9);
            // Assert
            Assert.AreEqual("40\" x 60\"", description);


        }

        /// <summary>
        /// Strings returned by Canvas.GetDescription method with input parameter out of range
        /// </summary>
        [TestMethod]
        public void GetDescriptionUnknownCanvasSize()
        {
            // Arrange
            string description = null;

            Random r = new Random();
            
            int IndexOutOfBounds = r.Next(Enum.GetNames(typeof(Order.CanvasSize)).Length, 100);

            // Act
            description = Order.GetDescription((Order.CanvasSize)IndexOutOfBounds);
            // Assert
            Assert.AreEqual("Unrecognized Size", description);
        }

        /// <summary>
        /// Prices returned by Canvas.GetPrice method
        /// </summary>
        [TestMethod]
        public void GetPriceKnownCanvasSize()
        {
            // Arrange
            decimal Price;

            // Act
            Price = Order.GetPrice((Order.CanvasSize)0);
            // Assert
            Assert.AreEqual(72m, Price);

            // Act
            Price = Order.GetPrice((Order.CanvasSize)1);
            // Assert
            Assert.AreEqual(72m, Price);

            // Act
            Price = Order.GetPrice((Order.CanvasSize)2);
            // Assert
            Assert.AreEqual(73m, Price);

            // Act
            Price = Order.GetPrice((Order.CanvasSize)3);
            // Assert
            Assert.AreEqual(110m, Price);

            // Act
            Price = Order.GetPrice((Order.CanvasSize)4);
            // Assert
            Assert.AreEqual(143m, Price);

            // Act
            Price = Order.GetPrice((Order.CanvasSize)5);
            // Assert
            Assert.AreEqual(193m, Price);

            // Act
            Price = Order.GetPrice((Order.CanvasSize)6);
            // Assert
            Assert.AreEqual(243m, Price);

            // Act
            Price = Order.GetPrice((Order.CanvasSize)7);
            // Assert
            Assert.AreEqual(243, Price);

            // Act
            Price = Order.GetPrice((Order.CanvasSize)8);
            // Assert
            Assert.AreEqual(369m, Price);

            // Act
            Price = Order.GetPrice((Order.CanvasSize)9);
            // Assert
            Assert.AreEqual(523m, Price);


        }

        /// <summary>
        /// Prices returned by Canvas.GetPrice method with input parameter out of range
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Unrecognized size")]
        public void GetPriceUnknownCanvasSize()
        {
            // Arrange
            Random r = new Random();
            int IndexOutOfBounds = r.Next(Enum.GetNames(typeof(Order.CanvasSize)).Length, 100);

            // Act
            decimal Price = Order.GetPrice((Order.CanvasSize)IndexOutOfBounds);
            
        }
    }
}
