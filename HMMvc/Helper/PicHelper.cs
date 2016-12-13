using System;
using System.IO;
using System.Web;


namespace C
{
    public class PicHelper
    {
        internal static string GetMacPic(int MachineID, string MachineTypeID)
        {

                var url = "/Pics/MachinePicture/";
                var path = url + "MacPic_" + MachineID + ".jpg";
                path = File.Exists(HttpContext.Current.Server.MapPath(path)) ? 
                    path :
                    url + GetMacImgByType(MachineTypeID);
                return path + "#" + DateTime.Now; 
            
            
        }

        private static string GetMacImgByType(string MachineTypeID)
        {
           
            MachineTypeID= MachineTypeID == null ? "1" : MachineTypeID;
            var path = "MacType_" + MachineTypeID + ".jpg";
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

            return path ;
        }

        internal static string GetPointPic(int MachinePointID)
        {

                var url = "/Pics/MachinePointPicture/";
                var path = url + MachinePointID;
                path = File.Exists(HttpContext.Current.Server.MapPath(path)) ?
                    path :
                    url + "PointDefault.jpg";
                return path;
           
        }

        internal static string GetClientLogo(int ClientID)
        {

                var url = "/Pics/ClientLogos/";
                var path = url + "Logo_" + ClientID + ".jpg";
                path = File.Exists(HttpContext.Current.Server.MapPath(path)) ?
                    path :
                    url + "DefLogo.jpg";
                return path;
            
        }

        internal static string GetPointResualt(int PointResualtID)
        {
           
                var url = "/Pics/PointResualt/";
                var path = url + "PointResSpect_" + PointResualtID + ".jpg";
                path = File.Exists(HttpContext.Current.Server.MapPath(path)) ?
                    path :
                    "/";
                return path;
           
        }
    }
}