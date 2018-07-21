using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Store/StorePurchase")]
    public class StorePurchaseController : Controller
    {
        [Route("CreatePO")]
        public ActionResult CreatePO()
        {
            return View();
        }

        [Route("POList")]
        public ActionResult POList()
        {
            if (Session["POListPage"].ToString() == "1")
            {
                return View("POList");
            }
            else
            {
                Session["POListPage"] = "1";
                return View("DisplayPO");
            }
        }

        //[HttpPost]
        //public ActionResult Test(FormCollection data)
        //{
        //    string test = data["PONumber"];
        //    return null;
        //}

        [HttpPost]
        public RedirectToRouteResult DisplayPO(FormCollection data)
        {
            Session["POListPage"] = "2";
            string poNumber = data["PONumber"];
            return RedirectToAction("Purchase", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToPOList()
        {
            Session["POListPage"] = "1";

            return RedirectToAction("Purchase", "Store");
        }

        [Route("GoodsReceivedList")]
        public ActionResult GoodsReceivedList()
        {
            if (Session["GRListPage"].ToString() == "1")
            {
                @Session["BackToGRList"] = "false";
                return View("GoodsReceivedList");
            }
            else
            {
                Session["GRListPage"] = "1";
                string id = "C001";
                string grn = null;
                DateTime date = new DateTime();
                string suppler = null;
                string rec = null;
                string remarks = null;
                string po = null;
                List<GoodsReceivedList> grl = GoodsReceivedLists(id);
                foreach (GoodsReceivedList g in grl)
                {
                    grn = g.ReceiptNo.ToString();
                    date = (DateTime)g.ReceivedDate;
                    suppler = g.CompanyName;
                    rec = g.EmployeeName;
                    remarks = g.Remarks;
                    po = g.PONumber;
                }
                ViewBag.Id = grn;
                ViewBag.date = date;
                ViewBag.sup = suppler;
                ViewBag.rec = rec;
                ViewBag.rem = remarks;
                ViewBag.po = po;
                return View("GoodsReceivedList2");
            }
        }

        //Temporary data to put here for Jayden since JWTempData is deleted
        public static List<GoodsReceivedList> GoodsReceivedLists(string itemcode)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                return m.GoodsReceivedLists.Where(x => x.ItemCode == itemcode).ToList();
            }
        }



        [HttpPost]
        public RedirectToRouteResult DisplayGR()
        {
            Session["GRListPage"] = "2";
            return RedirectToAction("Purchase", "Store");
        }

       
        [HttpPost]
        public RedirectToRouteResult BackToGRList()
        {
            Session["GRListPage"] = "1";
            Session["BackToGRList"] = "true";

            return RedirectToAction("Purchase", "Store");
        }

    }
}
