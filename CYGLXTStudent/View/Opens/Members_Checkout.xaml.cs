using System.Windows;

namespace CYGLXTStudent.Views.Opens
{
    /// <summary>
    /// Members_Checkout.xaml 的交互逻辑
    /// </summary>
    public partial class Members_Checkout : Window
    {
        public Members_Checkout()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.Opens.MembersCheckoutViewModel();
        }
    }
}
