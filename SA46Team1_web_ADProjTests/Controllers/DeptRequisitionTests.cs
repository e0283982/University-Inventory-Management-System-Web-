using Microsoft.VisualStudio.TestTools.UnitTesting;
using SA46Team1_Web_ADProj.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA46Team1_Web_ADProj.Controllers.Tests
{
    [TestClass()]
    public class DeptRequisitionTests
    {
        [TestMethod()]
        public void AddNewReqItemTest()
        {
            //Item itemToAdd = new Item();

            //using (SSISdbEntities e = new SSISdbEntities())
            //{
            //    string itemCode = Request.Form["SelectItemDesc"].ToString();
            //    itemToAdd = e.Items.Where(x => x.ItemCode == itemCode).FirstOrDefault();

            //    List<StaffRequisitionDetail> list = new List<StaffRequisitionDetail>();
            //    list = (List<StaffRequisitionDetail>)Session["newReqList"];
            //    StaffRequisitionDetail srd = new StaffRequisitionDetail();
            //    srd.ItemCode = itemToAdd.ItemCode;
            //    srd.FormID = Session["currentFormId"].ToString();
            //    srd.QuantityOrdered = item2.QuantityOrdered;
            //    srd.QuantityDelivered = 0;
            //    srd.QuantityBackOrdered = item2.QuantityOrdered;
            //    srd.CancelledBackOrdered = 0;

            //    srd.Item = e.Items.Where(x => x.ItemCode == itemToAdd.ItemCode).FirstOrDefault();

            //    srd.Item.Description = itemToAdd.Description;
            //    srd.Item.UoM = itemToAdd.UoM;

            //    list.Add(srd);
            //    Session["newReqList"] = list;

            //    //add to list meant for already added items
            //    List<String> tempList = (List<String>)Session["tempList"];
            //    tempList.Add(itemToAdd.ItemCode);
            //    Session["tempList"] = tempList;

            //    List<Item> newItemList = new List<Item>();

            //    return RedirectToAction("Requisition", "Dept");
            //}


        }

        [TestMethod()]
        public void SubmitNewRequestFormTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DiscardNewItemsTest()
        {
            Assert.Fail();
        }
    }
}