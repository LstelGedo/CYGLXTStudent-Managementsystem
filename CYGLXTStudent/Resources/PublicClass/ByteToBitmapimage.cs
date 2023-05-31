using System.IO;
using System.Windows.Media.Imaging;

namespace CYGLXTStudent.Resources.PublicClass
{
    /// <summary>
    /// byte[] 转换为BitmapImage
    /// </summary>
    public class ByteToBitmapimage
    {
        /// <summary>
        /// byte[] 转换为bitmapimage：
        /// </summary>
        /// <param name="bytearray"></param>
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
