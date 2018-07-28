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
        [System.Web.Mvc.Route("GetRoleDelegationList/{id}")]
        public List<EmployeeDelegationModel> GetRoleDelegationList(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;

                List<Employee> list = m.Employees.Where(x => x.DepartmentCode == id).OrderBy(x => x.EmployeeName).ToList<Employee>();
                List<EmployeeDelegationModel> list2 = new List<EmployeeDelegationModel>();
                list2 = list.ConvertAll(x => new EmployeeDelegationModel
                {
                    EmpId = x.EmployeeID,
                    EmpName = x.EmployeeName,
                    Role = x.Designation,
                    ToDate = m.ApprovalDelegations.Where(y => y.EmployeeID == x.EmployeeID && y.Active == 1).
                    Select(y => y.ToDate).FirstOrDefault(),
                    FromDate = m.ApprovalDelegations.Where(y => y.EmployeeID == x.EmployeeID && y.Active == 1).
                    Select(y => y.FromDate).FirstOrDefault()
                });

                return list2;
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
                List<StaffRequisitionHeader> list = m.StaffRequisitionHeaders.Where(x => x.ApprovalStatus != "Approved").OrderBy(x => x.FormID).ToList<StaffRequisitionHeader>();
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
        [System.Web.Mvc.Route("GetAllPendingApprovals")]
        public List<RequisitionModel> GetAllPendingApprovals()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                //to further filter by user's deptCode
                m.Configuration.ProxyCreationEnabled = false;
                List<StaffRequisitionHeader> list = m.StaffRequisitionHeaders.Where(x => x.ApprovalStatus != "Approved" && x.NotificationStatus != "Deleted").OrderBy(x => x.FormID).ToList<StaffRequisitionHeader>();
                List<RequisitionModel> list2 = new List<RequisitionModel>();
                list2 = list.ConvertAll(x => new RequisitionModel { ReqFormId = x.FormID, ReqEmpName = m.Employees.Where(z => z.EmployeeID == x.EmployeeID).Select(a => a.EmployeeName).First(), DateReq = x.DateRequested });

                return list2;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetReadPendingApprovals")]
        public List<RequisitionModel> GetReadPendingApprovals()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                //to further filter by user's deptCode
                m.Configuration.ProxyCreationEnabled = false;
                List<StaffRequisitionHeader> list = m.StaffRequisitionHeaders.Where(x => x.ApprovalStatus != "Approved" && x.NotificationStatus == "Read").OrderBy(x => x.FormID).ToList<StaffRequisitionHeader>();
                List<RequisitionModel> list2 = new List<RequisitionModel>();
                list2 = list.ConvertAll(x => new RequisitionModel { ReqFormId = x.FormID, ReqEmpName = m.Employees.Where(z => z.EmployeeID == x.EmployeeID).Select(a => a.EmployeeName).First(), DateReq = x.DateRequested });

                return list2;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetUnreadPendingApprovals")]
        public List<RequisitionModel> GetUnreadPendingApprovals()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                //to further filter by user's deptCode
                m.Configuration.ProxyCreationEnabled = false;
                List<StaffRequisitionHeader> list = m.StaffRequisitionHeaders.Where(x => x.ApprovalStatus != "Approved" && x.NotificationStatus == "Unread").OrderBy(x => x.FormID).ToList<StaffRequisitionHeader>();
                List<RequisitionModel> list2 = new List<RequisitionModel>();
                list2 = list.ConvertAll(x => new RequisitionModel { ReqFormId = x.FormID, ReqEmpName = m.Employees.Where(z => z.EmployeeID == x.EmployeeID).Select(a => a.EmployeeName).First(), DateReq = x.DateRequested });

                return list2;
            }
        }
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetBackOrdersByDept/{id}")]
        public List<BackOrderModel> GetBackOrdersByDept(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;

                //get list of active SRFs headers belonging to dept
                List<String> deptReqIds = m.StaffRequisitionHeaders.Where(x => x.DepartmentCode == id).Select(x => x.FormID).ToList<String>();
                List<StaffRequisitionDetail> list = m.StaffRequisitionDetails.Where(i => deptReqIds.Contains(i.FormID) && i.QuantityBackOrdered > 0).ToList<StaffRequisitionDetail>();

                List<BackOrderModel> list2 = new List<BackOrderModel>();
                list2 = list.ConvertAll(x => new BackOrderModel
                {
                    ItemDesc = m.Items.Where(z => z.ItemCode == x.ItemCode).Select(y => y.Description).First()
                    ,
                    UOM = m.Items.Where(z => z.ItemCode == x.ItemCode).Select(a => a.UoM).First(),
                    OutstandingQty = x.QuantityBackOrdered,
                    ReqId = x.FormID,
                    ItemCode = x.ItemCode
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
        [System.Web.Mvc.Route("GetInventoryOverviewList")]
        public List<InventoryOverview> GetInventoryOverviewList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.InventoryOverviews.ToList<InventoryOverview>();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetGoodsReceivedLists/{id}")]
        public List<GoodsReceivedList> GetGoodsReceivedLists(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.GoodsReceivedLists.Where(x => x.ReceiptNo == id).ToList<GoodsReceivedList>();
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

                //return m.StockAdjustmentOverviews.ToList<StockAdjustmentOverview>();

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
        [System.Web.Mvc.Route("GetStockRetrievalList/{id}")]
        public List<StockRetrievalList> GetStockRetrievalList(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.StockRetrievalLists.Where(x => x.Id == id).ToList<StockRetrievalList>();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetPOList")]
        public List<POList> GetPOList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.POLists.ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetPODetails/{id}")]
        public List<POList> GetPODetails(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.POLists.Where(x => x.PONumber == id).ToList<POList>();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetItemListAndSupplier")]
        public List<AllItemPrice> GetItemListAndSupplier()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                return m.AllItemPrices.ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetGROverview")]
        public List<GRList> GetGROverview()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                return m.GRLists.ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetPOFullDetails/{id}")]
        public List<POFullDetail> GetPOFullDetails(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                return m.POFullDetails.Where(x => x.PONumber == id).ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetCollectionList")]
        public List<CollectionList> GetCollectionList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                return m.CollectionLists.ToList();
            }
        }

        //[System.Web.Mvc.Authorize]
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

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetStockAdjustmentManagerApproval")]
        public List<StockAdjustmentApprovalForManager> GetStockAdjustmentManagerApproval()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.StockAdjustmentApprovalForManagers.ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetItemsList")]
        public List<ItemFullDetail> GetItemsList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.ItemFullDetails.ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetItemsById/{id}")]
        public ItemFullDetail GetItemsById(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                ItemFullDetail item = m.ItemFullDetails.Where(x => x.ItemCode == id).FirstOrDefault();                
                return item;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetCategoryList")]
        public List<Category> GetCategoryList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.Categories.ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetCategoryList/{id}")]
        public List<Category> GetCategoryList(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<Category> item = m.Categories.Where(x => x.CategoryID == id).ToList<Category>();
                return item;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetBinsList")]
        public List<Bin> GetBinsList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.Bins.ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetBinsList/{id}")]
        public List<Bin> GetBinsList(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<Bin> item = m.Bins.Where(x => x.Number.ToString() == id).ToList<Bin>();
                return item;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetCollectionPointList")]
        public List<CollectionPoint> GetCollectionPointList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<CollectionPoint> list = m.CollectionPoints.ToList<CollectionPoint>();

                return list;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetCollectionPointList/{id}")]
        public List<CollectionPoint> GetCollectionPointList(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<CollectionPoint> list = m.CollectionPoints.Where(x => x.CollectionPointID == id).ToList<CollectionPoint>();
                return list;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetSuppliersList")]
        public List<Supplier> GetSuppliersList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<Supplier> list = m.Suppliers.ToList<Supplier>();

                return list;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetSuppliersList/{id}")]
        public List<Supplier> GetSuppliersList(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<Supplier> list = m.Suppliers.Where(x => x.SupplierCode == id).ToList<Supplier>();
                return list;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetDeptsList")]
        public List<DeptFullDetailsModel> GetDeptsList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<DepartmentDetail> list = m.DepartmentDetails.ToList<DepartmentDetail>();
                List<DeptFullDetailsModel> list2 = new List<DeptFullDetailsModel>();
                list2 = list.ConvertAll(x => new DeptFullDetailsModel
                {
                    DepartmentCode = x.DepartmentCode,
                    DepartmentName = m.Departments.Where(y => y.DepartmentCode == x.DepartmentCode).Select(y => y.DepartmentName).
                    FirstOrDefault(),
                    ContactName = m.Employees.Where(y => y.EmployeeID == x.RepresentativeID).Select(y => y.EmployeeName).FirstOrDefault(),
                    TelephoneNo = x.TelephoneNo,
                    FaxNo = x.FaxNo,
                    ApproverName = m.Employees.Where(y => y.EmployeeID == x.ApproverID).Select(y => y.EmployeeName).FirstOrDefault(),
                    CollectionPointName = m.CollectionPoints.Where(y => y.CollectionPointID == x.CollectionPointID).Select(y => y.CollectionPointDescription).FirstOrDefault(),
                    RepresentativeName = m.Employees.Where(y => y.EmployeeID == x.RepresentativeID).Select(y => y.EmployeeName).FirstOrDefault(),
                    Active = x.Active
                });

                return list2;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetDeptsList/{id}")]
        public List<DeptFullDetailsModel> GetDeptsList(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<DepartmentDetail> list = m.DepartmentDetails.Where(x => x.DepartmentCode == id).ToList<DepartmentDetail>();
                List<DeptFullDetailsModel> list2 = new List<DeptFullDetailsModel>();
                list2 = list.ConvertAll(x => new DeptFullDetailsModel
                {
                    DepartmentCode = x.DepartmentCode,
                    DepartmentName = m.Departments.Where(y => y.DepartmentCode == x.DepartmentCode).Select(y => y.DepartmentName).
                    FirstOrDefault(),
                    ContactName = m.Employees.Where(y => y.EmployeeID == x.RepresentativeID).Select(y => y.EmployeeName).FirstOrDefault(),
                    TelephoneNo = x.TelephoneNo,
                    FaxNo = x.FaxNo,
                    ApproverName = m.Employees.Where(y => y.EmployeeID == x.ApproverID).Select(y => y.EmployeeName).FirstOrDefault(),
                    CollectionPointName = m.CollectionPoints.Where(y => y.CollectionPointID == x.CollectionPointID).Select(y => y.CollectionPointDescription).FirstOrDefault(),
                    RepresentativeName = m.Employees.Where(y => y.EmployeeID == x.RepresentativeID).Select(y => y.EmployeeName).FirstOrDefault(),
                    Active = x.Active
                });

                return list2;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetRequisitionHistory/{id}")]
        public List<RequisitionHistory> GetRequisitionHistory(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                List<string> deptReqFormIdsList = m.StaffRequisitionHeaders.Where(x => x.DepartmentCode == id).Select(x => x.FormID).ToList();
                return m.RequisitionHistories.Where(x=>deptReqFormIdsList.Contains(x.FormID)).OrderBy(x=>x.FormID).ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetRequisitionHistoryDetail/{id}")]
        public List<RequisitionHistoryDetail> GetRequisitionHistoryDetail(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.RequisitionHistoryDetails.Where(x => x.FormID == id).ToList<RequisitionHistoryDetail>();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetStockTakeList")]
        public List<StockTakeList> GetStockTakeList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                return m.StockTakeLists.ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetReorderList")]
        public List<ReorderList> GetReorderList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                return m.ReorderLists.ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetRptInventory")]
        public List<InventoryValuationReport> GetRptInventory()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                return m.InventoryValuationReports.ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetRptReorder")]
        public List<ReorderReport> GetRptReorder()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                return m.ReorderReports.ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetRptDepartmentUsage")]
        public List<DepartmentUsageReport> GetRptDepartmentUsage()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                return m.DepartmentUsageReports.ToList();
            }
        }

        //TODO: Hendri new restful controllers
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Route("CreateNewStockAdjustment")]
        public void CreateNewStockAdjustment(StockAdjustmentModel stockAdjustmentModel)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                //Adding new StockAdjustmentHeader            
                StockAdjustmentHeader stockAdjustmentHeader = new StockAdjustmentHeader();

                int stockAdjustmentHeaderCount = m.StockAdjustmentHeaders.Count() + 1;
            
                stockAdjustmentHeader.RequestId = "SA-" + stockAdjustmentHeaderCount;

                //DateTime
                DateTime localDate = DateTime.Now;
                stockAdjustmentHeader.DateRequested = localDate;
                stockAdjustmentHeader.Requestor = stockAdjustmentModel.RequestorId;
                stockAdjustmentHeader.TransactionType = "Stock Adjustment";
                m.StockAdjustmentHeaders.Add(stockAdjustmentHeader);

                //Adding new StockAdjustmentDetails
                StockAdjustmentDetail stockAdjustmentDetail = new StockAdjustmentDetail();
                stockAdjustmentDetail.RequestId = stockAdjustmentHeader.RequestId;

                String itemDescription = stockAdjustmentModel.ItemDescription;
                String itemCode = m.Items.Where(x => x.Description == itemDescription).Select(x => x.ItemCode).FirstOrDefault();
                stockAdjustmentDetail.ItemCode = itemCode;
                stockAdjustmentDetail.ItemQuantity = stockAdjustmentModel.AdjustedQuantity;

                float itemUnitCost = m.Items.Where(x => x.ItemCode == stockAdjustmentDetail.ItemCode).Select(x => x.AvgUnitCost).FirstOrDefault();
                stockAdjustmentDetail.Amount = itemUnitCost * stockAdjustmentDetail.ItemQuantity;
                stockAdjustmentDetail.Remarks = stockAdjustmentModel.Remarks;
                stockAdjustmentDetail.Status = "Pending";
                m.StockAdjustmentDetails.Add(stockAdjustmentDetail);

                //Update Stock Retrieval Details
                //If it is from item screen then there will not be any stock retrieval
                if (!stockAdjustmentModel.StockRetrievalId.Equals("NoStockRetrieval"))
                {
                    StockRetrievalDetail stockRetrievalDetail = new StockRetrievalDetail();
                    stockRetrievalDetail = m.StockRetrievalDetails.Where(x => x.Id == stockAdjustmentModel.StockRetrievalId && x.ItemCode == itemCode).FirstOrDefault();
                    stockRetrievalDetail.QuantityRetrieved = stockRetrievalDetail.QuantityRetrieved - stockAdjustmentModel.AdjustedQuantity;
                    stockRetrievalDetail.QuantityAdjusted = stockRetrievalDetail.QuantityAdjusted + stockAdjustmentModel.AdjustedQuantity;
                    stockRetrievalDetail.Remarks = stockAdjustmentModel.Remarks;
                }


                //Create new Item Transaction
                ItemTransaction itemTransaction = new ItemTransaction();
                itemTransaction.TransDateTime = localDate;
                itemTransaction.DocumentRefNo = stockAdjustmentHeader.RequestId;
                itemTransaction.ItemCode = stockAdjustmentDetail.ItemCode;
                itemTransaction.TransactionType = "Stock Adjustment";
                itemTransaction.Quantity = stockAdjustmentDetail.ItemQuantity;
                itemTransaction.UnitCost = itemUnitCost;
                itemTransaction.Amount = itemTransaction.Quantity * itemTransaction.UnitCost;
                m.ItemTransactions.Add(itemTransaction);

                //Update Item Quantity
                Item item = m.Items.Where(x => x.Description == itemDescription).FirstOrDefault();
                item.Quantity = item.Quantity - stockAdjustmentDetail.ItemQuantity;

                m.SaveChanges();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetLatestStockRetrievalId")]
        public StockRetrievalHeader GetLatestStockRetrievalId()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.StockRetrievalHeaders.OrderByDescending(x => x.Date).FirstOrDefault();

            }
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Route("UpdateDisbursement")]
        public void UpdateDisbursement(DisbursementDetailModel disbursementDetailModel)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;

                //Update Disbursement Header to completed
                DisbursementHeader disbursementHeader = m.DisbursementHeaders.Where(x => x.Id == disbursementDetailModel.DisbursementId).FirstOrDefault();
                disbursementHeader.Status = "Completed";

                //Update Disbursement Detail               
                Item item = m.Items.Where(x => x.Description == disbursementDetailModel.ItemDescription).FirstOrDefault();
                string itemCode = item.ItemCode;

                DisbursementDetail disbursementDetail = m.DisbursementDetails.Where(x => x.Id == disbursementDetailModel.DisbursementId && x.ItemCode == itemCode).FirstOrDefault();
                disbursementDetail.QuantityReceived = disbursementDetailModel.QuantityReceived;
                disbursementDetail.QuantityAdjusted = disbursementDetailModel.QuantityAdjusted;

                if(disbursementDetail.QuantityAdjusted > 0)
                {

                    //Adding new StockAdjustmentHeader            
                    StockAdjustmentHeader stockAdjustmentHeader = new StockAdjustmentHeader();
                    int stockAdjustmentHeaderCount = m.StockAdjustmentHeaders.Count() + 1;
                    stockAdjustmentHeader.RequestId = "SA-" + stockAdjustmentHeaderCount;

                    //DateTime
                    DateTime localDate = DateTime.Now;
                    stockAdjustmentHeader.DateRequested = localDate;
                    stockAdjustmentHeader.Requestor = disbursementDetailModel.RequestorId;
                    stockAdjustmentHeader.TransactionType = "Stock Adjustment";
                    m.StockAdjustmentHeaders.Add(stockAdjustmentHeader);

                    //Adding new StockAdjustmentDetails
                    StockAdjustmentDetail stockAdjustmentDetail = new StockAdjustmentDetail();
                    stockAdjustmentDetail.RequestId = stockAdjustmentHeader.RequestId;

                    stockAdjustmentDetail.ItemCode = itemCode;
                    stockAdjustmentDetail.ItemQuantity = disbursementDetail.QuantityAdjusted;

                    float itemUnitCost = m.Items.Where(x => x.ItemCode == stockAdjustmentDetail.ItemCode).Select(x => x.AvgUnitCost).FirstOrDefault();
                    stockAdjustmentDetail.Amount = itemUnitCost * stockAdjustmentDetail.ItemQuantity;
                    stockAdjustmentDetail.Remarks = "Damaged";
                    stockAdjustmentDetail.Status = "Pending";
                    m.StockAdjustmentDetails.Add(stockAdjustmentDetail);

                    //Create 2 Item Transactions to plus and minus
                    //to add back damaged items that are not taken by the employee
                    DateTime localDate1 = DateTime.Now;
                    ItemTransaction itemTransaction1 = new ItemTransaction();
                    itemTransaction1.TransDateTime = localDate1;
                    itemTransaction1.DocumentRefNo = disbursementHeader.Id;
                    itemTransaction1.ItemCode = disbursementDetail.ItemCode;
                    itemTransaction1.TransactionType = "Disbursement Adjustment";
                    itemTransaction1.Quantity = disbursementDetail.QuantityAdjusted;
                    itemTransaction1.UnitCost = itemUnitCost;
                    itemTransaction1.Amount = itemTransaction1.Quantity * itemTransaction1.UnitCost;
                    m.ItemTransactions.Add(itemTransaction1);

                    //To create stock adjustment
                    DateTime localDate2 = DateTime.Now;
                    ItemTransaction itemTransaction2 = new ItemTransaction();
                    itemTransaction2.TransDateTime = localDate2;
                    itemTransaction2.DocumentRefNo = stockAdjustmentHeader.RequestId;
                    itemTransaction2.ItemCode = stockAdjustmentDetail.ItemCode;
                    itemTransaction2.TransactionType = "Stock Adjustment";
                    itemTransaction2.Quantity = stockAdjustmentDetail.ItemQuantity;
                    itemTransaction2.UnitCost = itemUnitCost;
                    itemTransaction2.Amount = itemTransaction2.Quantity * itemTransaction2.UnitCost;
                    m.ItemTransactions.Add(itemTransaction2);

                }

                m.SaveChanges();            

            }
                    
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetStaffRequisitionHeader")]
        public List<StaffRequisitionHeader> GetStaffRequisitionHeader()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.StaffRequisitionHeaders.OrderByDescending(x => x.ApprovalStatus).ToList<StaffRequisitionHeader>();                
            }
        }















    }
}