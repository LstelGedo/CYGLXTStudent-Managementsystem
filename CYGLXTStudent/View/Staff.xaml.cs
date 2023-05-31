using System.Windows.Controls;

namespace CYGLXTStudent.Views
{
    /// <summary>
    /// Staff.xaml 的交互逻辑
    /// </summary>
    public partial class Staff : UserControl
    {
        public Staff()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.StaffViewModel();
        }
    }
}
