using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void SubmitNewReqTest()
        {

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

                HttpContext.Current = SA46Team1_Web_ADProjTests.MockSession.FakeHttpContext();

                var wrapper = new HttpContextWrapper(HttpContext.Current);
                DeptRequisitionController controller = new DeptRequisitionController();
                HttpContext.Current.Session["DepartmentCode"] = "ZOOL";
                HttpContext.Current.Session["LoginEmployeeID"] = "E25";
                HttpContext.Current.Session["currentFormId"] = "SR-1001";
                HttpContext.Current.Session["newReqList"] = list;
                HttpContext.Current.Session["NoUnreadRequests"] = 10;
                HttpContext.Current.Session["tempList"] = new List<String>();
                HttpContext.Current.Session["EmpName"] = "Keith Ho";
                controller.ControllerContext = new ControllerContext(wrapper, new RouteData(), controller);
                
                var result1 = controller.SubmitNewRequestForm();
                
                Assert.IsNotNull(result1);

                DAL.StaffRequisitionRepositoryImpl dal = new DAL.StaffRequisitionRepositoryImpl(e);
                Models.StaffRequisitionHeader srh = dal.GetStaffRequisitionHeaderById("SR-1000");
                string emp = srh.EmployeeID;
                string deptCode = srh.DepartmentCode;

                Assert.AreEqual(emp, "E25");
                Assert.AreEqual(deptCode, "ZOOL");

                dal.DeleteStaffRequisitionHeader("SR-1001");
                e.SaveChanges();
            }


        }
    }
}

//public class MockHttpSession : HttpSessionStateBase
//{
//    Dictionary<string, object> m_SessionStorage = new Dictionary<string, object>();

//    public override object this[string name]
//    {
//        get { return m_SessionStorage[name]; }
//        set { m_SessionStorage[name] = value; }
//    }
//}