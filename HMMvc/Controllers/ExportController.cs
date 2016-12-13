//using Spire.Pdf;
using BL;
using BL.Moduls;
using Common;
using Microsoft.Practices.Unity;
using MvcBlox.Models;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace MVC.Controllers
{

    public class PdfModel
    {
        public string Lang { get; set; }
        public string Dir { get; set; }
        public string EmpID { get; set; }
        public int? ClientID { get; set; }

    }

    public class ExportController : Controller
    {

        private readonly VbBL _vbBL;
        private readonly JobBL _jobBL;
        private readonly IGlobalManager _globalManager;
        private readonly QuoteVersionBL _quoteVersionBL;
        private readonly JobAlignmentBL _jobAlignmentBL;
        private readonly ContactInfoModule _contactInfoModule;

        public ExportController([Dependency]VbBL vbBL, 
            [Dependency]QuoteVersionBL quoteVersionBL,
            [Dependency]JobAlignmentBL jobAlignmentBL,
            [Dependency] IGlobalManager globalManager,
             [Dependency]ContactInfoModule contactInfoModule,
            [Dependency]JobBL jobBL)
        {
            _globalManager = globalManager;
            _vbBL = vbBL;
            _jobBL = jobBL;
            _quoteVersionBL = quoteVersionBL;
            _jobAlignmentBL = jobAlignmentBL;
            _contactInfoModule = contactInfoModule;
        }


        #region HTML
        public ActionResult VbReportHtml(int id, int[] ids, bool disableHf = false, string lang = null, string dir = null, string empID = null)
        {
            SetLang(lang, dir);
         
            var rprt = _vbBL.GetVbReport(id, IsEnglish());

            if (rprt == null || (empID == null && !_globalManager.AuthorizeUser(rprt.JobDM.ClientID)))
                return Redirect("/Home/NotValidate");

            if (ids != null)
                RprtWithFilter(rprt, ids);

            ViewBag.disableHF = disableHf;
            ViewBag.ClientID = rprt.JobDM.ClientID;

            return PartialView(rprt);


        }

        private bool IsEnglish()
        {
            return Session["dir"].ToString() == "False";
        }

        public ActionResult QuoteExport(int id, bool disableHf = false, string lang = null, string dir = null, string empID = null)
        {
            SetLang(lang, dir);
            var model = _quoteVersionBL.GetSingleItemDM(id);
            ViewBag.EmpSignture = PicHelper.GetEmployeeSignture(model.QuoteDM.CreatorID);
            ViewBag.Creator = model.QuoteDM.Creator;

           var clientID = model.QuoteDM.ClientID;


            //if (model == null || (EmpID == null && !_globalManager.AuthorizeUser(ClientID)))
            //    return Redirect("/Home/NotValidate");


            ViewBag.disableHF = disableHf;
            ViewBag.ClientID = clientID;

            return PartialView(model);
        }

        public ActionResult AlignmentExport(int id, bool disableHf = false, string lang = null, string dir = null, string empID = null)
        {
            SetLang(lang, dir);
           
            var model = _jobAlignmentBL.GetSingleItemDM(id);

            ViewBag.EmpSignture =  PicHelper.GetEmployeeSignture((int)model.JobDM.CreatorID);
            ViewBag.Creator = model.JobDM.Creator;

            var clientID = model.JobDM.ClientID;

            ViewBag.disableHF = disableHf;
            ViewBag.ClientID = clientID;

            return PartialView(model);
        }


        #endregion

        #region Email

        public ActionResult QuoteEmail(int id)
        {
            ViewBag.PopTitle = "שליחת הצעה במייל";
            QuoteDM quoteInfo = _quoteVersionBL.GetQuoteDM(id);
              
            var model = new EmailDm
            {
                Id = id,
                Subject = string.Format(GlobalDM.GetTransStr("EmailQuote_Subject"), quoteInfo.QuoteTitle),
                From = _contactInfoModule.GetEmail(quoteInfo.CreatorID, ObjType.Employee),
                To = _contactInfoModule.GetEmail(quoteInfo.UserID ?? 0, ObjType.User) ,
                EmailBody = string.Format(GlobalDM.GetTransStr("EmailQuote_Body"), quoteInfo.ContactName, quoteInfo.Creator,"" )
            };

            return PartialView(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult QuoteEmail(EmailDm model, string downloadID = "")
        {
          
                var pdfModel = SetPdfModel(downloadID);

                var qs = SetPdfCollection(pdfModel, model.Id, null);

                var clientID = string.IsNullOrEmpty(pdfModel.EmpID) ? (int?)null : _quoteVersionBL.GetClientID(model.Id);

                var pdfResult = new UrlAsPdf(Url.Action(nameof(QuoteExport), "Export", null, "http") + "?" + qs)
                {
                    FileName = "Quote_" + model.Id + ".pdf",
                    CustomSwitches = GetHeaderAndFooter(pdfModel, clientID, "Quot"),
                    PageMargins = GetContentMargins(),
                };



               var binary = pdfResult.BuildPdf(this.ControllerContext);
                sendEmail(model, binary);

                return Json(new { msg = "Success" });
           

          

        }

        private void sendEmail(EmailDm model, byte[] binary)
        {
            if (string.IsNullOrEmpty(model.From))
                throw new Exception("no From address");

            _contactInfoModule.SetSenderInfo(model);


            if (string.IsNullOrEmpty(model.EmailPassword))
                throw new Exception("no To password");

            if (string.IsNullOrEmpty(model.To))
                throw new Exception("no To address");

            var email = new MailMessage
            {

                From = new MailAddress(model.From, model.Creator)//model.From
            };
            foreach (var to in model.To.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                email.To.Add(new MailAddress(to));
            }


            email.IsBodyHtml = true;
            var dir = Session["dir"].ToString() == "False" ? "ltr" : "rtl";
            email.Body = string.Format("<div dir=\"" + dir + "\">{0}</div>", model.EmailBody);

            email.Subject = model.Subject;

            if (binary != null)
            {
                var ms = new MemoryStream(binary);
                using (var attachment = new Attachment(ms, new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf)))
                {
                    email.Attachments.Add(attachment);
                }

            }

            using (var smtp = new SmtpClient
            {
                Host = ConfigurationManager.AppSettings["Smtp.Host"],
                Port = int.Parse(ConfigurationManager.AppSettings["Smtp.Port"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(model.From, model.EmailPassword)
            })
            {
                smtp.Send(email);
            }
        }

        #endregion

        #region PDF

        public ActionResult PdfTest()
        {
            var id = 1;
            var model = SetPdfModel("353");

            var qs = SetPdfCollection(model, id, null);

            var clientID = 100;

            return new UrlAsPdf(Url.Action(nameof(TestExport), "Export", null, "http") + "?" + qs)
            {
                FileName = "Quote_" + id + ".pdf",
                CustomSwitches = GetHeaderAndFooter(model, clientID, "Quot"),
                PageMargins = GetContentMargins(),
            };


        }

        public ActionResult TestExport(int id, bool disableHf = false, string lang = null, string dir = null, string empID = null)
        {
            SetLang(lang, dir);
            


            ViewBag.disableHF = disableHf;

            return PartialView();
        }




        public ActionResult QuotePdf(int id, string downloadID)
        {

            var model = SetPdfModel(downloadID);

            var qs = SetPdfCollection(model, id, null);

            var clientID = string.IsNullOrEmpty(model.EmpID) ? (int?)null : _quoteVersionBL.GetClientID(id);

            return new UrlAsPdf(Url.Action(nameof(QuoteExport), "Export", null, "http") + "?" + qs)
            {
                FileName = "Quote_" + id + ".pdf",
                CustomSwitches = GetHeaderAndFooter(model, clientID, "Quot"),
                PageMargins = GetContentMargins(),
            };

        }

        public ActionResult AlignmentPdf(int id, string date, string downloadID)
        {
             
            var model = SetPdfModel(downloadID);

            var qs = SetPdfCollection(model, id, null);
     

            var clientID = string.IsNullOrEmpty(model.EmpID) ? (int?)null : _jobAlignmentBL.GetClientID(id);

            return new UrlAsPdf(Url.Action("AlignmentExport", "Export", null, "http") + "?" + qs)
            {
                FileName = "Alignment_" + id + ".pdf",
                CustomSwitches = GetHeaderAndFooter(model, clientID, "JobType_Izunim", date),
                PageMargins = GetContentMargins(),
            };

        }

        public ActionResult VbReportPdf(int id, int[] ids,string downloadID)
        {
            var model = SetPdfModel(downloadID);

            var qs = SetPdfCollection(model, id, ids);

            var clientID = string.IsNullOrEmpty(model.EmpID) ? (int?)null : _jobBL.GetClientID(id);

            return new UrlAsPdf(Url.Action("VBReportHtml", "Export", null, "http") + "?" + qs)
            {
                FileName = "VB.pdf",
                CustomSwitches = GetHeaderAndFooter(model, clientID, "JobType_Vibration"),
                PageMargins = GetContentMargins(),
            };


        }

        /// <summary>
        /// not include header and footer (top, right, bottom,left)
        /// </summary>
        /// <returns></returns>
        private static Rotativa.Options.Margins GetContentMargins()
        {
            return new Rotativa.Options.Margins(21, 0, 50, 0);//21, 0, 50, 0
        }

        //create spacing by content margin and css
        private string GetHeaderAndFooter(PdfModel model, int? clientID, string title,string date=null)
        {
           

            return "--print-media-type --header-html \"" +
                Url.Action(nameof(HeaderExport), "Export", new { lang = model.Lang, dir = model.Dir, ClientID = clientID, date, title }, "http") +
                "\" --footer-html \"" +
                Url.Action(nameof(FooterExport), "Export", new { lang = model.Lang, dir = model.Dir, model.EmpID }, "http") +
                //"\""+
                //" --footer-right 24 " +
                // "--footer-left 23 " +
                // "--header-right 22" +
                // "--header-left 21 " +
                 //"--footer-spacing 1 " +
                 //"--header-spacing 19 "
                
                 "\" --header-spacing 0 --footer-spacing 0 " 
                //"\" --header-right 20"
                ;
        }


        private PdfModel SetPdfModel(string downloadID)
        {
            SetDownloadCookie(downloadID);

            return new PdfModel
            {
                Lang = Session["lang"].ToString(),
                Dir = Session["dir"].ToString(),
                EmpID = Session["EmpID"] != null ? Session["EmpID"].ToString() : null
            };

        }

        private void SetDownloadCookie(string downloadID)
        {
            if (string.IsNullOrEmpty(downloadID))
                return;

                var downloadCock = Request.Cookies["downloadID"];
                if (downloadCock == null)
                {
                    downloadCock = new HttpCookie("downloadID");
                    Response.Cookies.Add(downloadCock);
                }

                downloadCock.Value = downloadID;
                Response.Cookies.Set(downloadCock);

            
        }

        private static NameValueCollection SetPdfCollection(PdfModel model, int id, int[] ids)
        {
            var qs = HttpUtility.ParseQueryString("");

            //is actualy only for vb, but doesnt get in the way...
            GetVbsPfdIds(ids, qs);

            qs.Add("id", id.ToString());

            qs.Add("disableHF", true.ToString());
            qs.Add("lang", model.Lang);
            qs.Add("dir", model.Dir);
            qs.Add("EmpID", model.EmpID);

            return qs;
        }


        private static void GetVbsPfdIds(int[] ids, NameValueCollection qs)
        {
            if (ids == null)
            {
                qs.Add("ids", "nulll");
            }
            else
            {
                foreach (var id in ids)
                {
                    qs.Add("ids", id.ToString());
                };
            }
        }

        public ActionResult HeaderExport(string lang, string dir, int? clientID, string date,string title)
        {
            SetLang(lang, dir);
            ViewBag.ClientID = clientID;
            ViewBag.StartDate = date;
            ViewBag.ReportTitle = GlobalDM.GetTransStr(title);
            return PartialView();
        }

        public ActionResult FooterExport(string lang, string dir)
        {
            SetLang(lang, dir);
            return PartialView();
        }

        #endregion



        #region EXCEL
        public ActionResult VbReportExcel(int id, int[] ids, string downloadID)
        {
            SetDownloadCookie( downloadID);

            var rprt = _vbBL.GetVbReport(id,false);
            if (rprt == null || !_globalManager.AuthorizeUser(rprt.JobDM.ClientID))
                return Redirect("/Home/NotValidate");


            if (ids != null)
                RprtWithFilter(rprt, ids);

            ExportToEcxel(rprt);
            return PartialView(rprt);
        }

        public void ExportToEcxel(VbReportDM rprt)
        {
            try
            {

                Response.ClearContent();
                var fileName = "VbReport_" + ((DateTime)rprt.JobDM.StartDate).ToShortDateString().Replace("/", "_") + ".xls";
                // string FileName = "VbReport_" + id + ".xls";

                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.ContentType = "application/excel";
                Response.ContentEncoding = System.Text.Encoding.Unicode;
                Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());


                var sw = new StringWriter();
                var htw = new HtmlTextWriter(sw);
                var grid = new System.Web.UI.WebControls.GridView();

                ICollection<VbReportExcelDM> dt = rprt.VbReportMachineDMs
                    .Select(r => new VbReportExcelDM
                    {
                        MachineName = r.MachineName,
                        Description = r.Details,
                        Location = r.Areas,
                        MaxValue = r.MaxValue,
                        Status = r.Status,
                        Notes = (string)Session["lang"] == "he-IL" ? r.NotesIL ?? r.NotesEN : r.NotesEN ?? r.NotesIL,

                    }).ToList()
                                 ;

                grid.DataSource = dt;
                grid.DataBind();

                grid.RenderControl(htw);

                Response.Write(sw.ToString());
                Response.End();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion



        #region COMMON

        private void SetLang(string lang, string dir)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                Session["lang"] = lang;
            }
            if (!string.IsNullOrEmpty(dir))
            {
                Request.Cookies["dir"].Value = dir;
                Session["dir"] = dir;
            }
            ViewBag.dir = Session["dir"].ToString() == "False" ? "ltr" : "rtl";
        }

        private void RprtWithFilter(VbReportDM rprt, int[] ids)
        {
            rprt.VbReportMachineDMs = rprt.VbReportMachineDMs.Where(x => ids.Contains(Convert.ToInt32(x.StatusID))).ToList();
        }
        #endregion



    }
}
