using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eConnect.Model;
using eConnect.Logic;
using System.IO;
using System.Configuration;
using System.Globalization;

namespace eConnect.Application.Controllers
{
    public class AdminController : Controller
    {
        string RootFilePath = System.Web.HttpContext.Current.Server.MapPath(Convert.ToString(ConfigurationManager.AppSettings["RootFilePath"]));
        string UploaderFilePath = System.Web.HttpContext.Current.Server.MapPath(Convert.ToString(ConfigurationManager.AppSettings["UploaderFiles"]));
        List<SelectListItem> ddlMonths = new List<SelectListItem>();
        List<SelectListItem> ddlYears = new List<SelectListItem>();
        public ActionResult ApproveCSP()
        {
            UserLogic ul = new UserLogic();
            var users = ul.GetUsersInfoByStatus(1);
            ViewBag.Users = users;
            return View();
        }
        public ActionResult ViewCSPProfile(string id)
        {
            if (id != null)
            {
                string decrypt = CommonLogic.DecryptText(id.ToString());
                UserLogic ul = new UserLogic();
                var model = ul.GetUsersDetailsByid(Convert.ToInt32(decrypt));
                return View(model);
            }
            else
            {
                return null;
            }
        }
        public FileResult OpenFile(string id, string filename, string filetype)
        {
            string fullpath = string.Empty;
            if (filename != null || filename != "")
            {
                try
                {

                    fullpath = "~/UploadedFiles//Proofs//" + CommonLogic.DecryptText(id.ToString()) + "//" + filetype + "//" + filename;
                    return File(new FileStream(Server.MapPath(fullpath), FileMode.Open), "application/octetstream", filename);
                }
                catch (Exception ex)
                {
                    return File(new FileStream(Server.MapPath(fullpath), FileMode.Open), "application/octetstream", filename);

                    // throw ex;
                }
            }
            else
            {
                return null;
            }

        }
        public FileResult OpenFileNew(string id, string filename, string filetype)
        {
            string fullpath = string.Empty;
            if (filename != null || filename != "")
            {
                try
                {
                    fullpath = "~/UploadedFiles//Proofs//" + id.ToString() + "//" + filetype + "//" + filename;
                    return File(new FileStream(Server.MapPath(fullpath), FileMode.Open), "application/octetstream", filename);
                }
                catch (Exception ex)
                {
                    return File(new FileStream(Server.MapPath(fullpath), FileMode.Open), "application/octetstream", filename);

                    // throw ex;
                }
            }
            else
            {
                return null;
            }

        }


        string FolderImagesPath = System.Web.HttpContext.Current.Server.MapPath(Convert.ToString(ConfigurationManager.AppSettings["FolderImages"]));

        public ActionResult UploadFolderImages()
        {
            FolderDetailsLogic folderdetail = new FolderDetailsLogic();
            var folderlist = folderdetail.GetAllFolderDetails();
            ViewBag.Allfolderlist = folderlist;
            return View();
        }


        [HttpPost]
        public ActionResult UploadFolderImages(FolderDetailsModel model)
        {

            string fpath = string.Empty;
            string path = Path.Combine(FolderImagesPath, model.FolderName.ToString());
            if (model.FolderImage != null)
            {
                fpath = CheckDirectory(path, model.FolderImage);
                model.FolderImage.SaveAs(fpath);

                TempData["Message"] = "Image Name: " + model.FolderImage.FileName + " " + "has been successfully uploaded to folder" + model.FolderName;
            }

            return RedirectToAction("UploadFolderImages");
        }
        public string CheckDirectory(string path, HttpPostedFileBase postedfile)
        {
            string fullpath = string.Empty;
            // fullpath = Path.Combine(path, filetype);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            fullpath = path + "\\" + Path.GetFileName(postedfile.FileName);
            return fullpath;
        }
        public ActionResult ShowFolderImages()
        {
            int id = 2;
            FolderDetailsLogic folderdetail = new FolderDetailsLogic();
            var model = folderdetail.GetAllFolderDetailsById(id);
            return View(model);
        }
        public ActionResult Uploader()
        {
            RequestTypeLogic Rtypes = new RequestTypeLogic();
            var RtypesList = Rtypes.GetAllRequestTypes();
            ViewBag.RequestTypes = RtypesList;
            ViewBag.Years = GetYears().OrderByDescending(x => x.Value);
            ViewBag.Months = GetMonths().OrderByDescending(x => x.Value);
            return View();
        }
        [HttpPost]
        public ActionResult Uploader(UploaderModel model)
        {
            string path = UploaderFilePath;
            string fpath = string.Empty;
            UploaderLogic uploaderLogic = new UploaderLogic();
            long userID = uploaderLogic.InsertUploader(model);
            if (model.fileupload != null)
            {
                fpath = CheckUploaderDirectory(path, model);
                model.fileupload.SaveAs(fpath);
            }
            TempData["Message"] = "Record submitted successfully.";
            return RedirectToAction("Uploader");
        }
        public ActionResult UploaderDetails(UploaderModel model)
        {
            RequestTypeLogic Rtypes = new RequestTypeLogic();
            var RtypesList = Rtypes.GetAllRequestTypes();
            ViewBag.RequestTypes = RtypesList;
            ViewBag.Years = GetYears().OrderByDescending(x => x.Value);
            ViewBag.Months = GetMonths().OrderByDescending(x => x.Value);
            return View();
        }
        [HttpPost]
        public ActionResult UploaderDetails(string RequestType, string year, string month)
        {
            List<UploaderModel> reportsList = new List<UploaderModel>();
            UploaderLogic upmodel = new UploaderLogic();
            var Reqlist = upmodel.GetAllUploaderDetails().ToList();
            foreach (var item in Reqlist)
            {
                reportsList.Add(
                               new UploaderModel
                               {
                                   UploaderId = item.UploaderId,
                                   Year = item.Year,
                                   Month = item.Month,
                                   ReportType = item.ReportType,
                                   ApplyTDS = item.ApplyTDS,
                                   FileName = item.FileName,
                                   UpdatedDate = (DateTime)item.UpdatedDate,
                                   ReportTypeName = item.tblReportType.Name,
                                   MonthName = CommonLogic.GetMonthName(Convert.ToInt32(item.Month)),
                               });
            }
            return PartialView("_UploaderList", reportsList);
        }
        private SelectList GetMonths()
        {
            var months = Enumerable.Range(1, 12).Select(i => new
            {
                A = i,
                B = DateTimeFormatInfo.CurrentInfo.GetMonthName(i)
            });
            int CurrentMonth = 1; //January  
            {
                CurrentMonth = DateTime.Now.Month;
                months = Enumerable.Range(1, CurrentMonth).Select(i => new
                {
                    A = i,
                    B = DateTimeFormatInfo.CurrentInfo.GetMonthName(i)
                });
            }
            foreach (var item in months)
            {
                ddlMonths.Add(new SelectListItem { Text = item.B.ToString(), Value = item.A.ToString() });
            }
            return new SelectList(ddlMonths, "Value", "Text", CurrentMonth);
        }
        private SelectList GetYears()
        {
            int CurrentYear = DateTime.Now.Year;
            for (int i = 2010; i <= CurrentYear; i++)
            {
                ddlYears.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });
            }
            return new SelectList(ddlYears, "Value", "Text");
        }

        public string CheckUploaderDirectory(string path, UploaderModel model)
        {
            string fullpath = string.Empty;
            fullpath = Path.Combine(path, model.ReportType.ToString(), model.Year.ToString(), model.Month.ToString());
            if (!Directory.Exists(fullpath))
            {
                Directory.CreateDirectory(fullpath);
            }
            fullpath = fullpath + "\\" + Path.GetFileName(model.fileupload.FileName);
            return fullpath;
        }
        public ActionResult ApplicationSetting(UploaderModel model)
        {
            BusinessLogic BusinessList = new BusinessLogic();
            var BList = BusinessList.GetAllBusiness();
            ViewBag.BusinessList = BList;
            return View();
        }
        [HttpPost]
        public ActionResult ApplicationSetting(string type)
        {
            if (type == "Search")
            {
                List<ApplicationSettingModel> products = new List<ApplicationSettingModel>();
                ApplicationSettingLogic aplogic = new ApplicationSettingLogic();
                var Applist = aplogic.GetAllApplicationSetting().ToList();
                foreach (var item in Applist)
                {
                    products.Add(
                                   new ApplicationSettingModel
                                   {
                                       ApplicationName = item.ApplicationName,
                                       SettingId = item.SettingId,
                                       BusinessId = item.BusinessId,
                                       AutoBackUp = item.AutoBackUp,
                                       AutoBackUpDuration = item.AutoBackUpDuration,
                                   });
                }
                return PartialView("_ApplicationSetting", products);
            }
            else
            {
                BusinessLogic BusinessList = new BusinessLogic();
                var BList = BusinessList.GetAllBusiness();
                ViewBag.BusinessList = BList;
                return PartialView("_AddApplicationSetting");
            }
        }
        public ActionResult _AddApplicationSetting()
        {
            return PartialView("_AddApplicationSetting");
        }
        public FileResult OpenFileUploader(int id, int year, int month, string filename)
        {
            string path = UploaderFilePath;
            string fullpath = string.Empty;
            //fullpath = UploaderFilePath;
            if (id > 0)
            {
                try
                {
                    fullpath = "~/UploaderFiles//" + id.ToString() + "//" + year + "//" + month + "//" + filename;
                    return File(new FileStream(Server.MapPath(fullpath), FileMode.Open), "application/octetstream", filename);
                }
                catch (Exception ex)
                {
                    return File(new FileStream(Server.MapPath(fullpath), FileMode.Open), "application/octetstream", filename);
                }
            }
            else
            {
                return null;
            }

        }


        public ActionResult AccountConfiguration()
        {
            BusinessLogic BusinessList = new BusinessLogic();
            var BList = BusinessList.GetAllBusiness();
            ViewBag.BusinessList = BList;
            return View();
        }
        [HttpPost]
        public ActionResult AccountConfiguration(string type)
        {
            if (type == "Search")
            {
                List<AccountConfigurationModel> products = new List<AccountConfigurationModel>();
                AccountConfigurationLogic aplogic = new AccountConfigurationLogic();
                var Applist = aplogic.GetAllAccountConfiguration().ToList();
                foreach (var item in Applist)
                {
                    products.Add(
                                   new AccountConfigurationModel
                                   {
                                       ConfigurationId = item.ConfigurationId,
                                       BusinessId = (int)item.BusinessId,
                                       DeactiveAccountDaysForInvalidPwd = (int)item.DeactiveAccountDaysForInvalidPwd,
                                       LockAccountDaysForInvalidPwd = (int)item.LockAccountDaysForInvalidPwd,
                                       AutoUnlockAccountMinutes = (int)item.AutoUnlockAccountMinutes,
                                       PasswordAutoExpireInDays = (int)item.PasswordAutoExpireInDays,
                                       IsPasswordNeverExpired = (bool)item.IsPasswordNeverExpired,
                                       ChangedPwdOnNextLogin = (bool)item.ChangedPwdOnNextLogin,
                                       PasswordLength = (int)item.PasswordLength,
                                       NotifiedToCSP = (bool)item.NotifiedToCSP,
                                   });
                }
                return PartialView("_AccountConfigurationDetails", products);
            }
            else
            {
                BusinessLogic BusinessList = new BusinessLogic();
                var BList = BusinessList.GetAllBusiness();
                ViewBag.BusinessList = BList;
                return PartialView("_AddApplicationSetting");
            }
        }

        public ActionResult _AddAccountConfiguration()
        {
            return PartialView("_AddAccountConfiguration");
        }

        [HttpPost]
        public ActionResult _AddAccountConfiguration(AccountConfigurationModel model)
        {
            AccountConfigurationLogic cl = new AccountConfigurationLogic();
            if (model.BusinessId > 0)
            {
                cl.InsertAccountConfiguration(model);
                TempData["Message"] = "Record Updated successfully.";
                return RedirectToAction("SAdmin", "TechSupportReqDetails");

            }
            return PartialView("_AddAccountConfiguration");
        }


        //[HttpGet]
        //public ActionResult _EditAccountConfiguration(int id)
        //{
        //    AccountConfigurationLogic cl = new AccountConfigurationLogic();
        //    var aconfig = cl.GetAccountConfigurationById(id);
        //    return PartialView("_EditAccountConfiguration", aconfig);
        //}

        [HttpPost]
        public ActionResult _EditAccountConfiguration(int id, AccountConfigurationModel model,string btnname)
        {
            if (btnname=="Update")
            {
                AccountConfigurationLogic cl = new AccountConfigurationLogic();
                cl.UpdateAccountConfigurationBy(model);
                return PartialView("_EditAccountConfiguration");
            }
            else
            {
                AccountConfigurationLogic cl = new AccountConfigurationLogic();
                var aconfig = cl.GetAccountConfigurationById(id);
                return PartialView("_EditAccountConfiguration", aconfig);
            }
        }

    }
}
