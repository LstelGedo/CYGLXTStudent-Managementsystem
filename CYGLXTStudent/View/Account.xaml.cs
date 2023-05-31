using System.Windows.Controls;

namespace CYGLXTStudent.Views
{
    /// <summary>
    /// Account.xaml 的交互逻辑
    /// </summary>
    public partial class Account : UserControl
    {
        public Account()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.AccountViewModel();
        }
    }
}
