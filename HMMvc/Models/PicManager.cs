
﻿using MVC.Helper;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;

namespace MVC.Models
{
    public class PicManager
    {
        private static ImageCodecInfo _jpgEncoder;
        private static EncoderParameters _myEncoderParameters;
        private static readonly long _picCompressionRate = 95L;

        internal static void UploadTempPic(Avatar avat, HttpRequestBase request)
        {
            /** create if doesnt exist*/
            if (avat.SubMode)
            {
                FolderManager.Create(avat.ImgFolder);
                SetPicNumber(avat);
            }

            // avat.ImgFolder = HttpContext.Server.MapPath(".").Replace("\\Pic", "") + _foldpath;

            var upld = new UploadFileHelper();
            upld.UploadFile(avat.Temppath, request);

            // IsMinResolution(avat.Temppath);

            float h; float w;
            switch (avat.ObjType)
            {
                case "ClientLogos":
                    h = 200;
                    w = 300;
                    break;
                case "PointResualt":
                    h = 500;
                    w = 500;
                    break;
                case "Job":
                    h = 500;
                    w = 500;
                    break;
                default:
                    h = 381;
                    w = 556;
                    break;
            }

            //new Task(() =>
            //{
            ResizePic(avat.Temppath, h, w);
            //COMPRESS
            // }).Start();



        }

        /// <summary>
        /// evey step can have several pics
        /// </summary>
        /// <param name="avat"></param>
        private static void SetPicNumber(Avatar avat)
        {
            for (int i = 1; i < 20; i++)
            {
                avat.Number = i;
                if (!FileManager.Exists(avat.path))
                    break;
            }
        }

        private static void IsMinResolution(string temppath)
        {

            var sourceImage = Image.FromFile(temppath);
            if (sourceImage.Width < 100 || sourceImage.Height < 100)
            {
                // do sometings 
            }
            sourceImage.Dispose();
        }



        private static void ResizePic(string temppath, float h, float w)
        {
            var sourceImage = Image.FromFile(temppath);
            FixAutoRotate(sourceImage);


            var propW = w / sourceImage.Width;
            var propH = h / sourceImage.Height;
            var total = Math.Max(propH, propW);
            int width = Convert.ToInt32(sourceImage.Width * total);
            int height = Convert.ToInt32(sourceImage.Height * total);

            Image destImage = ResizeImage(height, width, sourceImage);

            var type = sourceImage.RawFormat;
            sourceImage.Dispose();


            CompressAndSave(temppath, destImage, type);
            //destImage.Save(temppath);
            //destImage.Dispose();


        }

        internal static void Delete(Avatar avat)
        {

            FileManager.Delete(avat.path);
            FileManager.Delete(avat.Temppath);


        }

        private static void CompressAndSave(string imgUrl, Image destImage, ImageFormat type)
        {


            long rate = _picCompressionRate;

            //if (ImageFormat.Jpeg.Equals(type))
            //{
            SetCompresionValues(rate);
            destImage.Save(imgUrl, _jpgEncoder, _myEncoderParameters);
            destImage.Dispose();
            //}
            //else if (ImageFormat.Png.Equals(type))
            //{
            //    /** need to save first because size pic doesnot exist yet*/
            //    destImage.Save(imgUrl);
            //    destImage.Dispose();

            //    var quantizer = new WuQuantizer();
            //    using (var bitmap = new Bitmap(imgUrl))
            //    {
            //        using (var quantized = quantizer.QuantizeImage(bitmap))
            //        {
            //            bitmap.Dispose();
            //            quantized.Save(imgUrl, ImageFormat.Png);
            //            quantized.Dispose();

            //        }
            //    }

            //}
            //else
            //{
            //    destImage.Save(imgUrl);
            //    destImage.Dispose();
            //}


        }

        private static void SetCompresionValues(long rate)
        {
            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            _jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            Encoder myEncoder = Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one
            // EncoderParameter object in the array.
            _myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, rate);
            _myEncoderParameters.Param[0] = myEncoderParameter;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }



        internal void ChangeTempPic(Avatar avat)
        {


            CropPic(avat);


            int[] sizes;
            bool? byWidth = true;
            switch (avat.ObjType)
            {
                case "ClientLogos":
                    sizes = new int[1] { 46 };
                    byWidth = false;
                    break;
                case "MachinePicture":
                    sizes = new int[1] { 380 };
                    break;
                case "MachinePointPicture":
                    sizes = new int[1] { 164 };
                    break;
                case "Job":
                    byWidth = null;
                    sizes = new int[1] { 500 };
                    break;
                case "PointResualt":
                    sizes = new int[1] { 500 };
                    break;
                default:
                    sizes = new int[1] { 380 };
                    break;
            }
            CreatePicSizes(avat, sizes, byWidth);

            FileManager.Delete(avat.Temppath);

        }

        private void CropPic(Avatar avat)
        {
            if (avat.W == 0)
                return;


            /** always origin stay in path and temp is the one whi is being crooped*/
            if (FileManager.Exists(avat.Temppath))
            {//"C:\\Dropbox\\HMErp\\WebErp\\HMErpSolution\\HMMvc\\Pics\\MachinePicture\\MacPic_6042.jpg"
                FileManager.Copy(avat.Temppath, avat.path);
            }
            else
            {
                FileManager.Copy(avat.path, avat.Temppath);
            }



            using (var sourceImage = Image.FromFile(avat.Temppath))
            {
                //string ImgName = "prop" +id + ".jpg";//System.IO.Path.GetFileName(dest);
                using (var destImage = new Bitmap(avat.W, avat.H))
                {
                    using (var g = Graphics.FromImage(destImage))
                    {

                        var rectDestination = new Rectangle(0, 0, destImage.Width, destImage.Height);
                        var rectCropArea = new Rectangle(avat.X, avat.Y, avat.W, avat.H);
                        g.DrawImage(sourceImage, rectDestination, rectCropArea, GraphicsUnit.Pixel);

                        //
                        var type = sourceImage.RawFormat;
                        sourceImage.Dispose();

                        CompressAndSave(avat.Temppath, destImage, type);
                        //destImage.Save(avat.Temppath);
                        //destImage.Dispose();
                    }
                }
            }

        }

        static Image ResizeImage(int height, int width, Image sourceImage)
        {
            Image destImage = new Bitmap(sourceImage, width, height);

            using (var g = Graphics.FromImage(destImage))
            {
                //oGraphic.CompositingQuality = CompositingQuality.HighQuality;
                //oGraphic.SmoothingMode = SmoothingMode.HighQuality;
                //oGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;

                var oRectangle = new Rectangle(0, 0, width, height);
                g.DrawImage(sourceImage, oRectangle);
                return destImage;

            }


        }

        private static void FixAutoRotate(Image sourceImage)
        {
            if (!sourceImage.PropertyIdList.Contains(0x0112))
                return;


            int rotationValue = sourceImage.GetPropertyItem(0x0112).Value[0];
            switch (rotationValue)
            {
                case 1: // landscape, do nothing
                    break;

                case 8: // rotated 90 right
                        // de-rotate:
                    sourceImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate270FlipNone);
                    break;

                case 3: // bottoms up
                    sourceImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate180FlipNone);
                    break;

                case 6: // rotated 90 left
                    sourceImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate90FlipNone);
                    break;
            }

        }

        public void Rotate(Avatar avat, bool isCounter)
        {


            RotateAndSavePic(isCounter, avat.path);

        }

        private static void RotateAndSavePic(bool isCounter, string imgUrl)
        {

            if (!FileManager.Exists(imgUrl))
                throw new Exception("img url doesnt exist. " + imgUrl);

            var sourceImage = Image.FromFile(imgUrl);

            sourceImage.RotateFlip(GetDegree(isCounter));

            sourceImage.Save(imgUrl);

            sourceImage.Dispose();
        }

        private static RotateFlipType GetDegree(bool isCounter)
        {
            return isCounter ? RotateFlipType.Rotate270FlipNone : RotateFlipType.Rotate90FlipNone;

        }

        public void CreatePicSizes(Avatar avat, int[] sizes, bool? byWidth)
        {
            int height, width;

            for (var i = 0; i < sizes.Length; i++)
            {

                var sourceImage = Image.FromFile(avat.Temppath);

                if (!byWidth.HasValue)
                {
                    var maxVal = Math.Max(sourceImage.Height, sourceImage.Width);
                    if (maxVal <= sizes[i])
                    {
                        width = sourceImage.Width;
                        height = sourceImage.Height;
                    }
                    else
                    {

                        var ratio = maxVal / sizes[i];
                        width = Convert.ToInt32(sourceImage.Width / ratio);
                        height = Convert.ToInt32(sourceImage.Height / ratio);
                    }

                }
                else if (byWidth == true)
                {
                    width = sizes[i];
                    height = Convert.ToInt32(width * 3 / 4);
                }
                else
                {
                    height = sizes[i];
                    width = sourceImage.Width * height / sourceImage.Height;
                }

                Image destImage = ResizeImage(height, width, sourceImage);

                var type = sourceImage.RawFormat;
                sourceImage.Dispose();

                CompressAndSave(avat.path, destImage, type);


            }
        }



        internal void ChangeFolder(string oldFolder, string newFolder)
        {
            FolderManager.Move(oldFolder, newFolder);
        }



    }
}