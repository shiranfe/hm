using BL;
using Common;
using HtmlAgilityPack;
using Microsoft.Practices.Unity;
using MVC.Helper;
using MvcBlox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{

    public class ImportController : _BasicController
    {
        private readonly ImportVbHtmlBL _importVBHtmlBL;
        private readonly VbBL _vbBL;
        private readonly IGlobalManager _globalManager;


        public ImportController([Dependency]ImportVbHtmlBL importVBHtmlBL, [Dependency]VbBL vbBL, [Dependency]IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _importVBHtmlBL = importVBHtmlBL;
            _vbBL = vbBL;
        }

        public ActionResult ImportVbHtmlReport()
        {
            // _vbBL.InsertVbHtmlPointResualt(111, 193);
            return PartialView();
        }

        [HttpPost]
        public ActionResult UploadVbHtml(string qqfile)
        {
            try
            {
                var path = GetFolder();
                    var upld = new UploadFileHelper();
                upld.UploadFile(path, Request);


                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }

        }

        private string GetFolder()
        {
            var folder = HttpContext.Server.MapPath(".").Replace("\\Admin\\Import", "");
            var path = folder + "\\Content\\Html\\VbReport.html";
            return path;
        }


        [HttpPost]
        public ActionResult ImportVbLoadHtmlAndChooseMacines()
        {
            try
            {
                var path = GetFolder();

                var vbHtmlReportDMList = ConvertHtmlTableToList(path);

                _importVBHtmlBL.AddJobVibrationHtml(vbHtmlReportDMList);
                  
                var clnts = _vbBL.GetImportVbHtmlReportClients();
                ViewBag.Clients = new SelectList(clnts, "ClientID", "ClientName",clnts.First().ClientID);

                var a = ((SelectList)ViewBag.Clients).Where(x => x.Selected).ToList();

                var macs = _importVBHtmlBL.GetVbNewMachine();
                return PartialView(macs);
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }

        }

        [HttpPost]
        public ActionResult ImportVbChooseJob()
        {
            try
            {
                var dates = _importVBHtmlBL.GetVbHtmlReportChooseJobDM();

                var jobs = _vbBL.GetOpenJobs();
                ViewBag.jobs = new SelectList(jobs, "JobID", "JobName");

                return PartialView(dates);
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }

        }

        [HttpPost]
        public ActionResult ImportVbCommitImport(int ClientID, int? JobID)
        {
            try
            {
                JobID = _importVBHtmlBL.CommitImportHtmlReport(ClientID, JobID);

                return Json("<h3>הקובץ על בהצלחה!</h3> <a class=\"linktext\" href=\"/Admin/VB/VBAnalysis?JobID=" + JobID + "\">המשך לניתוח הדוח</a>");
               // return Json(new { JobID = JobID });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }


        }

        /// <summary>
        /// toogle machinse, or dates to be included in he import process
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Include"></param>
        /// <param name="Entity"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ImportVbToggleIncludes(int Id, bool Include, string Entity)
        {
            try
            {
                _importVBHtmlBL.ToggleVbHtmlIncludes(Id, Include, Entity);
                return Json(new { });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }

        }


        public static List<VbHtmlReportDM> ConvertHtmlTableToList(string path)
        {

            var list = new List<VbHtmlReportDM>();
            // Load the html document
            var web = new HtmlWeb();
            var doc = web.Load(path);

            // Get all tables in the document
            var tables = doc.DocumentNode.SelectNodes("//table");

            var macname = "";
            // Iterate all rows in the first table
            var rows = tables[0].SelectNodes(".//tr");
            for (var i = 1; i < rows.Count; ++i)
            {


                var cols = rows[i].SelectNodes(".//td");

                if (cols.Count == 1) // mac name row
                {
                    macname = cols[0].InnerText.Replace("\r\n", "");
                }
                else
                {
                    var location = cols[0].InnerText.Replace(" ", string.Empty);
                    if (cols[0].InnerText.Split('-').Length > 2)
                    {
                        var schdule = location.Split('-').Last();
                        var pointNumber = location.Replace("-" + schdule, "");
                        pointNumber = pointNumber.Replace("-", "_");
                        location = pointNumber + "-" + schdule;
                    }


                    list.Add(new VbHtmlReportDM
                    {
                        MachineName = macname,
                        Location = location,
                        Date = cols[7].InnerText,
                        Latest = cols[4].InnerText,
                        ScheduleEntry = cols[1].InnerText,
                        Type = cols[2].InnerText
                    });
                }


            }

            return list;

        }









    }


}



/*

   private string GetHtmlPage(string path)
        {

            String strResult;
            WebResponse objResponse;
            WebRequest objRequest = HttpWebRequest.Create(path);
            objResponse = objRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                strResult = sr.ReadToEnd();

                sr.Close();
            }
            // strResult = strResult.Remove(0, strResult.LastIndexOf("<table>"));
            string[] values = strResult.Split(new string[] { "<table ", "</table>" }, StringSplitOptions.RemoveEmptyEntries);

            // Response.Write("<table>" + values[1] + "</table>");
           // ConvertHTMLTablesToDataSet("<table " + values[1] + "</table>");
            //  List<string> list = new List<string>(values);

            return strResult;
        }

        //private DataSet ConvertHTMLTablesToDataSet(string HTML)
        //{
        //    // Declarations 
        //    DataSet ds = new DataSet();
        //    DataTable dt = null;
        //    DataRow dr = null;
        //    DataColumn dc = null;
        //    string TableExpression = "<table[^>]*>(.*?)</string></string></table>";
        //    string HeaderExpression = "<th[^>]*>(.*?)";
        //    string RowExpression = "<tr[^>]*>(.*?)";
        //    string ColumnExpression = "<td[^>]*>(.*?)";
        //    bool HeadersExist = false;
        //    int iCurrentColumn = 0;
        //    int iCurrentRow = 0;

        //    // Get a match for all the tables in the HTML 
        //    MatchCollection Tables = Regex.Matches(HTML, TableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        //    // Loop through each table element 
        //    foreach (Match Table in Tables)
        //    {
        //        // Reset the current row counter and the header flag 
        //        iCurrentRow = 0;
        //        HeadersExist = false;

        //        // Add a new table to the DataSet 
        //        dt = new DataTable();

        //        //Create the relevant amount of columns for this table (use the headers if they exist, otherwise use default names) 
        //        if (Table.Value.Contains("<th"))
        //        {
        //            // Set the HeadersExist flag 
        //            HeadersExist = true;

        //            // Get a match for all the rows in the table 
        //            MatchCollection Headers = Regex.Matches(Table.Value, HeaderExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        //            // Loop through each header element 
        //            foreach (Match Header in Headers)
        //            {
        //                dt.Columns.Add(Header.Groups[1].ToString());
        //            }
        //        }
        //        else
        //        {
        //            for (int iColumns = 1; iColumns <= Regex.Matches(Regex.Matches(Regex.Matches(Table.Value, TableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase)[0].ToString(), RowExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase)[0].ToString(), ColumnExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase).Count; iColumns++)
        //            {
        //                dt.Columns.Add("Column " + iColumns);
        //            }
        //        }


        //        //Get a match for all the rows in the table 

        //        MatchCollection Rows = Regex.Matches(Table.Value, RowExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        //        // Loop through each row element 
        //        foreach (Match Row in Rows)
        //        {
        //            // Only loop through the row if it isn't a header row 
        //            if (!(iCurrentRow == 0 && HeadersExist))
        //            {
        //                // Create a new row and reset the current column counter 
        //                dr = dt.NewRow();
        //                iCurrentColumn = 0;

        //                // Get a match for all the columns in the row 
        //                MatchCollection Columns = Regex.Matches(Row.Value, ColumnExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        //                // Loop through each column element 
        //                foreach (Match Column in Columns)
        //                {
        //                    // Add the value to the DataRow 
        //                    dr[iCurrentColumn] = Column.Groups[1].ToString();

        //                    // Increase the current column  
        //                    iCurrentColumn++;
        //                }

        //                // Add the DataRow to the DataTable 
        //                dt.Rows.Add(dr);

        //            }

        //            // Increase the current row counter 
        //            iCurrentRow++;
        //        }


        //        // Add the DataTable to the DataSet 
        //        ds.Tables.Add(dt);

        //    }

        //    return ds;

        //}

        private string CleanHtml(string path)
        {
            //String strResult;
            //WebResponse objResponse;
            //WebRequest objRequest = HttpWebRequest.Create(path);
            //objResponse = objRequest.GetResponse();
            //using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            //{
            //    strResult = sr.ReadToEnd();

            //    sr.Close();
            //}
            //// strResult = strResult.Remove(0, strResult.LastIndexOf("<table>"));
            //string[] values = strResult.Split(new string[] { "<table ", "</table>" }, StringSplitOptions.RemoveEmptyEntries);
           // string html ="" ;//= "<table " + values[1] + "</table>";

            //write      
            string newpath = "C:\\Dropbox\\HMErp\\WebErp\\HMErpSolution\\HMMvc\\Content\\HtmlPage1.html";
            FileStream fsW = new FileStream(newpath, FileMode.Create);
            StreamWriter w = new StreamWriter(fsW, Encoding.UTF8);
            w.Write("");


            //read
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader r = new StreamReader(fs, Encoding.UTF8);
            String line;



            bool firstendtr = true;
        

            while ((line = r.ReadLine()) != null)
            {
                line = line.Replace("<font face=arial>", "");
                if (line == "</tr>")
                {
                    if (firstendtr)
                    {
                        w.WriteLine(line);
                        firstendtr = false;
                    }
                }
                else
                {
                    firstendtr = true;
                    w.WriteLine(line);
                }
                
            }
            r.Close();
            fs.Close();

            w.Flush();
            w.Close();
            fs.Close();

            return newpath;
        }

        private void laundery(string path)
        {
            String strResult;
            WebResponse objResponse;
            WebRequest objRequest = HttpWebRequest.Create(path);
            objResponse = objRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                strResult = sr.ReadToEnd();

                sr.Close();
            }

            string whitelist = "<table><tr></tr></table>";
           // string html = "alma <b>barack <span>citrom dinnye</span> eper";
            //string accepted = "alma <b>barack <span>citrom dinnye</span> eper</b>";

            HtmlLaundry machine = new HtmlLaundry(whitelist);

            string outcome = machine.CleanHtml(strResult);

           // bool areequal = accepted == outcome;
        }

        public static void ConvertHtmlTableToList(string path)
        {
            String strResult;
            WebResponse objResponse;
            WebRequest objRequest = HttpWebRequest.Create(path);
            objResponse = objRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                strResult = sr.ReadToEnd();

                sr.Close();
            }
            string HTML_TAG_PATTERN = "<.*?>";
            string[] values = strResult.Split(new string[] { "<td>", "</td>" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var td in values)
            {
                string str = Regex.Replace(td, HTML_TAG_PATTERN, string.Empty);
            }
        }


        public static void ConvertHtmlTableToList2(string path)
        {

            List<VbHtmlReportDM> rprt = new List<VbHtmlReportDM>();
           

            HtmlWeb web = new HtmlWeb();

            //read
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader r = new StreamReader(fs, Encoding.UTF8);
            String line;



            bool TakeMacName = false;
            bool TaKeRow = false;
            string MacName = "";
           // string HTML_TAG_PATTERN = "<.*?>";

            while ((line = r.ReadLine()) != null)
            {


                if (TakeMacName)
                {
                    HtmlDocument doc = web.Load(path);
                    MacName = doc.DocumentNode.InnerText;
                    //TakeMacName = Regex.Replace(line, HTML_TAG_PATTERN, string.Empty);
                   // string[] strs = line.Split('<');
                   // MacName = strs[0];
                    TakeMacName = false;
                }
                else if (TaKeRow)
                {

                    VbHtmlReportDM row = new VbHtmlReportDM
                   {
                       MachineName = MacName,

                   };


                    rprt.Add(row);
                    
                }
                else if (line.Contains("<tr><td colspan"))//machine name row
                {
                    TakeMacName = true;
                }
                else if (line == "<tr>")//data row
                {
                    TaKeRow = true;
                }
                else if (line.Contains("</tr>"))//data row
                {
                    TaKeRow = false;
                }
            }
            r.Close();
            fs.Close();

            fs.Close();

        }

 */
