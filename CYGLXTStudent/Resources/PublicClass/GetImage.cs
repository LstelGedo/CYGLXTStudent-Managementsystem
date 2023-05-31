using System.IO;
using System.Windows.Media.Imaging;


namespace CYGLXTStudent.Resources.PublicClass
{
    public static class GetImage
    {
        /// <summary>
        /// 1、图片
        /// </summary>
        /// <param name="imagePath">文件路径</param>
        /// <returns></returns>
        public static BitmapImage GetBitmapImage(string imagePath)
        {
            BitmapImage bitmap = new BitmapImage();

            if (File.Exists(imagePath))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                using (Stream ms = new MemoryStream(File.ReadAllBytes(imagePath)))
                {
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }
            }

            return bitmap;
        }

        /// <summary>
        /// 2、byte[] 转换为BitmapImage：
        /// </summary>
        /// <param name="bytearray">二进制图片</param>
        /// <returns></returns>
        public static BitmapImage Bytearraytobitmapimage(byte[] bytearray)
        {
            BitmapImage bmp;
            try
            {
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(bytearray);
                bmp.EndInit();
            }
            catch
            {
                bmp = null;
            }
            return bmp;
        }
    }
}
