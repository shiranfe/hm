using System.Web;
using System.Web.Hosting;

namespace MVC.Models
{
    public class Avatar
    {

        public Avatar()
        {
            SubMode = false;
        }

        public Avatar(int macId, string type, string pre, bool subMode=false)
        {
            ObjID = macId;
            ObjType = type;
            FilePre = pre;
            SubMode = subMode;
        }

        public int ObjID { get; set; }
        public string ObjType { get; set; }
        public string FilePre { get; set; }
        /** pic/machine/macPic_1.jpg  ||  pic/machine/1/macPic.jpg*/
        public bool SubMode { get; set; }
        public int Number { get; set; }

        public string ImgFolder => HostingEnvironment.ApplicationPhysicalPath +
            "Pics\\" + ObjType + "\\"  + (SubMode ? ObjID + "\\" : "");

        public string ImfFile => FilePre + (SubMode ?  Number.ToString(): ObjID.ToString()  )  + ".jpg";
        public string path => ImgFolder + ImfFile;
        public string Temppath => path.Replace(FilePre, "Temp_" + FilePre);

        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
    }
}