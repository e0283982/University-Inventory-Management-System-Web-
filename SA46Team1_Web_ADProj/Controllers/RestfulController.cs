using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Data.Entity;


namespace SA46Team1_Web_ADProj.Controllers
{
    [System.Web.Mvc.RoutePrefix("api/Restful")]
    public class RestfulController : ApiController
    {
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetEmployeeList")]
        public List<Employee> GetEmployeeList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                //to further filter by user's deptCode
                m.Configuration.ProxyCreationEnabled = false;
                return m.Employees.OrderBy(x=>x.EmployeeName).ToList<Employee>();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetPendingApprovals")]
        public List<RequisitionModel> GetPendingApprovals()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                //to further filter by user's deptCode
                m.Configuration.ProxyCreationEnabled = false;
                List<StaffRequisitionHeader> list = m.StaffRequisitionHeaders.OrderBy(x => x.FormID).ToList<StaffRequisitionHeader>();
                List<RequisitionModel> list2 = new List<RequisitionModel>();
                list2 = list.ConvertAll(x => new RequisitionModel { ReqFormId = x.FormID, ReqEmpName = m.Employees.Where(z => z.EmployeeID == x.EmployeeID).Select(a => a.EmployeeName).First(), DateReq = x.DateRequested });

                return list2;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetPendingApprovalsById/{id}")]
        public List<RequisitionDetModel> GetPendingApprovalsById(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<StaffRequisitionDetail> list = m.StaffRequisitionDetails.Where(x => x.FormID == id).ToList<StaffRequisitionDetail>();
                List<RequisitionDetModel> list2 = new List<RequisitionDetModel>();
                list2 = list.ConvertAll(x => new RequisitionDetModel
                {
                    ItemDesc = m.Items.Where(z => z.ItemCode == x.ItemCode).Select(y => y.Description).First()
                    ,
                    UOM = m.Items.Where(z => z.ItemCode == x.ItemCode).Select(a => a.UoM).First(),
                    OrderQty = x.QuantityOrdered
                });

                return list2;
            }
        }


        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetItemCodeList")]
        public List<String> GetItemCodeList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                return m.InventoryOverviews.Select(x => x.ItemCode).ToList<String>();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetInventoryOverviewList/{id}")]
        public List<InventoryOverview> GetInventoryOverviewList(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.InventoryOverviews.Where(x => x.ItemCode == id).ToList<InventoryOverview>();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetGoodsReceivedLists/{id}")]
        public List<GoodsReceivedList> GetGoodsReceivedLists(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.GoodsReceivedLists.Where(x => x.ItemCode == id).ToList<GoodsReceivedList>();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("AdjustmentOverView")]
        public List<StockAdjustmentHeader> GetStockAdjustmentOverview()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.StockAdjustmentHeaders.ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetStockAdjustmentList")]
        public List<StockAdjustmentOverview> GetStockAdjustmentList()
        {
            //Temporary placeholder to make the requestID = 1
            string requestorId = "E1";

            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.StockAdjustmentOverviews.Where(x => x.Requestor == requestorId).ToList<StockAdjustmentOverview>();
            }

        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetRequisitionList")]
        public List<RequisitionList> GetRequisitionList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.RequisitionLists.ToList<RequisitionList>();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetShortItemList")]
        public List<Item> GetShortItemList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.Items.ToList();
            }
        }


        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetRequisitionListDetails/{id}")]
        public List<RequisitionListDetail> GetRequisitionListDetails(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.RequisitionListDetails.Where(x => x.FormID == id).ToList<RequisitionListDetail>();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetDisbursementList")]
        public List<DisbursementList> GetDisbursementList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.DisbursementLists.ToList<DisbursementList>();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetDisbursementListDetails/{id}")]
        public List<DisbursementListDetail> GetDisbursementListDetails(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.DisbursementListDetails.Where(x => x.Id == id).ToList<DisbursementListDetail>();
            }
        }


        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetStockAdjustmentSupervisorApproval")]
        public List<StockAdjustmentApprovalForSupervisor> GetStockAdjustmentSupervisorApproval()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.StockAdjustmentApprovalForSupervisors.ToList();
            }
        }

    }
}