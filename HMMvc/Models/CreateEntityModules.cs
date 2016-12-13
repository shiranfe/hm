using System;
using System.IO;
using System.Text;

namespace MVC.Models
{
    public class CreateEntityModules
    {
        public string entity { get; set; }
        public string folder { get; set; }
        public bool IsAdmin { get; set; }


        private readonly string rootPath = "C:\\Dropbox\\HMErpSolution\\";
        private readonly string srcEntity = @"_Template";
        private readonly string adminArea = "Areas\\Admin\\";


        public void Create()
        {
            
            CreateDM();

            CreateBL();

            CreateModule();

            UpdateUnity();

            CreateController();
           

            // mvc folder is entity
            folder = entity;

            CreateViews();

            CreateScript();


        }

       

        private void CreateDM()
        {
            var path = rootPath + "Common\\";
            var end = "DM.cs";

            CopyFile(end, path);
        }


        //private void CreateDM()
        //{
        //    var path = rootPath + "Common\\";

        //    var end = entity+ "DM.cs";

        //    path = SetFolder(path);
                       
        //    var newfile = path + end;

        //    var fsW = new FileStream(newfile, FileMode.Create);
        //    using (var w = new StreamWriter(fsW, Encoding.UTF8))
        //    {
        //        w.WriteLine("namespace Common");
        //        w.WriteLine("{");
        //        w.WriteLine("    public class " + entity + "DM");
        //        w.WriteLine("    {");
        //        w.WriteLine("    }");
        //        w.WriteLine("}");

        //        w.Flush();
        //        w.Close();
        //        fsW.Close();
        //    }

        //}

        private string SetFolder(string path)
        {
            if (folder != null)
            {
                path += folder + "\\";
                var directoryName = Path.GetDirectoryName(path);
                if ((directoryName.Length > 0) && (!Directory.Exists(directoryName)))
                {
                    Directory.CreateDirectory(directoryName);
                }
            }
            return path;
        }


        private void CreateController()
        {

            var end = "Controller.cs";
            var path = GetMvcPath() + "Controllers\\";

            CopyFile(end, path);
        }

        /**HMMvc\\Area\\Admin... */
        private string GetMvcPath()
        {
            return rootPath + "HMMvc\\" + (IsAdmin ? adminArea : "");
        }

        private void CreateViews()
        {

            var path = GetMvcPath() + "Views\\";

            var end = "\\Index.cshtml";
            CopyFile(end, path,true);

            end = "\\Update.cshtml";
            CopyFile(end, path, true);

        }

        private void CreateScript()
        {        
            var path = GetMvcPath() + "Scripts\\";

            var end = "\\index.js";

            CopyFile(end, path, true);
        }

        private void CreateBL()
        {
            var path = rootPath + "BL\\" ;
            var end = "BL.cs";

            CopyFile(end, path);
        }

        private void CreateModule()
        {
            var path = rootPath + "BL\\" ;
            var end = "Module.cs";

            CopyFile(end, path);
        }

        private void UpdateUnity()
        {

            var filename = rootPath + "BL\\ServicesUnityExtension.cs";

            using (var fsw = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            {
                using (var w = new StreamWriter(fsw, Encoding.UTF8))
                {
                    using (var fsr = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var r = new StreamReader(fsr))
                        {

                            String line;
                            while ((line = r.ReadLine()) != null)
                            {
                                w.WriteLine(line);

                                if (line.Contains("#region Modules"))
                                {
                                    w.WriteLine("Container.RegisterType<" + entity + "Module," + entity + "Module>(new ContainerControlledLifetimeManager());");
                                }
                                else if (line.Contains("#region Services"))
                                {
                                    w.WriteLine("Container.RegisterType<" + entity + "BL," + entity + "BL>(new ContainerControlledLifetimeManager());");
                                }
                            }

                            r.Close();
                            fsr.Close();

                            w.Flush();
                            w.Close();
                            fsw.Close();

                        }
                    }
                }
            }

        }



        private void CopyFile(string end, string path, bool isMvc=false)
        {
            var srcFile = path + srcEntity + end;

            path = SetFolder(path);

            var ent = isMvc ? "" : entity;

            var newfile = path + ent + end;
           

            //write      
            var fsW = new FileStream(newfile, FileMode.Create);
            using (var w = new StreamWriter(fsW, Encoding.UTF8))
            {
                w.Write("");

                //read
                var fs = new FileStream(srcFile, FileMode.Open);
                using (var r = new StreamReader(fs, Encoding.UTF8))
                {
                    String line;
                    while ((line = r.ReadLine()) != null)
                    {
                        var newLine = line.Replace(srcEntity, entity);

                        w.WriteLine(newLine);
                    }
                    r.Close();
                    fs.Close();

                    w.Flush();
                    w.Close();
                    fsW.Close();
                }
            }

        }


    }
}