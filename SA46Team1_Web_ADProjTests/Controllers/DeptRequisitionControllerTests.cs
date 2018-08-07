using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SA46Team1_Web_ADProj.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SA46Team1_Web_ADProj.Controllers.Tests
{
    [TestClass()]
    public class DeptRequisitionControllerTests
    {
        [TestMethod()]
        public void NewReqTest()
        {
            var controller = new DeptRequisitionController();
            using (Models.SSISdbEntities e = new Models.SSISdbEntities())
            {
                Models.Item item1 = e.Items.Where(x => x.Description == "File Separator").FirstOrDefault();
                Models.Item item2 = e.Items.Where(x => x.Description == "Highlighter Blue").FirstOrDefault();
                Models.Item item3 = e.Items.Where(x => x.Description == "Pen Ballpoint Black").FirstOrDefault();

                Models.StaffRequisitionDetail srd1 = new Models.StaffRequisitionDetail();
                srd1.QuantityOrdered = 20;
                Models.StaffRequisitionDetail srd2 = new Models.StaffRequisitionDetail();
                srd2.QuantityOrdered = 10;
                Models.StaffRequisitionDetail srd3 = new Models.StaffRequisitionDetail();
                srd3.QuantityOrdered = 5;
                
                List<Models.StaffRequisitionDetail> list = new List<Models.StaffRequisitionDetail>();
                var itemCode = new NameValueCollection { { "SelectItemDesc", "F020" } };

                var context = new Mock<HttpContextBase>();
                var mockControllerContext = new Mock<ControllerContext>();
                var mockHttpContext = new Mock<HttpContextBase>();
                var controllerContext = new Mock<ControllerContext>();
               
                var request = new Mock<HttpRequestBase>();

                request.SetupGet(x => x.Headers).Returns(
                    new System.Net.WebHeaderCollection {
                        {"X-Requested-With", "XMLHttpRequest"}
                    });
                request.Setup(r => r.Form).Returns(itemCode);
                context.SetupGet(x => x.Request).Returns(request.Object);


                mockControllerContext.Setup(c => c.HttpContext).Returns(mockHttpContext.Object);
                controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
                
                var result1 = controller.AddNewReqItem(item1, srd1);
                
                Assert.IsNotNull(result1);
            }


        }
    }
}

public class MockHttpSession : HttpSessionStateBase
{
    Dictionary<string, object> m_SessionStorage = new Dictionary<string, object>();

    public override object this[string name]
    {
        get { return m_SessionStorage[name]; }
        set { m_SessionStorage[name] = value; }
    }
}