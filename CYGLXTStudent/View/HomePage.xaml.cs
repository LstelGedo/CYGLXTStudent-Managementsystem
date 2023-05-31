using System.Windows.Controls;

namespace CYGLXTStudent.Views
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : UserControl
    {
        public HomePage()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.HomePageViewModel();
        }
    }
}
