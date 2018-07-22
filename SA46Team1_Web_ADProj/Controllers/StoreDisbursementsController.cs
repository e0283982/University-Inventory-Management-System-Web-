using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Store/StoreDisbursements")]
    public class StoreDisbursementsController : Controller
    {
        [Route("Disbursement")]
        public ActionResult Disbursement()
        {
            if (Session["DisbursementListPage"].ToString() == "1")
            {
                @Session["BackToDisbursementList"] = "false";
                return View("Disbursement");
            }
            else
            {
                Session["DisbursementListPage"] = "1";
                return View("Disbursement2");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplayDisbursementDetails()
        {
            Session["DisbursementListPage"] = "2";
            return RedirectToAction("Disbursements", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToDisbursementList()
        {
            Session["DisbursementListPage"] = "1";
            Session["BackToDisbursementList"] = "true";

            return RedirectToAction("Disbursements", "Store");
        }


        [Route("Requisition")]
        public ActionResult Requisition()
        {
            //Session["DisbursementListPage"] = "0";

            if (Session["ReqListPage"].ToString() == "1")
            {
                return View("Requisition");
            }
            else
            {
                Session["ReqListPage"] = "1";
                return View("Requisition2");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplayReqDetails()
        {
            Session["ReqListPage"] = "2";
            return RedirectToAction("Disbursements", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToRequisitionsList()
        {
            Session["ReqListPage"] = "1";

            return RedirectToAction("Disbursements", "Store");
        }

        [Route("Retrieval")]
        public ActionResult Retrieval()
        {
            //Use disbursementlistpage equals to 1 to wire the tab
            Session["RetrievalListPage"] = "1";

            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                int id = m.StockRetrievalHeaders.Count();
                string reqId = "StoR-" + id;
                Session["RetrievalId"] = "StoR-" + id;
                ViewBag.IdCount = id;
                ViewBag.Disbursed = m.StockRetrievalHeaders.Where(x => x.ID == reqId).First().Disbursed;                
            }

            var tuple = new Tuple<StockRetrievalDetail, Item>(new StockRetrievalDetail(), new Item());

            return View(tuple);
        }

        //New Stuff
        [HttpPost]
        public RedirectToRouteResult DisburseItems()
        {
            Session["RetrievalListPage"] = "2";

            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                string id = (string) Session["RetrievalId"];
                StockRetrievalHeader srh = m.StockRetrievalHeaders.Where(x => x.ID == id).First();
                srh.Disbursed = 1;

                //Creating new disbursement
                DisbursementHeader newDH = new DisbursementHeader();

                int count = m.DisbursementHeaders.Count() + 1;
                string disId = "DH-" + count;
                newDH.Id = disId;

                newDH.Status = "Pending";

                //newDH.RequisitionFormID = srh.RequisitionFormID;
                //To Change
                newDH.RequisitionFormID = "Test123";

                DateTime localDate = DateTime.Now;
                newDH.Date = localDate;

                //newDH.DepartmentCode = m.StaffRequisitionHeaders.Where(x => x.FormID == srh.RequisitionFormID).FirstOrDefault().DepartmentCode;
                //To Change
                newDH.DepartmentCode = "COMM";

                newDH.CollectionPointID = m.DepartmentDetails.Where(x => x.DepartmentCode == newDH.DepartmentCode).FirstOrDefault().CollectionPointID;
                newDH.RepresentativeID = m.DepartmentDetails.Where(x => x.DepartmentCode == newDH.DepartmentCode).FirstOrDefault().RepresentativeID;

                //Temporary
                newDH.Amount = 100;

                m.DisbursementHeaders.Add(newDH);

                m.SaveChanges();

            }

            




            return RedirectToAction("Disbursements", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult AdjustItem(StockRetrievalDetail item1, Item item2)
        {
            Session["RetrievalListPage"] = "2";

            string itemCode;

            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;

                itemCode = m.Items.Where(x => x.Description == item2.Description).FirstOrDefault().ItemCode;

                StockRetrievalDetail srd = m.StockRetrievalDetails.Where(x => x.ItemCode == itemCode && x.Id == item1.Id).FirstOrDefault();

                int qtyAdjusted = item1.QuantityAdjusted;

                srd.QuantityAdjusted = srd.QuantityAdjusted + qtyAdjusted;
                srd.QuantityRetrieved = srd.QuantityRetrieved - qtyAdjusted;

                m.SaveChanges();

            }

            return RedirectToAction("Disbursements", "Store");
        }



    }
}
