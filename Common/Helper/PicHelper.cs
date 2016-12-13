using System;
using System.IO;
using System.Web;
using System.Web.Hosting;

namespace Common
{
    public class PicHelper
    {
        const string MacPicPath = "/Pics/MachinePicture/";

        static string root = HostingEnvironment.ApplicationPhysicalPath + @"Pics\";
        public static string JobPathPhys = root +@"Job\";
        public static string MacPicPathPhys = root + @"MachinePicture\";

        public static string GetMacPic(int MachineID, string MachineTypeID)
        {


            string path = MacPicPath + "MacPic_" + MachineID + ".jpg";
            path = FileExist(path) ?
                path :
                MacPicPath + GetMacImgByType("1"); //machine doesnt have type, only part

            return Fresh(path);

        }

        private static string Fresh(string path)
        {
            return path + "?" + DateTime.Now.Ticks;
        }

        public static string GetMacPicOrNull(int MachineID)
        {

       
            string path = MacPicPath + "MacPic_" + MachineID + ".jpg";
            if (FileExist(path))
                return Fresh(path);
         
            return null;

        }

        internal static string GetPartPic(string MachineTypeStr)
        {

            return MacPicPath + MachineTypeStr.Replace("Machine", "") + ".png";
        }

        private static string GetMacImgByType(string MachineTypeID)
        {

            MachineTypeID = MachineTypeID == null ? "1" : MachineTypeID;
            string path = "MacType_" + MachineTypeID + ".jpg";
            //switch (MachineTypeID)
            //{
            //    case null:
            //    case 1:
            //        path += "Event";
            //        break;
            //    case 2:
            //        path += "Conv";//show people pics
            //        break;
            //    case 3:
            //        path += "Block";
            //        break;
            //}

            return path;
        }

        public static string GetPointPic(int MachinePointID)
        {

            string url = "/Pics/MachinePointPicture/";
            string path = url + MachinePointID;
            path = FileExist(path) ?
                path :
                url + "PointDefault.jpg";
            return path;

        }

        public static string GetClientLogo(int ClientID)
        {

            string url = "/Pics/ClientLogos/";
            string path = url + "Logo_" + ClientID + ".jpg";
            path = FileExist(path) ?
                path :
                url + "DefLogo.jpg";
            return path;

        }

        public static string GetPointResualt(int PointResualtID)
        {

            string url = "/Pics/PointResualt/";
            string path = url + "PointResSpect_" + PointResualtID + ".jpg";
            path = FileExist(path) ?
                path :
                "/";
            return path;

        }

        public static string GetEmployeeSignture(int empID)
        {
            string url = "/Pics/Employee/";
            string path = url + "Sig_" + empID + ".png";
            path = FileExist(path) ? path :string.Empty;
            return path;
        }


        public static bool FileExist(string path)
        {
            return File.Exists(HttpContext.Current.Server.MapPath(path));
        }

        public static string PhysicalToUrl(string path, bool fresh=false)
        {
            var physPath = path.Replace(HostingEnvironment.ApplicationPhysicalPath, "/").Replace("\\", "/");

            return fresh ? Fresh(physPath) : physPath;
        }

        //public static string GetJobStepPic(int jobID, RefubrishStep step)
        //{
        //    var url = "/Pics/Job/" + jobID + "/" + step.ToString() + ".jpg";
        //    if (FileExist(url))
        //        return url + "?" + DateTime.Now;

        //    if (step == RefubrishStep.DetailsStep)
        //        return "/images/UploadTicket.png";

        //    return "/images/UploadImage.png";
        //}
    }
}