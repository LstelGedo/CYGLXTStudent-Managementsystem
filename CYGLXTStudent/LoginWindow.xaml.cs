using System.Windows;

namespace CYGLXTStudent
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.LoginViewModel();
        }
    }
}
