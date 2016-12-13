
using System;
using System.IO;
using System.Text;
using System.Web.Hosting;

[assembly: WebActivatorEx.PreApplicationStartMethod(
    typeof(MVC.LangDirection), "PreStart")]

namespace MVC
{
    public class LangDirection
    {
        public static void PreStart()
        {
            {

                var path = HostingEnvironment.ApplicationPhysicalPath + @"Content\" ;

                /** is production*/
                if (!path.Contains("Dropbox"))
                    return;

                //write      
                var fsW = new FileStream(path + "Site_ltr.css", FileMode.Create);
                using (var w = new StreamWriter(fsW, Encoding.UTF8))
                {
                    w.Write("");

                    //read
                    var fs = new FileStream(path + "Site_rtl.css", FileMode.Open);
                    using (var r = new StreamReader(fs, Encoding.UTF8))
                    {
                        String line;
                        while ((line = r.ReadLine()) != null)
                        {
                            var newLine = line.Replace("right", "TempRRR");
                            newLine = newLine.Replace("left", "right");
                            newLine = newLine.Replace("TempRRR", "left");
                            newLine = newLine.Replace("rtl", "ltr");
                            w.WriteLine(newLine);
                        }
                        r.Close();
                        fs.Close();

                        w.Flush();
                        w.Close();
                        fs.Close();
                    }
                }


            }
          
            
        }
    }
}