using CYGLXTStudent.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CYGLXTStudent.ViewModel.Menus
{
    /// <summary>
    /// 编辑菜系（新增/修改）
    /// </summary>
    public class MenuAddOrEditViewModel : ViewModelBase
    {
        public MenuAddOrEditViewModel()
        {
            //保存
            SaveCommand = new RelayCommand<Window>(Save, obj => obj != null);
            //关闭
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow, obj => obj != null);
            //选择图片
            OpenCommand = new RelayCommand(ChoisePicture);
            //清空图片
            CleanCommand = new RelayCommand(CleanPicture);
            //重置
            ResetCommand = new RelayCommand(Reset);
        }
        #region 【全局变量】
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        CYGLXTEntities myModel = new CYGLXTEntities();
        /// <summary>
        /// 定义委托
        /// </summary>
        public delegate void RefreshDelegate();
        /// <summary>
        /// 定义事件
        /// </summary>
        public event RefreshDelegate RefreshEvent;

        /// <summary>
        /// 全局变量（记录上传图片）
        /// </summary>
         List<byte[]> lstBytes = new List<byte[]>();
        /// <summary>
        /// 文件名
        /// </summary>
        private static string FileName;
        /// <summary>
        /// 文件路径
        /// </summary>
        private static string FullPath;
        #endregion
        #region 【1、属性】
        /// <summary>
        /// 私有字段：菜单实体
        /// </summary>
        private S_Menu menuEntity;
        /// <summary>
        /// 属性：菜单实体
        /// </summary>
        public S_Menu MenuEntity
        {
            get { return menuEntity; }
            set 
            {
                if (menuEntity != value)
                {
                    menuEntity = value;
                    RaisePropertyChanged(() => MenuEntity);
                }
            }
        }

        private bool isAdd=false;
        /// <summary>
        /// 新增/修改标志（默认false）
        /// </summary>
        public bool IsAdd
        {
            get { return isAdd; }
            set
            {
                if (isAdd!=value)
                {
                    isAdd = value;
                    RaisePropertyChanged(() => IsAdd);
                } 
            }
        }

        private BitmapImage caseCoverImage;
        /// <summary>
        /// 菜单图片
        /// </summary>
        public BitmapImage CaseCoverImage
        {
            get { return caseCoverImage; }
            set {
                if (caseCoverImage != value)
                {
                    caseCoverImage = value;
                    RaisePropertyChanged(() => CaseCoverImage);
                }
            }
        }
        /// <summary>
        ///字段： 菜单分类下拉框
        /// </summary>
        public string cuisine;
        /// <summary>
        ///属性： 菜单分类下拉框
        /// </summary>
        public string Cuisine
        {
            get { return cuisine; }
            set
            {
                if (cuisine != value)
                {
                    cuisine = value;
                    RaisePropertyChanged(() => Cuisine);
                }
            }
        }

        /// <summary>
        /// 字段：文件路径
        /// </summary>
        public string oldImage;
        /// <summary>
        /// 属性：文件路径(旧路径)
        /// </summary>
        public string OldImage
        {
            get { return oldImage; }
            set
            {
                if (oldImage != value)
                {
                    oldImage = value;
                    RaisePropertyChanged(() => OldImage);
                }
            }
        }
        #endregion
        #region 【2、命令】
        /// <summary>
        /// 2.1 关闭窗口命令
        /// </summary>
        public RelayCommand<Window> CloseWindowCommand { get; set; }
       
        /// <summary>
        /// 2.2 浏览图片命令
        /// </summary>
        public RelayCommand OpenCommand { get; set; }
        /// <summary>
        /// 2.3 清空图片命令
        /// </summary>
        public RelayCommand CleanCommand { get; set; }
        /// <summary>
        /// 2.4 保存菜单命令
        /// </summary>
        public RelayCommand<Window> SaveCommand { get; set; }
        /// <summary>
        /// 2.5 重置命令
        /// </summary>
        public RelayCommand ResetCommand { get; set; }
        #endregion
        #region 【3、方法】
        /// <summary>
        /// 3.1 关闭窗口
        /// </summary>
        /// <param name="window">窗口</param>
        private void CloseWindow(Window window)
        {
            if (window!=null)
            {
                window.Close();
            }
        }
        /// <summary>
        /// 3.2 重置窗口
        /// </summary>
        private void Reset()
        {
            if (IsAdd)
            {
                CaseCoverImage = null;//清空图片
                MenuEntity = new S_Menu();//新实体

            }
        }
        /// <summary>
        /// 3.3 选择图片
        /// </summary>
        private void ChoisePicture()
        {
            try
            {
                //声明两个局部变量
                Stream phpto = null;
                //1.1打开（文件框）
                OpenFileDialog ofdWenJian = new OpenFileDialog
                {
                    //设置是否允许多选
                    Multiselect = false,
                    //筛选文件类型（提示）
                    Filter = "ALL Image Files|*.*"
                };
                //显示对话框==点击确定按钮
                if (ofdWenJian.ShowDialog() == DialogResult.OK)
                {
                    //选定的文件(选定的文件打开只读流)
                    if ((phpto = ofdWenJian.OpenFile()) != null)
                    {
                        //获取流长度（用字节表示的流长度 ）                                          
                        int length = (int)phpto.Length;
                        //声明数组
                        byte[] bytes = new byte[length];
                        //读取文件（字节数组，从零开始的字节偏移量，读取的字节数）
                        phpto.Read(bytes, 0, length);
                        lstBytes.Add(bytes);
                        BitmapImage images = new BitmapImage(new Uri(ofdWenJian.FileName));
                        //绑定图片
                        CaseCoverImage = images;
                        FileName = ofdWenJian.FileName;
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("没办法选择图片！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("绑定图片出错，请重新选择！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.4 清空图片
        /// </summary>
        private void CleanPicture()
        {
            //清理List<byte[]>
            lstBytes.Clear();
            //清空图片
            CaseCoverImage = null;
            //文件名默认初始值
            FileName =string.Empty;
        }
        /// <summary>
        /// 3.5 保存菜单
        /// </summary>
        /// <param name="window">窗口</param>
        private void Save(Window window)
        {

            try
            {

                //获取页面属性，判断必填项不能为空；
                if (string.IsNullOrEmpty(MenuEntity.dishes))
                {
                    System.Windows.MessageBox.Show("请输入菜单名称！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                if (MenuEntity.price ==null )
                {
                    System.Windows.MessageBox.Show("请输入菜单价格！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                if (string.IsNullOrEmpty(Cuisine))
                {
                    System.Windows.MessageBox.Show("请输入菜单名称！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                //新增
                if (IsAdd)
                {
                    //1、判断重复性；
                    int intCount = myModel.S_Menu.Where(o => o.dishes == MenuEntity.dishes).Count();
                    if (intCount>0)
                    {
                        System.Windows.MessageBox.Show("菜单新增重复！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                    //2、实例化新增实体并赋值；
                    S_Menu menu = new S_Menu
                    {
                        dishes= MenuEntity.dishes,
                        price= MenuEntity.price,
                        cuisine= Cuisine.Trim(),
                        effective=true,
                    };
                    //声明局部变量（获取列表数据）
                    byte[][] bytePicture = new byte[lstBytes.Count][];
                    for (int i = 0; i < lstBytes.Count; i++)
                    {
                        bytePicture[i] = lstBytes[i];//提取图片
                    }
                    //调用封装方法获取文件名
                    string strName= ByteImageToFileName(bytePicture);
                    //3、图片文件保存
                    menu.picture = strName;
                    myModel.S_Menu.Add(menu);
                    //获取基目录下的目录
                    string strDB = AppDomain.CurrentDomain.BaseDirectory + @"MenuPicture\\";
                    if (!Directory.Exists(strDB))
                    {
                        Directory.CreateDirectory(strDB);
                    }
                    //路径=基目录下的目录+文件名
                    FullPath = strDB + strName;
                    //4、保存数据库表数据、图片文件保存
                    if (myModel.SaveChanges()>0)
                    {                      
                        //拷贝文件到指定位置
                        File.Copy(FileName, FullPath);
                        System.Windows.MessageBox.Show(MenuEntity.dishes+"菜单新增成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        //关闭窗口
                        window.Close();
                        //if (RefreshEvent!=null)
                        //{
                        //    RefreshEvent();
                        //}
                        //简化委托调用（刷新主页面）
                        RefreshEvent?.Invoke();                       
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(MenuEntity.dishes + "菜单新增失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);

                    }

                }
                //修改
                else
                {
                    //（1）判断重复性（不比较自身）
                    int intCount = myModel.S_Menu.Where(o => o.dishes == MenuEntity.dishes&& o.menuID!= MenuEntity.menuID).Count();
                    //（2）执行修改操作(文件保存+数据保存)
                    //①声明 byte[][], 获取上传的文件
                    byte[][] byPictrue= new byte[lstBytes.Count][];
                    //②for循环二进制数组，提取图片
                    for (int i = 0; i < lstBytes.Count; i++)
                    {
                        byPictrue[i] = lstBytes[i];//提取图片
                    }
                    //③查询菜单 by 菜单ID          
                    S_Menu menu = myModel.S_Menu.Find(MenuEntity.menuID);

                    //④获取基目录 + @"MenuPicture"
                    string strDB = AppDomain.CurrentDomain.BaseDirectory + @"MenuPicture\\";
                    //⑤调用封装的方法BytePictureToFileName，获取文件名
                    string strFileName = ByteImageToFileName(byPictrue);
                    //图片存在两种情况：（1）替换（新路径）、（2）保留原来OldImage
                    //⑥判断：文件名不为空，实体图片赋值，文件路径 = 基目录 + 文件名；
                    if (!string.IsNullOrEmpty(strFileName))
                    {
                        //（1）替换（新路径）
                        //实体图片赋值
                        menu.picture = strFileName;
                        //文件路径 = 基目录 + 文件名
                        FullPath = strDB + strFileName;

                    }
                    else
                    {
                        //（2）保留原来OldImage
                        //为空，文件路径 = 选中文件的文件名。
                        FullPath = FileName;
                    }
                    //⑦实体属性赋值
                    //实体属性赋值
                    menu.dishes = MenuEntity.dishes;//菜名
                    menu.price = Convert.ToDecimal(MenuEntity.price);//售价
                    menu.cuisine = Cuisine;//所属类型
                    //⑧执行修改
                    myModel.Entry(menu).State = System.Data.Entity.EntityState.Modified;
                    //⑨数据保存成功，
                    if (myModel.SaveChanges()>0)
                    {
                        //3、文件保存
                        //判断文件路径！= 文件名）                        
                        if (FullPath != FileName)
                        {
                            //替换图片:拷贝新文件
                            File.Copy(FileName, FullPath);
                            //删除旧文件
                            File.Delete(OldImage);
                        }
                        System.Windows.MessageBox.Show(MenuEntity.dishes + "菜单修改成功", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                        //关闭窗口
                        window.Close();
                        //简化委托调用（刷新主页面）
                        RefreshEvent?.Invoke();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(MenuEntity.dishes+"菜单修改失败", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                       

                }


            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 文件流转换路径
        /// </summary>
        /// <param name="byteTuPian">图片数组</param>
        private string ByteImageToFileName(byte[][] byteTuPian)
        {            
            try
            {
                //1、声明局部变量：文件路径、文件名
                string strPath = string.Empty;
                string strFileName = string.Empty;
                //2、AppDomain获取文件目录 + "image\\"；
                string strBD = AppDomain.CurrentDomain.BaseDirectory + "image\\";
                //3、Directory判断目录是否存在，目录不存在，创建目录；
                if (!Directory.Exists(strBD))
                {
                    Directory.CreateDirectory(strBD);
                }
                //4、声明局部变量文件名不含后缀，提取日期作为文件名；
                string strDate = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                //5、遍历二进制的数组的长度；
                for (int i = 0; i < byteTuPian.Length; i++)
                {
                    //（1）文件名：日期 + 后缀 ".png"
                    strFileName = strDate + ".png";
                    //（2）拼接文件保存路径 = 文件目录 + 文件名
                    strPath = strBD + strFileName;
                    //（3）FileInfo实例化创建文件
                    FileInfo fileInfo = new FileInfo(strPath);
                    //（4）声明FileStream，为文件提供Stream
                    FileStream stream;
                    //OpenWrite()创建只写 FileStream。
                    stream = fileInfo.OpenWrite();
                    //将字节块写入文件流。Write(数组，开始字节索引0开始，长度)
                    stream.Write(byteTuPian[i], 0, byteTuPian[i].Length);
                    //Close()关闭当前流并释放与之关联的所有资源
                    stream.Close();
                }
                //6、返回文件名
                return strFileName;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion


    }
}
