using CYGLXTStudent.Model;
using CYGLXTStudent.Model.Vo;
using CYGLXTStudent.Resources.PublicClass;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CYGLXTStudent.ViewModel.Staffs
{
    /// <summary>
    /// 员工个人信息查询
    /// </summary>
    public class StaffInformationViewModel: ViewModelBase
    {
        public StaffInformationViewModel()
        {
            LoadedCommand=new RelayCommand (Loaded);
            //关闭
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow, win => win != null);
        }
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        CYGLXTEntities myModel = new CYGLXTEntities();
        #region 【1、属性】
        private int staffID;
        /// <summary>
        /// 员工ID
        /// </summary>
        public int StaffID
        {
            get { return staffID; }
            set
            {
                if (staffID != value)
                {
                    staffID = value;
                    RaisePropertyChanged(() => StaffID);


                } }
        }

        private StaffVo currentStaffVo;
        /// <summary>
        /// 员工实体
        /// </summary>
        public StaffVo CurrentStaffVo
        {
            get { return currentStaffVo; }
            set
            {
                if (currentStaffVo != value)
                {
                    currentStaffVo = value;
                    RaisePropertyChanged(() => CurrentStaffVo);
                }
            }
        }

        private BitmapImage imgPicture;
        /// <summary>
        /// 员工图片
        /// </summary>
        public BitmapImage ImgPicture
        {
            get { return imgPicture; }
            set
            {
                if (imgPicture != value)
                {
                    imgPicture = value;
                    RaisePropertyChanged(() => ImgPicture);

                }
            }
        }

        private string age;
        /// <summary>
        /// 年龄
        /// </summary>
        public string Age
        {
            get { return age; }
            set
            {
                if (age != value)
                {
                    age = value;
                    RaisePropertyChanged(() => Age);
                }
            }
        }
        #endregion
        #region 【2、命令】
        /// <summary>
        /// 2.1 加载命令
        /// </summary>
        public RelayCommand LoadedCommand { get; set; }
        /// <summary>
        /// 2.2 关闭窗口命令
        /// </summary>
        public RelayCommand<Window> CloseWindowCommand { get; set; }
        #endregion
        #region 【3、函数】
        /// <summary>
        /// 加载（绑定员工信息）
        /// </summary>
        private void Loaded()
        {
            /*
           * 思路
              1、根据登录人员工ID查询员工信息；   

              2、调用封装【byte[] 转换为BitmapImage】的方法绑定员工图片
           */
            try
            {
                //1、根据登录人员工ID查询员工信息；
                CurrentStaffVo = (from tb in myModel.S_Staff
                                  where tb.staffID == StaffID
                                  select new StaffVo
                                  {
                                      
                                      IDNumber=tb.IDNumber,
                                      picture=tb.picture,
                                      name=tb.name,
                                      staffID=tb.staffID,
                                      EMPNO=tb.EMPNO,
                                      phone=tb.phone,
                                      sex=tb.sex,
                                      position=tb.position,
                                      statust=tb.statust,
                                      address=tb.address,
                                      StrEntryDate = tb.entryDate.ToString(),
                                  }).Single();



                //2、调用封装【byte[] 转换为BitmapImage】的方法绑定员工图片
                ImgPicture = ByteToBitmapimage.Bytearraytobitmapimage(CurrentStaffVo.picture);
                //3、年龄
                string strIDNumber = CurrentStaffVo.IDNumber.ToString();//获取身份证号码
                string strYear = strIDNumber.Substring(6, 4);//截取身份证号码的出生年份
                string strNowYear = DateTime.Now.ToString("yyyy");//获取当前年份
                decimal decYear = 0;
                decimal decNowYear = 0;
                if (strYear != null) decYear = Convert.ToDecimal(strYear);
                if (strNowYear != null) decNowYear = Convert.ToDecimal(strNowYear);
                decimal decAge = decNowYear - decYear;//年份相减
                Age = decAge.ToString();//年龄               

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.2 关闭窗口
        /// </summary>
        /// <param name="window">当前窗口</param>
        private void CloseWindow(Window window)
        {
            if (window!=null)
            {
                window.Close();
            }
        }
        #endregion
    }
}
