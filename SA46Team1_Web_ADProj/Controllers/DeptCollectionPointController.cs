using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Dept/DeptCollectionPoint")]
    public class DeptCollectionPointController : Controller
    {

        [Route("CollectionPoint")]
        public ActionResult CollectionPoint()
        {
            //get collection point id belonging to dept
            using (SSISdbEntities e = new SSISdbEntities()) {
                string deptCode = Session["DepartmentCode"].ToString();
                string collectionPointId = e.DepartmentDetails.Where(x => x.DepartmentCode == deptCode).Select(x => x.CollectionPointID).FirstOrDefault();
                ViewBag.collectionPointId = collectionPointId;
            }
            return View();
        }

        [HttpPost]
        public RedirectToRouteResult UpdateCollectionPoint(string[] collectionPointIds, string[] collectionPointDefault)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                //update dept's collection point
                int index = collectionPointDefault.ToList<String>().IndexOf("True");
                string newDefaultCPid = collectionPointIds[index];
                string deptCode = Session["DepartmentCode"].ToString();
                DepartmentDetail detail = e.DepartmentDetails.Where(x => x.DepartmentCode == deptCode).FirstOrDefault();
                detail.CollectionPointID = newDefaultCPid;

                DAL.DepartmentDetailsRepositoryImpl dal = new DAL.DepartmentDetailsRepositoryImpl(e);
                dal.UpdateDepartmentDetail(detail);

                e.SaveChanges();

                return RedirectToAction("CollectionPoint", "Dept");
            }
        }
    }
}
