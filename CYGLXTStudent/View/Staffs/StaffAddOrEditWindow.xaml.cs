using System.Windows;

namespace CYGLXTStudent.Views.Staffs
{
    /// <summary>
    /// StaffAddOrEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StaffAddOrEditWindow : Window
    {
        public StaffAddOrEditWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.Staffs.StaffAddOrEditViewModel();
        }
    }
}
