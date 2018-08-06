using Quartz;
using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

//-----------------------------------------------------------------
//   Author: Wong Wei Jie
//-----------------------------------------------------------------

namespace SA46Team1_Web_ADProj.Scheduler
{
    public class CreateRetrievalListJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                #region Variables
                // List of StaffReq with Open & Approved
                List<StaffRequisitionHeader> staffReqHeadList = m.StaffRequisitionHeaders
                        .Where(x => x.Status == "Open" && x.ApprovalStatus == "Approved").ToList();
                // List of StaffReq with Outstanding
                List<StaffRequisitionHeader> srhWithBO = m.StaffRequisitionHeaders.Where(x => x.Status == "Outstanding").ToList();

                Dictionary<Item, int> itemAndQty = new Dictionary<Item, int>();
                List<StaffRequisitionDetail> srdList = new List<StaffRequisitionDetail>();
                List<StaffRequisitionDetail> srdWithBackOrders = new List<StaffRequisitionDetail>();
                Dictionary<Item, List<string>> listOfItemsAndColAdded = new Dictionary<Item, List<string>>();
                List<string> colpt = new List<string>();
                List<StaffRequisitionDetail> allsrd = new List<StaffRequisitionDetail>();
                #endregion

                #region Prepare new StaffRequisitionDetails to add
                // Search for all Outstanding StaffReqDetails
                foreach (StaffRequisitionHeader srhBO in srhWithBO)
                {
                    List<StaffRequisitionDetail> SRD = m.StaffRequisitionDetails
                        .Where(x => x.FormID == srhBO.FormID && x.QuantityBackOrdered > 0).ToList();
                    if (SRD != null)
                    {
                        foreach (StaffRequisitionDetail sr in SRD)
                        {
                            srdWithBackOrders.Add(sr);
                        }
                    }
                }

                // Search for all Open && Approved StaffReqDetails
                foreach (StaffRequisitionHeader srhInWeek in staffReqHeadList)
                {
                    srhInWeek.Status = "Outstanding";
                    m.SaveChanges();

                    // Find List of StaffReqDetails based on Header ID
                    List<StaffRequisitionDetail> SRD = m.StaffRequisitionDetails
                        .Where(x => x.FormID == srhInWeek.FormID).ToList();
                    if (SRD != null)
                    {
                        foreach (StaffRequisitionDetail sr in SRD)
                        {
                            srdList.Add(sr);
                        }
                    }
                }
                #endregion

                // Check if there's any entries to collate
                if (srdList.Count() > 0 || srdWithBackOrders.Count() > 0)
                {
                    #region Create StockRetrievalHeader
                    // Create StockRetrievalHeader
                    StockRetrievalHeader newsrh = new StockRetrievalHeader();
                    int newSRH = m.StockRetrievalHeaders.Count() + 1;
                    string srhId = CommonLogic.SerialNo(newSRH, "StoR");
                    newsrh.ID = srhId;
                    newsrh.Date = DateTime.Now;
                    newsrh.Disbursed = 0;
                    newsrh.AllItemsRetrieved = 0;
                    m.StockRetrievalHeaders.Add(newsrh);
                    //m.SaveChanges();
                    #endregion

                    #region Confirming how many Items & quantity to collate
                    // Collate items based on Backorder SRDs (Total up items and its quantity to collect)
                    if (srdWithBackOrders.Count() > 0)
                    {
                        foreach (StaffRequisitionDetail srdlist in srdWithBackOrders)
                        {
                            Item item = m.Items.Where(x => x.ItemCode == srdlist.ItemCode).FirstOrDefault();
                            if (!itemAndQty.ContainsKey(item))
                            {
                                itemAndQty.Add(item, srdlist.QuantityBackOrdered);
                            }
                            else
                            {
                                itemAndQty[item] += srdlist.QuantityBackOrdered;
                            }
                        }
                    }

                    // Collate items based on new SRD (Total up items and its quantity to collect)
                    if (srdList.Count() > 0)
                    {
                        foreach (StaffRequisitionDetail srdlist in srdList)
                        {
                            Item item = m.Items.Where(x => x.ItemCode == srdlist.ItemCode).FirstOrDefault();
                            if (!itemAndQty.ContainsKey(item))
                            {
                                itemAndQty.Add(item, srdlist.QuantityOrdered);
                            }
                            else
                            {
                                itemAndQty[item] += srdlist.QuantityOrdered;
                            }
                        }
                    }

                    // Check if quantity is sufficient (If insufficient, can only collect based on Quantity on Hand)
                    foreach (Item itemToCollate in itemAndQty.Keys.ToList())
                    {
                        if (itemToCollate.Quantity < itemAndQty[itemToCollate])
                        {
                            itemAndQty[itemToCollate] = itemToCollate.Quantity;
                        }
                    }
                    #endregion

                    #region Fulfill Backorders
                    // Fulfill all backorders first
                    if (srdWithBackOrders.Count() > 0)
                    {
                        foreach (StaffRequisitionDetail retrievalListWithBO in srdWithBackOrders)
                        {
                            Item itemRetrieved = m.Items.Where(x => x.ItemCode == retrievalListWithBO.ItemCode).FirstOrDefault();
                            Bin bin = m.Bins.Where(x => x.ItemCode == retrievalListWithBO.ItemCode).FirstOrDefault();
                            StaffRequisitionHeader srhDeptCode = m.StaffRequisitionHeaders.Where(x => x.FormID == retrievalListWithBO.FormID).FirstOrDefault();
                            DepartmentDetail dd = m.DepartmentDetails.Where(x => x.DepartmentCode == srhDeptCode.DepartmentCode).FirstOrDefault();
                            StockRetrievalDetail newsrd = new StockRetrievalDetail();
                            int qtyAvailable = itemAndQty[itemRetrieved];

                            // Only create entries with qty available
                            if (qtyAvailable > 0)
                            {
                                // Create quantity requested that is really available for that item
                                int qtyToAdd = 0;
                                if (qtyAvailable > retrievalListWithBO.QuantityBackOrdered)
                                {
                                    qtyToAdd = retrievalListWithBO.QuantityBackOrdered;
                                }
                                else
                                {
                                    qtyToAdd = qtyAvailable;
                                }

                                if (listOfItemsAndColAdded.ContainsKey(itemRetrieved))
                                {
                                    if (!listOfItemsAndColAdded[itemRetrieved].Contains(dd.CollectionPointID))
                                    {
                                        // If its an entirely new Item for the collection point of the SRD, create a new entry
                                        newsrd.Id = srhId;
                                        newsrd.Bin = bin.Number;
                                        newsrd.ItemCode = retrievalListWithBO.ItemCode;
                                        newsrd.QuantityRetrieved = qtyToAdd;
                                        newsrd.CollectionPointID = dd.CollectionPointID;
                                        newsrd.QuantityAdjusted = 0;
                                        newsrd.Remarks = "";
                                        newsrd.Collected = 0;
                                        m.StockRetrievalDetails.Add(newsrd);
                                        allsrd.Add(retrievalListWithBO);

                                        // Prepare list for iteration of repeated entries (Indicate that there's exisiting record of Item & Collection point)
                                        List<string> existingColpt = listOfItemsAndColAdded[itemRetrieved];
                                        existingColpt.Add(dd.CollectionPointID);
                                        listOfItemsAndColAdded[itemRetrieved] = existingColpt;
                                    }
                                    else
                                    {
                                        // If there's existing entry (Based on previous collection point / back orders created), add to entry
                                        StockRetrievalDetail existingSRD = m.StockRetrievalDetails
                                            .Where(x => x.Id == srhId && x.ItemCode == itemRetrieved.ItemCode
                                            && x.CollectionPointID == dd.CollectionPointID).FirstOrDefault();
                                        existingSRD.QuantityRetrieved += qtyToAdd;
                                        allsrd.Add(retrievalListWithBO);
                                    }
                                }
                                else
                                {
                                    // If its an entirely new Item for the collection point of the SRD, create a new entry
                                    newsrd.Id = srhId;
                                    newsrd.Bin = bin.Number;
                                    newsrd.ItemCode = retrievalListWithBO.ItemCode;
                                    newsrd.QuantityRetrieved = qtyToAdd;
                                    newsrd.CollectionPointID = dd.CollectionPointID;
                                    newsrd.QuantityAdjusted = 0;
                                    newsrd.Remarks = "";
                                    newsrd.Collected = 0;
                                    m.StockRetrievalDetails.Add(newsrd);
                                    allsrd.Add(retrievalListWithBO);
                                    colpt = new List<string>();
                                    colpt.Add(dd.CollectionPointID);
                                    listOfItemsAndColAdded.Add(itemRetrieved, colpt);
                                }

                                //Remove quantity added from dictionary
                                itemAndQty[itemRetrieved] -= qtyToAdd;
                                m.SaveChanges();
                            }
                        }
                    }
                    #endregion

                    #region Fulfill new entries
                    // Create StockRetrievalDetails
                    if (srdList.Count() > 0)
                    {
                        foreach (StaffRequisitionDetail retrievalList in srdList)
                        {
                            Item itemRetrieved = m.Items.Where(x => x.ItemCode == retrievalList.ItemCode).FirstOrDefault();
                            Bin bin = m.Bins.Where(x => x.ItemCode == retrievalList.ItemCode).FirstOrDefault();
                            StaffRequisitionHeader srhDeptCode = m.StaffRequisitionHeaders
                                .Where(x => x.FormID == retrievalList.FormID).FirstOrDefault();
                            DepartmentDetail dd = m.DepartmentDetails
                                .Where(x => x.DepartmentCode == srhDeptCode.DepartmentCode).FirstOrDefault();
                            StockRetrievalDetail newsrd = new StockRetrievalDetail();
                            int qtyAvailable = itemAndQty[itemRetrieved];

                            // Only create entries with qty available
                            if (qtyAvailable > 0)
                            {
                                // Create quantity requested that is really available for that item
                                int qtyToAdd = 0;
                                if (qtyAvailable > retrievalList.QuantityOrdered)
                                {
                                    qtyToAdd = retrievalList.QuantityOrdered;
                                }
                                else
                                {
                                    qtyToAdd = qtyAvailable;
                                }

                                if (listOfItemsAndColAdded.ContainsKey(itemRetrieved))
                                {
                                    if (!listOfItemsAndColAdded[itemRetrieved].Contains(dd.CollectionPointID))
                                    {
                                        // If its an entirely new Item for the collection point of the SRD, create a new entry
                                        newsrd.Id = srhId;
                                        newsrd.Bin = bin.Number;
                                        newsrd.ItemCode = retrievalList.ItemCode;
                                        newsrd.QuantityRetrieved = qtyToAdd;
                                        newsrd.CollectionPointID = dd.CollectionPointID;
                                        newsrd.QuantityAdjusted = 0;
                                        newsrd.Remarks = "";
                                        newsrd.Collected = 0;
                                        m.StockRetrievalDetails.Add(newsrd);
                                        allsrd.Add(retrievalList);

                                        // Prepare list for iteration of repeated entries (Indicate that there's exisiting record of Item & Collection point)
                                        List<string> existingColpt = listOfItemsAndColAdded[itemRetrieved];
                                        existingColpt.Add(dd.CollectionPointID);
                                        listOfItemsAndColAdded[itemRetrieved] = existingColpt;
                                    }
                                    else
                                    {
                                        // If there's existing entry (Based on previous collection point / back orders created), add to entry
                                        StockRetrievalDetail existingSRD = m.StockRetrievalDetails
                                            .Where(x => x.Id == srhId && x.ItemCode == itemRetrieved.ItemCode
                                            && x.CollectionPointID == dd.CollectionPointID).FirstOrDefault();
                                        existingSRD.QuantityRetrieved += qtyToAdd;
                                        allsrd.Add(retrievalList);
                                    }
                                }
                                else
                                {
                                    // If its an entirely new Item for the collection point of the SRD, create a new entry
                                    newsrd.Id = srhId;
                                    newsrd.Bin = bin.Number;
                                    newsrd.ItemCode = retrievalList.ItemCode;
                                    newsrd.QuantityRetrieved = qtyToAdd;
                                    newsrd.CollectionPointID = dd.CollectionPointID;
                                    newsrd.QuantityAdjusted = 0;
                                    newsrd.Remarks = "";
                                    newsrd.Collected = 0;
                                    m.StockRetrievalDetails.Add(newsrd);
                                    allsrd.Add(retrievalList);

                                    colpt = new List<string>();
                                    colpt.Add(dd.CollectionPointID);
                                    listOfItemsAndColAdded.Add(itemRetrieved, colpt);
                                }

                                // Remove qty that is added
                                itemAndQty[itemRetrieved] -= qtyToAdd;
                                m.SaveChanges();
                            }

                        }
                    }
                    #endregion

                    #region Create StockRetrievalReqForm
                    List<String> allNewlyCreatedReqFormId = new List<String>();

                    foreach (StaffRequisitionDetail allNewlyCreated in allsrd)
                    {
                        allNewlyCreatedReqFormId.Add(allNewlyCreated.FormID);
                    }

                    //To remove duplicate req form id
                    allNewlyCreatedReqFormId = allNewlyCreatedReqFormId.Distinct().ToList();

                    foreach (String newReqFormId in allNewlyCreatedReqFormId)
                    {
                        // New StockRetrievalReqForm
                        StockRetrievalReqForm srrf = new StockRetrievalReqForm();
                        //srrf.ReqFormID = allNewlyCreated.FormID;
                        srrf.ReqFormID = newReqFormId;
                        srrf.StockRetrievalID = srhId;
                        m.StockRetrievalReqForms.Add(srrf);
                        m.SaveChanges();
                    }
                    #endregion

                }
            }
            return null;
        }        
    }
}