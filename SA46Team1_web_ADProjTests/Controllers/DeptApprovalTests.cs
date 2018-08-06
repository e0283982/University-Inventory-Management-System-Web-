using Microsoft.VisualStudio.TestTools.UnitTesting;
using SA46Team1_Web_ADProj.Controllers;
using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SA46Team1_Web_ADProj.Controllers.Tests
{
    [TestClass()]
    public class DeptApprovalTests
    {
        [TestMethod()]
        public void ApprovalTest()
        {
            // Display Approval Page
            HttpContext.Current = SA46Team1_Web_ADProjTests.MockSession.FakeHttpContext();

            var wrapper = new HttpContextWrapper(HttpContext.Current);

            DeptApprovalController controller = new DeptApprovalController();
            HttpContext.Current.Session["ReqApprovalPage"] = "1";
            controller.ControllerContext = new ControllerContext(wrapper, new RouteData(), controller);

            var results = controller.Approval() as ViewResult;

            Assert.AreEqual("Approval", results.ViewName);
        }

        [TestMethod()]
        public void DisplayApprovalDetailsTest()
        {
            //Session["ReqApprovalPage"] = "2";
            //Session["ReviewNewRequisitionId"] = ReqFormId;

            //using (SSISdbEntities e = new SSISdbEntities())
            //{
            //    DAL.StaffRequisitionRepositoryImpl dal = new DAL.StaffRequisitionRepositoryImpl(e);
            //    StaffRequisitionHeader srh = dal.GetStaffRequisitionHeaderById(ReqFormId);
            //    TempData["RequisitionRequstor"] = e.Employees.Where(x => x.EmployeeID == srh.EmployeeID).Select(x => x.EmployeeName).First();
            //    TempData["RequisitionDateReq"] = srh.DateRequested;
            //}

            //return RedirectToAction("Approval", "Dept");

            HttpContext.Current = SA46Team1_Web_ADProjTests.MockSession.FakeHttpContext();

            var wrapper = new HttpContextWrapper(HttpContext.Current);

            DeptApprovalController controller = new DeptApprovalController();
            HttpContext.Current.Session["ReqApprovalPage"] = "2";
            HttpContext.Current.Session["ReviewNewRequisitionId"] = "SR-077";
            controller.ControllerContext = new ControllerContext(wrapper, new RouteData(), controller);

            var results = controller.DisplayApprovalDetails("SR-077") as RedirectToRouteResult;

            Assert.IsTrue(results.RouteValues.ContainsKey("action"));
            Assert.IsTrue(results.RouteValues.ContainsKey("controller"));
            Assert.AreEqual("Approval", results.RouteValues["action"].ToString());
            Assert.AreEqual("Dept", results.RouteValues["controller"].ToString());

        }

        [TestMethod()]
        public void BackToApprovalListTest()
        {
            HttpContext.Current = SA46Team1_Web_ADProjTests.MockSession.FakeHttpContext();

            var wrapper = new HttpContextWrapper(HttpContext.Current);

            DeptApprovalController controller = new DeptApprovalController();
            HttpContext.Current.Session["ReqApprovalPage"] = "1";
            controller.ControllerContext = new ControllerContext(wrapper, new RouteData(), controller);

            var results = controller.BackToApprovalList() as RedirectToRouteResult;

            Assert.IsTrue(results.RouteValues.ContainsKey("action"));
            Assert.IsTrue(results.RouteValues.ContainsKey("controller"));
            Assert.AreEqual("Approval", results.RouteValues["action"].ToString());
            Assert.AreEqual("Dept", results.RouteValues["controller"].ToString());
        }

        [TestMethod()]
        public void ApproveTest()
        {
            HttpContext.Current = SA46Team1_Web_ADProjTests.MockSession.FakeHttpContext();

            var wrapper = new HttpContextWrapper(HttpContext.Current);

            DeptApprovalController controller = new DeptApprovalController();
            controller.ControllerContext = new ControllerContext(wrapper, new RouteData(), controller);
            HttpContext.Current.Session["ReviewNewRequisitionId"] = "SR-077";
            HttpContext.Current.Session["UserId"] = "E1";
            HttpContext.Current.Session["NoUnreadRequests"] = 10;
            var results = controller.Approve("remarks") as RedirectToRouteResult;


            using(SSISdbEntities m = new SSISdbEntities())
            {
                StaffRequisitionHeader srh = m.StaffRequisitionHeaders.Where(x => x.FormID == "SR-077").FirstOrDefault();
                Assert.AreEqual("Approved", srh.ApprovalStatus);
                
            }

            Assert.IsTrue(results.RouteValues.ContainsKey("action"));
            Assert.IsTrue(results.RouteValues.ContainsKey("controller"));
            Assert.AreEqual("Approval", results.RouteValues["action"].ToString());
            Assert.AreEqual("Dept", results.RouteValues["controller"].ToString());
        }

        [TestMethod()]
        public void RejectTest()
        {
            HttpContext.Current = SA46Team1_Web_ADProjTests.MockSession.FakeHttpContext();

            var wrapper = new HttpContextWrapper(HttpContext.Current);

            DeptApprovalController controller = new DeptApprovalController();
            controller.ControllerContext = new ControllerContext(wrapper, new RouteData(), controller);
            HttpContext.Current.Session["ReviewNewRequisitionId"] = "SR-077";
            HttpContext.Current.Session["UserId"] = "E1";
            HttpContext.Current.Session["NoUnreadRequests"] = 10;
            var results = controller.Reject("remarks") as RedirectToRouteResult;


            using (SSISdbEntities m = new SSISdbEntities())
            {
                StaffRequisitionHeader srh = m.StaffRequisitionHeaders.Where(x => x.FormID == "SR-077").FirstOrDefault();
                Assert.AreEqual("Rejected", srh.ApprovalStatus);

            }

            Assert.IsTrue(results.RouteValues.ContainsKey("action"));
            Assert.IsTrue(results.RouteValues.ContainsKey("controller"));
            Assert.AreEqual("Approval", results.RouteValues["action"].ToString());
            Assert.AreEqual("Dept", results.RouteValues["controller"].ToString());
        }
    }
}