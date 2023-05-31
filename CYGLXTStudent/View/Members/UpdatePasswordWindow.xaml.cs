using System.Windows;

namespace CYGLXTStudent.Views.Members
{
    /// <summary>
    /// UpdatePasswordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UpdatePasswordWindow : Window
    {
        public UpdatePasswordWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.Members.UpdatePasswordViewModel();
        }
    }
}
