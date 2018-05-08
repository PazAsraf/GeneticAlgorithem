using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeneticAlgorithem;
using GeneticAlgorithem.Controllers;

namespace GeneticAlgorithem.Tests.Controllers
{
    [TestClass]
    public class ValuesControllerTest
    {
        [TestMethod]
        public void GetById()
        {
            // Arrange
            GAController controller = new GAController();

            // Act
            string result = controller.Get("5");

            // Assert
            Assert.AreEqual("value", result);
        }

        [TestMethod]
        public void Post()
        {
            // Arrange
            GAController controller = new GAController();

            // Act
            controller.Post("value");

            // Assert
        }

        [TestMethod]
        public void Put()
        {
            // Arrange
            GAController controller = new GAController();

            // Act
            controller.Put(5, "value");

            // Assert
        }

        [TestMethod]
        public void Delete()
        {
            // Arrange
            GAController controller = new GAController();

            // Act
            controller.Delete(5);

            // Assert
        }
    }
}
