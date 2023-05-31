using System.Windows;

namespace CYGLXTStudent.Views.Members
{
    /// <summary>
    /// CancelMembershipWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CancelMembershipWindow : Window
    {
        public CancelMembershipWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.Members.CancelMembershipViewModel();
        }
    }
}
