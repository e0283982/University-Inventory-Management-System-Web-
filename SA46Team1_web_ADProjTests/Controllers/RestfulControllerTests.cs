using Microsoft.VisualStudio.TestTools.UnitTesting;
using SA46Team1_Web_ADProj.Controllers;
using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA46Team1_Web_ADProj.Controllers.Tests
{
    [TestClass()]
    public class RestfulControllerTests
    {
        [TestMethod()]
        public void GetShortItemListTest()
        {
            //using (SSISdbEntities e = new SSISdbEntities())
            //{
            //    e.Configuration.ProxyCreationEnabled = false;
            //}

            var controller = new RestfulController();
            var result = controller.GetShortItemList();
            Assert.IsNotNull(result);
        }
    }
}