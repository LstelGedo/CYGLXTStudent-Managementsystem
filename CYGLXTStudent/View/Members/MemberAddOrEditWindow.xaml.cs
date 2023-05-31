using System.Windows;

namespace CYGLXTStudent.Views.Members
{
    /// <summary>
    /// MemberAddOrEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MemberAddOrEditWindow : Window
    {
        public MemberAddOrEditWindow()
        {
            InitializeComponent();
            this.DataContext=new ViewModel.Members.MemberAddOrEditViewModel();
        }
    }
}
