using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.DAL;
using SA46Team1_Web_ADProj.Models;

//-----------------------------------------------------------------
//   Authors: Chirag Shetty
//-----------------------------------------------------------------

namespace SA46Team1_Web_ADProj.Controllers
{    
    [CustomAuthorize(Roles = "Store Manager, Store Supervisor")]
    [RoutePrefix("Store/StoreInventoryManager")]
    public class StoreManagerInventoryController : Controller
    {
        IStockAdjustmentDetailsRepository repo;
        public StoreManagerInventoryController()
        {
            repo = new StockAdjustmentDetailsRepositoryImpl(new SSISdbEntities());
        }
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

        [Route("ReviewAdj")]
        public ActionResult ReviewAdj()
        {
            return View();
        }

        [HttpPost]
        [Route("Approve")]
        public RedirectToRouteResult Approve(string requestid, string itemcode)
        {
            StockAdjustmentDetail stockadjdet = repo.GetStockAdjustmentDetailById(requestid, itemcode);
            stockadjdet.Status = "Approved";
            stockadjdet.NotificationStatus = "Unread";
            stockadjdet.StockAdjustmentHeader.DateProcessed = DateTime.Now;
            repo.UpdateStockAdjustmentDetail(stockadjdet);
            repo.Save();


            using(SSISdbEntities m = new SSISdbEntities())
            {
                StockAdjustmentHeader sjh = m.StockAdjustmentHeaders.Where(x => x.RequestId == requestid).FirstOrDefault();
                Employee emp = m.Employees.Where(x => x.EmployeeID == sjh.Requestor).FirstOrDefault();

                string title = "[LogicUniversity] Stock Adjustment Approved: " + requestid;
                string message = "Item Code: " + itemcode + " has been approved.";

                // Send to Employee
                CommonLogic.Email.sendEmail("stationerylogicuniversity@gmail.com", emp.EmployeeEmail, title, message);
            }
            
            Session["StockAdjPage"] = 2;
            Session["StoreInventoryTabIndex"] = "1";

            return RedirectToAction("Inventory", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult Reject(string requestid, string itemcode)
        {
            StockAdjustmentDetail stockadjdet = repo.GetStockAdjustmentDetailById(requestid, itemcode);
            using (IItemTransactionRepository itemRepo = new ItemTransactionRepositoryImpl(new SSISdbEntities()))
            {
                ItemTransaction itemTrans = new ItemTransaction
                {
                    DocumentRefNo = requestid,
                    TransDateTime = DateTime.Now,
                    ItemCode = itemcode,
                    TransactionType = "Reject Stock Adjustment",
                    Quantity = stockadjdet.ItemQuantity,
                    Amount = stockadjdet.Amount,
                    UnitCost = stockadjdet.Amount / stockadjdet.ItemQuantity
                };
                itemRepo.InsertItemTransaction(itemTrans);
                itemRepo.Save();
            }
            stockadjdet.Status = "Rejected";
            stockadjdet.NotificationStatus = "Unread";
            repo.UpdateStockAdjustmentDetail(stockadjdet);
            repo.Save();

            using (SSISdbEntities m = new SSISdbEntities())
            {
                StockAdjustmentHeader sjh = m.StockAdjustmentHeaders.Where(x => x.RequestId == requestid).FirstOrDefault();
                Employee emp = m.Employees.Where(x => x.EmployeeID == sjh.Requestor).FirstOrDefault();

                // Send to Employee
                string title = "[LogicUniversity] Stock Adjustment Rejected: " + requestid;
                string message = "Item Code: " + itemcode + " has been rejected.";

                // Send to Employee
                CommonLogic.Email.sendEmail("stationerylogicuniversity@gmail.com", emp.EmployeeEmail, title, message);
            }



            Session["StoreInventoryTabIndex"] = "1";
            Session["StockAdjPage"] = 2;
            return RedirectToAction("Inventory", "Store", new { area = "" });
        }
    }
}
