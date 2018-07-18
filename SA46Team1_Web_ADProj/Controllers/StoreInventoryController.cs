using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Store/StoreInventory")]
    public class StoreInventoryController : Controller
    {
        [Route("Overview")]
        public ActionResult Overview()
        {
            return View();
        }

        [Route("Reorder")]
        public ActionResult Reorder()
        {
            return View();
        }

        [Route("StockAdj")]
        public ActionResult StockAdj()
        {
            if (Session["StockAdjPage"].ToString() == "1")
            {
                return View("StockAdj");
            }
            else
            {
                if(TempData["ItemList"] == null)
                {
                    List<StockAdjustmentDetail> sadList = new List<StockAdjustmentDetail>();
                    StockAdjustmentDetail sad = new StockAdjustmentDetail();
                    sad.ItemCode = "C001";
                    sad.RequestId = 7;
                    sad.ItemQuantity = 100;
                    sad.Amount = 100;
                    sad.Remarks = "Damaged";
                    sad.Reason = "nil";
                    sadList.Add(sad);

                    sad.StockAdjustmentDetails = sadList;

                    Session["StockAdjPage"] = "1";
                    return View("StockAdj2", sad);
                }
                else
                {

                    StockAdjustmentDetail sad = TempData["ItemList"] as StockAdjustmentDetail;

                    Session["StockAdjPage"] = "1";
                    return View("StockAdj2", sad);
                }

                

                
                
            }
        }

        [HttpPost]
        public RedirectToRouteResult CreateNewStockAdj()
        {                        
            Session["StockAdjPage"] = "2";            
            return RedirectToAction("Inventory", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult AddNewItem(StockAdjustmentDetail stockAdjustmentDetail)
        {
            StockAdjustmentDetail sad = new StockAdjustmentDetail();
            sad.ItemCode = "C002";
            sad.RequestId = 1;
            sad.ItemQuantity = 1100;
            sad.Amount = 1010;
            sad.Remarks = "Damaged";
            sad.Reason = "nil";

            stockAdjustmentDetail.StockAdjustmentDetails.Add(sad);

            TempData["ItemList"] = stockAdjustmentDetail;

            Session["StockAdjPage"] = "2";
            return RedirectToAction("Inventory", "Store");
        }


        [HttpPost]
        public RedirectToRouteResult SubmitNewStockAdj(StockAdjustmentDetail stockAdjustmentDetail)
        {
            StockAdjustmentHeader sah = new StockAdjustmentHeader();
            sah.DateRequested = DateTime.Now;
            sah.Requestor = "E1";
            sah.Status = "Pending";
            sah.TransactionType = "Stock Adjustment";

            using(SSISdbEntities m = new SSISdbEntities())
            {
                m.StockAdjustmentHeaders.Add(sah);
                m.SaveChanges();
            }

            using (SSISdbEntities m = new SSISdbEntities())
            {
                foreach(StockAdjustmentDetail sad in stockAdjustmentDetail.StockAdjustmentDetails)
                {
                    StockAdjustmentDetail sadDb = new StockAdjustmentDetail();
                    sadDb.RequestId = sah.RequestId;
                    sadDb.ItemCode = sad.ItemCode;
                    sadDb.ItemQuantity = sad.ItemQuantity;
                    sadDb.Remarks = sad.Remarks;
                    sadDb.Reason = sad.Reason;
                    //Temporary
                    sadDb.Amount = 1000;

                    m.StockAdjustmentDetails.Add(sadDb);
                    m.SaveChanges();

                }

            }

            return RedirectToAction("Inventory", "Store");
        }


        [Route("StockTake")]
        public ActionResult StockTake()
        {
            return View();
        }
        
    }
}
