using System;
using System.IO;

namespace MVC.Models
{
    public class FileManager
    {
        internal static void Delete(string path, bool ShouldExist=false)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                return;
            }

            if (ShouldExist)
            {
                throw new Exception(path + " doesnt exist, cant be deleted ");
            }
               
        }

        internal static void Copy(string src, string dest, bool overide=true)
        {
            if (!Exists(src))
                throw new Exception(src + " was not found. ");

            File.Copy(src, dest, overide);
        }

        internal static bool Exists(string src)
        {
            return File.Exists(src);
        }
    }


    public class FolderManager
    {
        internal static void Create(string path)
        {
            Directory.CreateDirectory(path);
        }

        internal static void Delete(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        internal static void Move(string oldFolder, string newFolder)
        {
            Directory.Move(oldFolder, newFolder);
        }
    }

}