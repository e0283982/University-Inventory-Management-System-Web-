using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.DAL;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [Authorize(Roles = "Store Manager, Store Supervisor")]
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
        public RedirectToRouteResult Approve(string requestid, string itemcode)
        {
            StockAdjustmentDetail stockadjdet = repo.GetStockAdjustmentDetailById(requestid, itemcode);
            stockadjdet.Status = "Approved";
            stockadjdet.StockAdjustmentHeader.DateProcessed = DateTime.Now;
            repo.UpdateStockAdjustmentDetail(stockadjdet);
            repo.Save();
            Session["StockAdjPage"] = 2;
            return RedirectToAction("Inventory", "Store", new { area = "" });
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
                    UnitCost = stockadjdet.ItemQuantity / stockadjdet.Amount
                };
                itemRepo.InsertItemTransaction(itemTrans);
                itemRepo.Save();
            }
            stockadjdet.Status = "Rejected";
            repo.UpdateStockAdjustmentDetail(stockadjdet);
            repo.Save();
            Session["StockAdjPage"] = 2;
            return RedirectToAction("Inventory", "Store", new { area = "" });
        }
    }
}
