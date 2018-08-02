using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Data.Entity;
using SA46Team1_Web_ADProj.DAL;
using Newtonsoft.Json.Linq;

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

                List<Employee> list = m.Employees.Where(x => x.DepartmentCode == id && x.Active==1).OrderBy(x => x.EmployeeName).ToList();
                List<EmployeeDelegationModel> list2 = new List<EmployeeDelegationModel>();
                list2 = list.ConvertAll(x => new EmployeeDelegationModel
                {
                    EmpId = x.EmployeeID,
                    EmpName = x.EmployeeName,
                    Role = x.Designation,
                    ToDate = m.ApprovalDelegations.Where(y => y.EmployeeID == m.DepartmentDetails.Where(z =>z.Active==1 && z.DepartmentCode == id 
                        && z.ApproverID == x.EmployeeID).Select(z => z.ApproverID).FirstOrDefault() && y.Active==1)
                        .Select(y => y.ToDate).FirstOrDefault(),
                    FromDate = m.ApprovalDelegations.Where(y => y.EmployeeID == m.DepartmentDetails.Where(z=> z.Active == 1 && z.DepartmentCode==id 
                        && z.ApproverID==x.EmployeeID).Select(z=>z.ApproverID).FirstOrDefault() && y.Active == 1)
                        .Select(y=>y.FromDate).FirstOrDefault()
                });

                return list2;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetPendingApprovals/{id}")]
        public List<RequisitionModel> GetPendingApprovalsByDept(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                //to further filter by user's deptCode
                m.Configuration.ProxyCreationEnabled = false;
                List<StaffRequisitionHeader> list = m.StaffRequisitionHeaders.Where(x => x.ApprovalStatus == "Pending" && x.DepartmentCode==id).OrderBy(x => x.FormID).ToList();
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
                List<StaffRequisitionDetail> list = m.StaffRequisitionDetails.Where(x => x.FormID == id).ToList();
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
        [System.Web.Mvc.Route("GetAllPendingApprovals/{id}")]
        public List<StaffRequisitionHeader> GetAllPendingApprovals(string id)
        {
            List<StaffRequisitionHeader> srhList = new List<StaffRequisitionHeader>();
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                Employee emp = m.Employees.Where(x => x.EmployeeID == id).FirstOrDefault();
                if(emp.Designation == "Department Head")
                {
                    srhList = m.StaffRequisitionHeaders.Where(x => x.DepartmentCode == emp.DepartmentCode
                    && x.ApprovalStatus == "Pending" && x.NotificationStatus != "Deleted").ToList();
                }
                else if(emp.Approver == 1 && (emp.Designation == "Employee" || emp.Designation == "Employee Representative"))
                {
                    srhList = m.StaffRequisitionHeaders.Where(x => (x.DepartmentCode == emp.DepartmentCode
                    && x.ApprovalStatus == "Pending" && x.NotificationStatus != "Deleted") ||
                    (x.DepartmentCode == emp.DepartmentCode && x.EmployeeID == emp.EmployeeID
                        && (x.ApprovalStatus == "Approved" || x.ApprovalStatus == "Rejected") && x.NotificationStatus != "Deleted")).ToList();
                }
                else
                {
                    srhList = m.StaffRequisitionHeaders
                        .Where(x => x.DepartmentCode == emp.DepartmentCode && x.EmployeeID == emp.EmployeeID
                        && (x.ApprovalStatus == "Approved" || x.ApprovalStatus == "Rejected") && x.NotificationStatus != "Deleted").ToList();
                }

                if(srhList == null)
                {
                    srhList = new List<StaffRequisitionHeader>();
                }
            }
            return srhList;
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetReadPendingApprovals/{id}")]
        public List<StaffRequisitionHeader> GetReadPendingApprovals(string id)
        {
            List<StaffRequisitionHeader> srhList = new List<StaffRequisitionHeader>();
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                Employee emp = m.Employees.Where(x => x.EmployeeID == id).FirstOrDefault();
                if (emp.Designation == "Department Head")
                {
                    srhList = m.StaffRequisitionHeaders.Where(x => x.DepartmentCode == emp.DepartmentCode
                    && x.ApprovalStatus == "Pending" && x.NotificationStatus == "Read").ToList();
                }
                else
                {
                    srhList = m.StaffRequisitionHeaders
                        .Where(x => x.DepartmentCode == emp.DepartmentCode && x.EmployeeID == emp.EmployeeID
                        && (x.ApprovalStatus == "Approved" || x.ApprovalStatus == "Rejected") && x.NotificationStatus == "Read").ToList();
                }

                if (srhList == null)
                {
                    srhList = new List<StaffRequisitionHeader>();
                }
            }
            return srhList;
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetUnreadPendingApprovals/{id}")]
        public List<StaffRequisitionHeader> GetUnreadPendingApprovals(string id)
        {
            List<StaffRequisitionHeader> srhList = new List<StaffRequisitionHeader>();
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                Employee emp = m.Employees.Where(x => x.EmployeeID == id).FirstOrDefault();
                if (emp.Designation == "Department Head")
                {
                    srhList = m.StaffRequisitionHeaders.Where(x => x.DepartmentCode == emp.DepartmentCode
                    && x.ApprovalStatus == "Pending" && x.NotificationStatus == "Unread").ToList();
                }
                else
                {
                    srhList = m.StaffRequisitionHeaders
                        .Where(x => x.DepartmentCode == emp.DepartmentCode && x.EmployeeID == emp.EmployeeID
                        && (x.ApprovalStatus == "Approved" || x.ApprovalStatus == "Rejected") && x.NotificationStatus == "Unread").ToList();
                }

                if (srhList == null)
                {
                    srhList = new List<StaffRequisitionHeader>();
                }
            }
            return srhList;
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetBackOrdersByDept/{id}")]
        public List<BackOrderModel> GetBackOrdersByDept(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;

                //get list of active SRFs headers belonging to dept
                List<String> deptReqIds = m.StaffRequisitionHeaders.Where(x => x.DepartmentCode == id).Select(x => x.FormID).ToList();
                List<StaffRequisitionDetail> list = m.StaffRequisitionDetails
                    .Where(i => deptReqIds.Contains(i.FormID) && i.QuantityDelivered!=i.QuantityOrdered && i.QuantityDelivered>0 && i.CancelledBackOrdered==0).ToList();

                List<BackOrderModel> list2 = new List<BackOrderModel>();
                list2 = list.ConvertAll(x => new BackOrderModel
                {
                    ItemDesc = m.Items.Where(z => z.ItemCode == x.ItemCode).Select(y => y.Description).First(),
                    UOM = m.Items.Where(z => z.ItemCode == x.ItemCode).Select(a => a.UoM).First(),
                    OutstandingQty = x.QuantityBackOrdered,
                    ReqId = x.FormID,
                    ItemCode = x.ItemCode
                });

                return list2;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetBackOrdersByEmployee/{id}")]
        public List<BackOrderModel> GetBackOrdersByEmployee(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;

                //get list of active SRFs headers belonging to dept
                List<String> deptReqIds = m.StaffRequisitionHeaders.Where(x => x.EmployeeID == id).Select(x => x.FormID).ToList();
                List<StaffRequisitionDetail> list = m.StaffRequisitionDetails
                    .Where(i => deptReqIds.Contains(i.FormID) && i.QuantityDelivered != i.QuantityOrdered && i.QuantityDelivered > 0 && i.CancelledBackOrdered == 0).ToList();

                List<BackOrderModel> list2 = new List<BackOrderModel>();
                list2 = list.ConvertAll(x => new BackOrderModel
                {
                    ItemDesc = m.Items.Where(z => z.ItemCode == x.ItemCode).Select(y => y.Description).First(),
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
                return m.InventoryOverviews.Select(x => x.ItemCode).ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetInventoryOverviewList")]
        public List<InventoryOverview> GetInventoryOverviewList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.InventoryOverviews.ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetGoodsReceivedLists/{id}")]
        public List<GoodsReceivedList> GetGoodsReceivedLists(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.GoodsReceivedLists.Where(x => x.ReceiptNo == id).ToList();
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
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;

                return m.StockAdjustmentOverviews.ToList();
            }

        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetRequisitionList")]
        public List<RequisitionList> GetRequisitionList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.RequisitionLists.ToList();
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
                return m.RequisitionListDetails.Where(x => x.FormID == id).ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetDisbursementList")]
        public List<DisbursementList> GetDisbursementList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.DisbursementLists.ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetDisbursementListDetails/{id}")]
        public List<DisbursementListDetail> GetDisbursementListDetails(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.DisbursementListDetails.Where(x => x.Id == id).ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetStockRetrievalList/{id}")]
        public List<StockRetrievalList> GetStockRetrievalList(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.StockRetrievalLists.Where(x => x.Id == id).ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetPOList")]
        public List<POList> GetPOList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.POLists.OrderBy(x=>x.Date).ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetPODetails/{id}")]
        public List<POList> GetPODetails(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.POLists.Where(x => x.PONumber == id).ToList();
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

        [System.Web.Http.Authorize]
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
                List<Category> item = m.Categories.Where(x => x.CategoryID == id).ToList();
                return item;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetBinsList")]
        public List<FullBinModel> GetBinsList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<Bin> list = m.Bins.ToList();
                List<FullBinModel> list2 = list.ConvertAll(x => new FullBinModel
                {
                    Number = x.Number,
                    Location = x.Location,
                    Active = x.Active,
                    ItemDesc=m.Items.Where(y=>y.ItemCode==x.ItemCode).Select(y=>y.Description).FirstOrDefault()
                });

                return list2;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetBinsList/{id}")]
        public List<FullBinModel> GetBinsList(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<Bin> item = m.Bins.Where(x => x.Number.ToString() == id).ToList();
                List<FullBinModel> list2 = item.ConvertAll(x => new FullBinModel
                {
                    Number = x.Number,
                    Location = x.Location,
                    Active = x.Active,
                    ItemDesc = m.Items.Where(y => y.ItemCode == x.ItemCode).Select(y => y.Description).FirstOrDefault()
                });

                return list2;
            }
        
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetCollectionPointList")]
        public List<CollectionPoint> GetCollectionPointList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<CollectionPoint> list = m.CollectionPoints.OrderBy(x=>x.CollectionPointID).ToList();

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
                List<CollectionPoint> list = m.CollectionPoints.Where(x => x.CollectionPointID == id).ToList();
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
                List<Supplier> list = m.Suppliers.ToList();

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
                List<Supplier> list = m.Suppliers.Where(x => x.SupplierCode == id).ToList();
                return list;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetSupplierPriceList/{id}")]
        public List<FullSupplierPriceList> GetSupplierPriceList(string id) //where id is supplier id
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<SupplierPriceList> list = m.SupplierPriceLists.Where(x => x.SupplierCode == id).ToList();

                List<FullSupplierPriceList> list2 = list.ConvertAll(x => new FullSupplierPriceList
                {
                    SupplierCode = x.SupplierCode,
                    ItemCode = x.ItemCode,
                    ItemDesc = m.Items.Where(y=>y.ItemCode==x.ItemCode).Select(y=>y.Description).FirstOrDefault(),
                    UnitCost = x.UnitCost,
                    Active = x.Active,
                    UoM=x.UoM
                });

                return list2;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetDeptsList")]
        public List<DeptFullDetailsModel> GetDeptsList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<DepartmentDetail> list = m.DepartmentDetails.ToList();
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
                    CollectionPointName = m.CollectionPoints.Where(y => y.CollectionPointID == x.CollectionPointID)
                    .Select(y => y.CollectionPointDescription).FirstOrDefault(),
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
                List<DepartmentDetail> list = m.DepartmentDetails.Where(x => x.DepartmentCode == id).ToList();
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
                    CollectionPointName = m.CollectionPoints.Where(y => y.CollectionPointID == x.CollectionPointID)
                    .Select(y => y.CollectionPointDescription).FirstOrDefault(),
                    RepresentativeName = m.Employees.Where(y => y.EmployeeID == x.RepresentativeID).Select(y => y.EmployeeName).FirstOrDefault(),
                    Active = x.Active
                });

                return list2;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetRequisitionHistory/{id}")]
        public List<StaffRequisitionHeader> GetRequisitionHistory(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<StaffRequisitionHeader> deptReqForms = m.StaffRequisitionHeaders.Where(x => x.EmployeeID == id).ToList();
                
                return deptReqForms;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetDeptRequisitionHistory/{id}")]
        public List<StaffRequisitionHeader> GetDeptRequisitionHistory(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<StaffRequisitionHeader> deptReqForms = m.StaffRequisitionHeaders.Where(x => x.DepartmentCode == id).ToList();

                return deptReqForms;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetRequisitionHistoryDetail/{id}")]
        public List<RequisitionHistoryDetail> GetRequisitionHistoryDetail(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.RequisitionHistoryDetails.Where(x => x.FormID == id).ToList();
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

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetRptDepartmentUsage/{id}")]
        public List<DepartmentUsageReport> GetRptDepartmentUsage(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                string deptName = m.Departments.Where(x => x.DepartmentCode == id).Select(x => x.DepartmentName).FirstOrDefault();
                return m.DepartmentUsageReports.Where(x=>x.DepartmentName==deptName).ToList();
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

                stockAdjustmentHeader.RequestId = CommonLogic.SerialNo(stockAdjustmentHeaderCount, "SA");

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
                    stockRetrievalDetail = m.StockRetrievalDetails
                        .Where(x => x.Id == stockAdjustmentModel.StockRetrievalId && x.ItemCode == itemCode).FirstOrDefault();
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
                return m.StockRetrievalHeaders.OrderByDescending(x => x.ID).FirstOrDefault();

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
                DisbursementHeader disbursementHeader = m.DisbursementHeaders
                    .Where(x => x.Id == disbursementDetailModel.DisbursementId).FirstOrDefault();
                disbursementHeader.Status = "Completed";               

                //Update Disbursement Detail               
                Item item = m.Items.Where(x => x.Description == disbursementDetailModel.ItemDescription).FirstOrDefault();
                string itemCode = item.ItemCode;

                DisbursementDetail disbursementDetail = m.DisbursementDetails.Where(x => x.Id == disbursementDetailModel.DisbursementId && x.ItemCode == itemCode).FirstOrDefault();
                disbursementDetail.QuantityReceived = disbursementDetailModel.QuantityReceived;
                disbursementDetail.QuantityAdjusted = disbursementDetailModel.QuantityAdjusted;

                if (disbursementDetail.QuantityAdjusted > 0)
                {

                    //Only when id is 1
                    if (disbursementDetailModel.DisbursementAndroidId == 1)
                    {
                        //Adding new StockAdjustmentHeader            
                        StockAdjustmentHeader stockAdjustmentHeader = new StockAdjustmentHeader();
                        int stockAdjustmentHeaderCount = m.StockAdjustmentHeaders.Count() + 1;
                        stockAdjustmentHeader.RequestId = CommonLogic.SerialNo(stockAdjustmentHeaderCount, "SA");

                        //DateTime
                        DateTime localDate = DateTime.Now;
                        stockAdjustmentHeader.DateRequested = localDate;
                        stockAdjustmentHeader.Requestor = disbursementDetailModel.RequestorId;
                        stockAdjustmentHeader.TransactionType = "Stock Adjustment";
                        m.StockAdjustmentHeaders.Add(stockAdjustmentHeader);

                        m.SaveChanges();
                    }                    

                    //Adding new StockAdjustmentDetails
                    StockAdjustmentDetail stockAdjustmentDetail = new StockAdjustmentDetail();

                    int latestStockAdjustmentHeaderCount = m.StockAdjustmentHeaders.Count();                                    
                    stockAdjustmentDetail.RequestId = CommonLogic.SerialNo(latestStockAdjustmentHeaderCount, "SA");

                    stockAdjustmentDetail.ItemCode = itemCode;
                    stockAdjustmentDetail.ItemQuantity = disbursementDetail.QuantityAdjusted;

                    float itemUnitCost = m.Items.Where(x => x.ItemCode == stockAdjustmentDetail.ItemCode)
                        .Select(x => x.AvgUnitCost).FirstOrDefault();
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
                    itemTransaction2.DocumentRefNo = stockAdjustmentDetail.RequestId;
                    itemTransaction2.ItemCode = stockAdjustmentDetail.ItemCode;
                    itemTransaction2.TransactionType = "Stock Adjustment";
                    itemTransaction2.Quantity = stockAdjustmentDetail.ItemQuantity;
                    itemTransaction2.UnitCost = itemUnitCost;
                    itemTransaction2.Amount = itemTransaction2.Quantity * itemTransaction2.UnitCost;
                    m.ItemTransactions.Add(itemTransaction2);

                }

                

                //To update the list of staff req headers to be completed, there would be multiple staff requisition headers combined
                //To update the staff requisition details for quantity delivered

                List<String> listOfReqFormId = m.StockRetrievalReqForms.OrderBy(x => x.Id).Where(x => x.StockRetrievalID == disbursementHeader.StockRetrievalId).Select(x => x.ReqFormID).ToList<String>();
                String departmentCode = disbursementHeader.DepartmentCode;

                int trailingQuantityReceivedByDepartment = disbursementDetailModel.QuantityReceived;

                List<String> reqFormIdentified = new List<string>();

                foreach (String reqForm in listOfReqFormId)
                {
                    String deptCode = m.StaffRequisitionHeaders.Where(x => x.FormID == reqForm).Select(x => x.DepartmentCode).FirstOrDefault();
                    if (deptCode.Equals(departmentCode))
                    {
                        reqFormIdentified.Add(reqForm);
                    }                    

                }


                foreach (String reqForm in reqFormIdentified)
                {                    

                    StaffRequisitionHeader staffRequisitionHeader = m.StaffRequisitionHeaders.Where(x => x.FormID == reqForm && x.DepartmentCode == departmentCode).FirstOrDefault();                    
                    StaffRequisitionDetail staffRequisitionDetail = m.StaffRequisitionDetails.Where(x => x.FormID == reqForm && x.ItemCode == itemCode).FirstOrDefault();                    

                    if (trailingQuantityReceivedByDepartment <= 0)
                    {
                        break;
                    }
                    
                    //If trailing quantity received by department is more than the quantity backordered, then will be able to fulfill all the request
                    else if(staffRequisitionDetail.QuantityBackOrdered > 0 && trailingQuantityReceivedByDepartment >= staffRequisitionDetail.QuantityBackOrdered)
                    {
                        staffRequisitionDetail.QuantityDelivered = staffRequisitionDetail.QuantityDelivered + staffRequisitionDetail.QuantityBackOrdered;
                        trailingQuantityReceivedByDepartment = trailingQuantityReceivedByDepartment - staffRequisitionDetail.QuantityBackOrdered;
                        staffRequisitionDetail.QuantityBackOrdered = 0;
                        m.SaveChanges();
                    }
                    //This means that the trailing quantity cannot fulfill the entire backordered, so the quantity delivered will be the trailing request
                    else if (staffRequisitionDetail.QuantityBackOrdered > 0 && trailingQuantityReceivedByDepartment < staffRequisitionDetail.QuantityBackOrdered)
                    {
                        staffRequisitionDetail.QuantityDelivered = staffRequisitionDetail.QuantityDelivered + trailingQuantityReceivedByDepartment;
                        trailingQuantityReceivedByDepartment = 0;
                        staffRequisitionDetail.QuantityBackOrdered = staffRequisitionDetail.QuantityBackOrdered - trailingQuantityReceivedByDepartment;
                        m.SaveChanges();
                    }
                    
                }

                

                //To update the status to completed for staff requisition
                foreach (String reqForm in listOfReqFormId)
                {
                    bool completedStaffRequisition = true;

                    StaffRequisitionHeader staffRequisitionHeader = m.StaffRequisitionHeaders.Where(x => x.FormID == reqForm && x.DepartmentCode == departmentCode).FirstOrDefault();
                    List<StaffRequisitionDetail> staffRequisitionDetailsList = m.StaffRequisitionDetails.Where(x => x.FormID == staffRequisitionHeader.FormID).ToList<StaffRequisitionDetail>();

                    foreach(StaffRequisitionDetail srd in staffRequisitionDetailsList)
                    {
                        if(srd.QuantityBackOrdered > 0)
                        {
                            completedStaffRequisition = false;
                        }                        
                    }

                    if (completedStaffRequisition)
                    {
                        staffRequisitionHeader.Status = "Completed";
                        
                    }                                        

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
                return m.StaffRequisitionHeaders.OrderByDescending(x => x.ApprovalStatus).ToList();
            }
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Route("CreateNewRequisition")]
        public void CreateNewRequisition(NewRequisitionModel newRequisitionModel)
        {

            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;

                //Only when id match with size then create new requisition Header
                if (newRequisitionModel.RequisitionAndroidId == newRequisitionModel.RequisitionSize)
                {
                    //Create new Staff Requisition Header
                    StaffRequisitionHeader srh = new StaffRequisitionHeader();

                    int staffRequisitionHeaderCount = m.StaffRequisitionHeaders.Count() + 1;
                    srh.FormID = CommonLogic.SerialNo(staffRequisitionHeaderCount, "SR");

                    srh.EmployeeID = newRequisitionModel.EmployeeId;

                    srh.DepartmentCode = m.Employees.Where(x => x.EmployeeID == srh.EmployeeID).Select(x => x.DepartmentCode).FirstOrDefault();
                    srh.DateRequested = DateTime.Now;
                    srh.Status = "Open";
                    srh.ApprovalStatus = "Pending";

                    //to change to null (default)
                    //srh.DateProcessed = System.DateTime.Now; 

                    srh.Approver = m.Employees.Where(x => x.EmployeeID == srh.EmployeeID).Select(x => x.ReportsTo).FirstOrDefault();
                    srh.NotificationStatus = "Unread";

                    m.StaffRequisitionHeaders.Add(srh);

                    m.SaveChanges();
                }


                //Create new Staff Requisition Details
                StaffRequisitionDetail srd = new StaffRequisitionDetail();
                String itemCode = m.Items.Where(x => x.Description == newRequisitionModel.ItemDescription).Select(x => x.ItemCode).FirstOrDefault();
                srd.ItemCode = itemCode;

                int latestStaffRequisitionHeaderCount = m.StaffRequisitionHeaders.Count();

                srd.FormID = CommonLogic.SerialNo(latestStaffRequisitionHeaderCount, "SR");
                srd.QuantityOrdered = newRequisitionModel.OrderedQuantity;
                srd.QuantityDelivered = 0;
                srd.QuantityBackOrdered = newRequisitionModel.OrderedQuantity;
                srd.CancelledBackOrdered = 0;

                m.StaffRequisitionDetails.Add(srd);

                m.SaveChanges();

            }

        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetRequisitionHistoryDepartmentRep/{id}")]
        public List<RequisitionHistory> GetRequisitionHistoryDepartmentRep(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                List<string> deptReqFormIdsList = m.StaffRequisitionHeaders.Where(x => x.DepartmentCode == id).Select(x => x.FormID).ToList();
                return m.RequisitionHistories.Where(x => deptReqFormIdsList.Contains(x.FormID)).OrderByDescending(x => x.ApprovalStatus).ToList();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetDeptStaffReqs/{id}")]
        public List<StaffReqModel> GetDeptStaffReqs(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;

                List<StaffRequisitionHeader> list = m.StaffRequisitionHeaders.Where(x => x.DepartmentCode == id && x.ApprovalStatus == "Approved" && (x.Status == "Open" || x.Status == "Outstanding"))
                    .OrderBy(x => x.DateRequested).ToList<StaffRequisitionHeader>();
                List<StaffReqModel> list2 = new List<StaffReqModel>();
                list2 = list.ConvertAll(x => new StaffReqModel
                {
                    FormId = x.FormID,
                    RequestDate = x.DateRequested,
                    ReqName = m.Employees.Where(y => y.EmployeeID == x.EmployeeID).Select(y => y.EmployeeName).FirstOrDefault()
                });

                return list2;
            }
        }


        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetDeptCollectionItems/{id}")]
        public List<DeptCollectionItemModel> GetDeptCollectionItems(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;

                List<DisbursementDetail> list = m.DisbursementDetails.Where(x => x.DisbursementHeader.DepartmentCode == id && x.DisbursementHeader.Status == "Open").ToList();

                List<DeptCollectionItemModel> list2 = new List<DeptCollectionItemModel>();
                list2 = list.ConvertAll(x => new DeptCollectionItemModel
                {
                    ItemCode = x.ItemCode,
                    ItemDesc = m.Items.Where(y => y.ItemCode == x.ItemCode).Select(y => y.Description).FirstOrDefault(),
                    UoM = m.Items.Where(y => y.ItemCode == x.ItemCode).Select(y => y.UoM).FirstOrDefault(),
                    ExpectedQty = x.QuantityOrdered - x.QuantityReceived //check that for partial disbursement, status is open.
                });

                return list2;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetStoreAllNotifications/{id}")]
        public List<StockAdjustmentFullDetail> GetStoreAllNotifications(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<StockAdjustmentFullDetail> safdList = new List<StockAdjustmentFullDetail>();
                Employee emp = m.Employees.Where(x => x.EmployeeID == id).FirstOrDefault();
                string empRole = emp.Designation;
                if(empRole == "Store Clerk")
                {
                    safdList = m.StockAdjustmentFullDetails.Where(x => x.Status == "Approved" || x.Status == "Rejected").ToList();
                }
                else
                if(empRole == "Store Supervisor")
                {
                    safdList = m.StockAdjustmentFullDetails.Where(x => x.Status == "Pending" && x.Amount < 250).ToList();
                }
                else
                if(empRole == "Store Manager")
                {
                    safdList = m.StockAdjustmentFullDetails.Where(x => x.Status == "Pending" && x.Amount >= 250).ToList();
                }
                return safdList;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetStoreReadNotifications/{id}")]
        public List<StockAdjustmentFullDetail> GetStoreReadNotifications(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<StockAdjustmentFullDetail> safdList = new List<StockAdjustmentFullDetail>();
                Employee emp = m.Employees.Where(x => x.EmployeeID == id).FirstOrDefault();
                string empRole = emp.Designation;
                if (empRole == "Store Clerk")
                {
                    safdList = m.StockAdjustmentFullDetails.Where(x => (x.Status == "Approved" && x.NotificationStatus == "Read") 
                    || (x.Status == "Rejected" && x.NotificationStatus == "Read")).ToList();
                }
                else
                if (empRole == "Store Supervisor")
                {
                    safdList = m.StockAdjustmentFullDetails
                        .Where(x => x.Status == "Pending" && x.Amount < 250 && x.NotificationStatus == "Read").ToList();
                }
                else
                if (empRole == "Store Manager")
                {
                    safdList = m.StockAdjustmentFullDetails
                        .Where(x => x.Status == "Pending" && x.Amount >= 250 && x.NotificationStatus == "Read").ToList();
                }
                return safdList;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetStoreUnreadNotifications/{id}")]
        public List<StockAdjustmentFullDetail> GetStoreUnreadNotifications(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<StockAdjustmentFullDetail> safdList = new List<StockAdjustmentFullDetail>();
                Employee emp = m.Employees.Where(x => x.EmployeeID == id).FirstOrDefault();
                string empRole = emp.Designation;
                if (empRole == "Store Clerk")
                {
                    safdList = m.StockAdjustmentFullDetails.Where(x => (x.Status == "Approved" && x.NotificationStatus == "Unread")
                    || (x.Status == "Rejected" && x.NotificationStatus == "Unread")).ToList();
                }
                else
                if (empRole == "Store Supervisor")
                {
                    safdList = m.StockAdjustmentFullDetails
                        .Where(x => x.Status == "Pending" && x.Amount < 250 && x.NotificationStatus == "Unread").ToList();
                }
                else
                if (empRole == "Store Manager")
                {
                    safdList = m.StockAdjustmentFullDetails
                        .Where(x => x.Status == "Pending" && x.Amount >= 250 && x.NotificationStatus == "Unread").ToList();
                }
                return safdList;
            }
        }

        [System.Web.Http.Authorize]
        [System.Web.Http.HttpGet]
        [System.Web.Mvc.Route("GetEmployeeRole/{email}")]
        public Employee GetEmployeeRole(string email)
        {

            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.Employees.Where(x => x.EmployeeEmail == email).FirstOrDefault();
            }


                //using (IEmployeeRepository empRepo = new EmployeeRepositoryImpl(new SSISdbEntities()))
                //{
                //    Employee employee = empRepo.FindEmployeeEmailId(email);
                //    return employee.Designation;
                //}

        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetCollectionList/{id}")]
        public List<CollectionList> GetCollectionList(string id)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.CollectionLists.Where(x => x.DepartmentCode == id).ToList();
            }
        }


    }
}