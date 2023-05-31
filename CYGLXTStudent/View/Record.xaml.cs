using System.Windows.Controls;

namespace CYGLXTStudent.Views
{
    /// <summary>
    /// Record.xaml 的交互逻辑
    /// </summary>
    public partial class Record : UserControl
    {
        public Record()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.RecordViewModel();
        }
    }
}
