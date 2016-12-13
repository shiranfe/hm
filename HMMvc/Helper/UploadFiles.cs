using System.IO;
using System.Linq;
using System.Web;

namespace MVC.Helper
{
    public class UploadFileHelper
    {
        internal void UploadFile(string newFilename, HttpRequestBase request)
        {
            using (var strm = request.InputStream)
            {
                var br = new BinaryReader(strm);
                byte[] fileContents = {};
                const int chunkSize = 1024*1024;

                // We need to hand IE a little bit differently...
                if (request.Browser.Browser == "IE")
                {
                    // GET FILES FROM POST
                    var myfiles = HttpContext.Current.Request.Files;
                    var postedFile = myfiles[0];
                    if (!postedFile.FileName.Equals(""))
                    {
                        var fn = Path.GetFileName(postedFile.FileName);
                        br = new BinaryReader(postedFile.InputStream);
                        //qqfile = fn;
                    }
                }

                // Nor have the binary reader on the IE file input Stream. Back to normal...
                while (br.BaseStream.Position < br.BaseStream.Length - 1)
                {
                    var b = new byte[chunkSize];
                    var readLen = br.Read(b, 0, chunkSize);
                    var dummy = fileContents.Concat(b).ToArray();
                    fileContents = dummy;
                    dummy = null;
                }


                var writeStream = new FileStream(newFilename, FileMode.Create);
                var bw = new BinaryWriter(writeStream);
                bw.Write(fileContents);
                bw.Close();

                strm.Flush();
                strm.Dispose();
            }
        }

       
    }
}