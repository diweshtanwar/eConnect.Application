using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eConnect.Model;
using eConnect.Logic;
using System.IO;
using System.Configuration;
using System.Threading.Tasks;

namespace eConnect.Application.Controllers
{
    public class SAdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TechSupportReqDetails()
        {
            TechSupportRequestLogic cl = new TechSupportRequestLogic();
            var Reqlist = cl.GetAllTechSupportRequests();
            ViewBag.AllTecRequests = Reqlist;

            StatusLogic sl = new StatusLogic();
            var Statuslist = sl.GetAllStatus();
            ViewBag.Statuslist = Statuslist;

            return View();
        }
        public ActionResult ManageUser()
        {
            return View();
        }
        //public ActionResult _BindSupportRequest()
        //{
        //    List<TechSupportRequestModel> products = new List<TechSupportRequestModel>();
        //    TechSupportRequestLogic cl = new TechSupportRequestLogic();
        //    var Reqlist = cl.GetAllTechSupportRequestsByFilterNew().ToList();
        //    foreach (var item in Reqlist)
        //    {
        //        products.Add(
        //                       new TechSupportRequestModel
        //                       {
        //                           TechSupportReqID = item.TechRequestId,
        //                           // UserID = item.RaisedBy,
        //                           //  UserName = item.User.CSPName,
        //                           MobileNumber = Convert.ToInt64(item.MobileNo),
        //                           //RequestDate = item.RequestedDate.ToString("dd-MMMM-yyyy"),
        //                           Status = (int)item.Status,
        //                       });
        //    }
        //    //return View("_BindSupportRequest", products);
        //    return PartialView("_BindSupportRequest", products);
        //}
        [HttpPost]
        public ActionResult TechSupportReqDetails(string ReqID, string CSPName, string CSPID, string Reqtype, string Status, string From, string To)
        {
            List<TechSupportRequestModel> products = new List<TechSupportRequestModel>();
            TechSupportRequestLogic cl = new TechSupportRequestLogic();
            var Reqlist = cl.GetAllTechSupportRequestsByFilter(ReqID, CSPName, CSPID, Reqtype, Status, From, To).ToList();
            foreach (var item in Reqlist)
            {
                products.Add(
                               new TechSupportRequestModel
                               {
                                   TechSupportReqID = item.TechRequestId,
                                   Description = item.Description,
                                   RaisedByName = item.tblCSPDetail.CSPCode + " (" + item.tblCSPDetail.BranchCode + ")",
                                   TechProbName = item.tblProblemType.Name,
                                   MobileNo = item.MobileNo,
                                   RequestedDate = (DateTime)item.RequestedDate,
                                   RequestDate = item.RequestedDate.ToString(),
                                   //Status = (int)item.Status,
                                   Status = item.Status,
                                   StatusName = item.tblStatu.Name
                               });
            }
            return PartialView("_BindSupportRequest", products);
        }

        [HttpPost]
        public ActionResult _EditRequest(TechSupportRequestModel model, long reqid, string btnSubmit)
        {
            TechSupportRequestLogic cl = new TechSupportRequestLogic();
            if (btnSubmit == "Submit")
            {
                cl.UpdateTechSupportRequest(model);
                TempData["Message"] = "Record Updated successfully.";
                return RedirectToAction("SAdmin", "TechSupportReqDetails");

            }
            else
            {
                TechSupportProblemLogic ProbList = new TechSupportProblemLogic();
                var ProblemList = ProbList.GetAllTechSupportProblems();
                StatusLogic StatsList = new StatusLogic();
                var StatusList = StatsList.GetAllStatus();
                //TechSupportRequestLogic cl = new TechSupportRequestLogic();
                var ReqDetails = cl.GetTechSupportRequestsByTReqId(reqid);
                ViewBag.ProblemList = new SelectList(ProblemList, "ProblemTypeId", "Name", ReqDetails.ProblemType);
                ViewBag.StatusList = new SelectList(StatusList, "StatusId", "Name", ReqDetails.Status);
                ViewBag.ReqDetails = ReqDetails;
                return PartialView("_EditRequest");

            }



        }

        [HttpPost]
        public ActionResult _TecRequestHistory(long reqid)
        {
            TechSupportRequestLogic cl = new TechSupportRequestLogic();
            //TechSupportProblemLogic ProbList = new TechSupportProblemLogic();
            //var ProblemList = ProbList.GetAllTechSupportProblems();
            //StatusLogic StatsList = new StatusLogic();
            //var StatusList = StatsList.GetAllStatus();
            ////TechSupportRequestLogic cl = new TechSupportRequestLogic();
            var ReqDetails = cl.GetTechSupportRequestsByTReqId(reqid);
            //ViewBag.ProblemList = new SelectList(ProblemList, "ProblemTypeId", "Name", ReqDetails.ProblemType);
            //ViewBag.StatusList = new SelectList(StatusList, "StatusId", "Name", ReqDetails.Status);

            ViewBag.ReqDetails = ReqDetails;
            return PartialView("_TecRequestHistory");
        }


        public ActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateRole(RoleMasterModel model)
        {
            RoleMasterLogic RMaster = new RoleMasterLogic();
            RMaster.InsertRoleMaster(model);
            TempData["Message"] = "Record updated successfully.";
            return RedirectToAction("CreateRole");
        }
        public ActionResult RoleDetails()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RoleDetails(string rolename)
        {
            List<RoleMasterModel> roles = new List<RoleMasterModel>();
            RoleMasterLogic cl = new RoleMasterLogic();
            var Rolelist = cl.GetAllRoles().ToList();
            if(!string.IsNullOrEmpty(rolename))
            {
                Rolelist = Rolelist.Where(x => x.Name == rolename).ToList();
            }
            foreach (var item in Rolelist)
            {
                roles.Add(
                               new RoleMasterModel
                               {
                                   RoleId = item.RoleId,
                                   Name = item.Name,
                                   
                               });
            }
            return PartialView("_BindRoles", roles);
        }

    }
}