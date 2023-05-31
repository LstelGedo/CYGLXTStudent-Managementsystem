using CYGLXTStudent.Model;
using CYGLXTStudent.Model.Vo;
using CYGLXTStudent.Resources.Control;
using CYGLXTStudent.Resources.PublicClass;
using CYGLXTStudent.ViewModel.Staffs;
using CYGLXTStudent.Views.Staffs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Win32;

namespace CYGLXTStudent.ViewModel.Staffs
{
    /// <summary>
    /// 工作证打印与图片导出
    /// </summary>
    public class PrintStaffViewModel: ViewModelBase
    {
        public PrintStaffViewModel()
        {
            //关闭窗口
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow, wd => wd != null);
            //打印员工工作证
            PrintCommand=new RelayCommand<Visual>(PrintStaff, vs => vs != null);
            //导出工作证
            ExportCommand = new RelayCommand<DockPanel>(ExportStaff, dp => dp != null);
        }
        #region 【1、全局变量】

        #endregion
        #region 【2、属性】
        private S_Staff currentStaffEntity;
        /// <summary>
        /// 员工实体
        /// </summary>
        public S_Staff CurrentStaffEntity
        {
            get { return currentStaffEntity; }
            set
            {
                if (currentStaffEntity != value)
                {
                    currentStaffEntity = value;
                    RaisePropertyChanged(()=> CurrentStaffEntity);
                } 
            }
        }

        private BitmapImage caseCoverImage;
        /// <summary>
        /// 员工图片
        /// </summary>
        public BitmapImage CaseCoverImage
        {
            get { return caseCoverImage; }
            set
            {
                if (caseCoverImage != value)
                {
                    caseCoverImage = value;
                    RaisePropertyChanged(() => CaseCoverImage);
                }
            }
        }


        #endregion
        #region 【3、命令】
        /// <summary>
        /// 3.1 关闭窗口命令
        /// </summary>
        public RelayCommand<Window> CloseWindowCommand { get; set; }
        /// <summary>
        /// 3.2 打印命令
        /// </summary>
        public RelayCommand<Visual> PrintCommand { get; set; }
        /// <summary>
        /// 3.3 导出图片命令
        /// </summary>
        public RelayCommand<DockPanel> ExportCommand { get; set; }

        #endregion
        #region 【4、方法】
        /// <summary>
        /// 4.1 关闭窗口
        /// </summary>
        /// <param name=""></param>
        private void CloseWindow(Window window)
        {
            if (window!=null)
            {
                window.Close();
            }
        }
        /// <summary>
        /// 4.2 打印员工工作证
        /// </summary>
        /// <param name="dock"></param>
        private void PrintStaff(Visual visual)
        {
            //实例化打印对话框
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog()==true)
            {
                //打印visual（非文本）对象（打印区域，要打印的作业的说明）；
                dialog.PrintVisual(visual, "员工工作证打印");
            }
        }
        /// <summary>
        /// 4.3 导出 界面指定元素的图片
        /// </summary>
        /// <param name="dock">区域</param>
        private void ExportStaff(DockPanel dock)
        {
            //1、实例化文件对话框对象
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG文件(*.png)|*.png|JPG文件(*.jpg)|*.jpg|BMP文件(*.bmp)|*.bmp|GIF文件(*.gif)|*.gif|TIF文件(*.tif)|*.tif"
            };
            //2、判断对话框是否打开;
            if (saveFileDialog.ShowDialog()==true)
            {
                //3、选择文件图片保存的目录；
                string strDir=Path.GetDirectoryName(saveFileDialog.FileName);
                //4、判断目录是否存在，
                if (!Directory.Exists(strDir))
                {
                    //不存在，创建目录;
                    Directory.CreateDirectory(strDir);
                }
                //5、保存图片(图像流)
                string strFileName = saveFileDialog.FileName;
                if (File.Exists(strFileName))
                {
                    //删除文件
                    File.Delete(strFileName);
                }
                //保存图片
                SaveToImage(dock, strFileName);
            }

        }
        /// <summary>
        /// 4.4 图像流
        /// </summary>
        /// <param name="frameworkElement"></param>
        /// <param name="fileName"></param>
        private void SaveToImage(FrameworkElement frameworkElement,string fileName)
        {
            //1、保存在指定路径
            FileStream fs=new FileStream(fileName, FileMode.Create);
            /* 2、构造一个RenderTargetBitmap 类的实例,
             * RenderTargetBitmap 类的作用是Visual 对象转换为位图。
             * Visual 类为WPF中的呈现提供支持，其中包括命中测试、坐标转换和边界框计算。
            *参数（位图的宽度、位图的高度、位图的水平 DPI、位图的垂直 DPI、位图的格式）*/
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)frameworkElement.ActualWidth * 2, (int)frameworkElement.ActualHeight * 2, 100, 100, PixelFormats.Default);
            //呈现Visual对象
            bmp.Render(frameworkElement);
            //3、对象的集合编码转换成图像流
            BitmapEncoder encoder = new TiffBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            //4、保存到路径中
            encoder.Save(fs);
            //5、释放资源
            fs.Close();
        }
        #endregion
    }
}
