using System.Windows;

namespace CYGLXTStudent.Views.Staffs
{
    /// <summary>
    /// PrintStaffWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PrintStaffWindow : Window
    {
        public PrintStaffWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.Staffs.PrintStaffViewModel();
        }
    }
}
