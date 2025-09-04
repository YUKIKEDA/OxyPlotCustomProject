using System.Windows;

namespace OxyPlotCustomProject
{
    /// <summary>
    /// ClickableScatterWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ClickableScatterWindow : Window
    {
        public ClickableScatterWindow()
        {
            InitializeComponent();
            this.DataContext = new ClickableScatterViewModel();
        }
    }
}
