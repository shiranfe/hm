using BL;
using Common;
using Microsoft.Practices.Unity;
using MVC.Areas.Admin.Controllers;
using MVC.Models;
using MvcBlox.Models;
using System;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class PicController : _BasicController
    {

        private readonly MachineBL _machineBL;
        private readonly IGlobalManager _globalManager;
        private readonly PicManager _picManger;

        public PicController([Dependency]MachineBL machineBL,
            [Dependency]PicManager picManger,
            [Dependency]IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _machineBL = machineBL;
            _picManger = picManger;
        }


        private readonly string _foldpath = @"\Pics\";
        // private string foldpathclnt = "/images/crops/";

        //GET:


        public void GetMacPic(SelectedMachine mac)
        {
            _globalManager.GetMacPic(mac);
        }

        //public ActionResult GetAvatar(int ObjID,string ObjType)
        //{
        //    string x, y, w, h;

        //    ///< GET CORDS FROM DB

        //    string PicCords;
        //    switch (ObjType)
        //    {
        //        case "Machine":
        //            PicCords = _machineBL.GetPicCords(ObjID);
        //            break;
        //        default:
        //            break;
        //    }


        //    string[] a = Regex.Split(PicCords, ",");
        //    x = a[0];
        //    y = a[1];
        //    w = a[2];
        //    h = a[3];

        //    return Json(new { sts = 1, x = x, y = y, h = h, w = w });
        //}


        [HttpPost]
        public ActionResult Add(string qqfile, Avatar avat)
        {
            try
            {
                PicManager.UploadTempPic(avat, Request);

                return Json(new { success = true,  path = PicHelper.PhysicalToUrl(avat.Temppath), number = avat.Number });

            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }

        }

  

        [HttpPost]
        public ActionResult Update(Avatar avat)
        {
            try
            {
                PicManager.ChangeTempPic(avat);
                // avat.ImgFolder = HttpContext.Server.MapPath(".").Replace("\\Pic", "") + _foldpath;


                return Json(new { sts = "1", path = PicHelper.PhysicalToUrl(avat.path) });

            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }
        }




        [HttpPost]
        public ActionResult Rotate(Avatar avat, bool isCounter)
        {
            try
            {
                PicManager.Rotate(avat, isCounter);
                return Json(new { success = true, path = PicHelper.PhysicalToUrl(avat.path) + "?" + DateTime.Now.Ticks });

            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }
        }

        [HttpPost]
        public ActionResult Delete(Avatar avat)
        {
            try
            {
                PicManager.Delete(avat);
                return Json(new { success = true, path = PicHelper.PhysicalToUrl(avat.path) });

            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }
        }



        [HttpPost]
        public ActionResult DelAvatar(int objID, string objType, string filePre)
        {
            try
            {
                var path = GetPath(objType + "\\", objID, filePre);
                System.IO.File.Delete(path);
                ///< DELETE ALL SIZE
                /* System.IO.File.Delete(path.Replace("Card_", "Card_s1_"));
                 System.IO.File.Delete(path.Replace("Card_", "Card_s2_"));
                 System.IO.File.Delete(path.Replace("Card_", "Card_s3_"));*/
                //var id = GetCardID();
                //AccountBL.DeleteCords(id);
                var macPic = "";
                if (objType == "Machine")
                {
                    var mac = _machineBL.GetSelectedMachine(objID);
                    GetMacPic(mac);
                    macPic = mac.MacPic;
                }
                return Json(new { sts = "1", path = macPic });
            }
            catch (Exception fileDeleteError)
            {
                return Json(new { sts = "2", msg = fileDeleteError });
            }

        }

        [HttpPost]
        public ActionResult DelTempAvatar(int objID, string objType, string filePre)
        {
            try
            {
                var path = GetPath(objType + "/Temp_", objID, filePre);
                System.IO.File.Delete(path);

                return Json(new { sts = "1" });
            }
            catch (Exception fileDeleteError)
            {
                return Json(new { sts = "2", msg = fileDeleteError });
            }

        }





        private string GetPath(string p, int objID, string filePre)
        {

            var mapath = HttpContext.Server.MapPath(".").Replace("\\Pic", "");
            var serverFileName = p + filePre + objID + ".jpg";
            var path = mapath + _foldpath + serverFileName;

            return path;
        }

    }
}
