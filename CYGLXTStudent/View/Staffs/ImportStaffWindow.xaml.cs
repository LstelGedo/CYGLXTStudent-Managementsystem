using System.Windows;

namespace CYGLXTStudent.Views.Staffs
{
    /// <summary>
    /// ImportStaffWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ImportStaffWindow : Window
    {
        public ImportStaffWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.Staffs.ImportStaffViewModel();
        }
    }
}
