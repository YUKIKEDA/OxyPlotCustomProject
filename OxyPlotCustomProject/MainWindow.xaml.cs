using System.Windows;

namespace OxyPlotCustomProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }

        private void OpenParallelCoordinatesPlot_Click(object sender, RoutedEventArgs e)
        {
            var parallelCoordinatesWindow = new ParallelCoordinatesWindow();
            parallelCoordinatesWindow.Show();
        }

        private void OpenClickableScatterPlot_Click(object sender, RoutedEventArgs e)
        {
            var clickableScatterWindow = new ClickableScatterWindow();
            clickableScatterWindow.Show();
        }

        private void OpenRectangleSelectionPlot_Click(object sender, RoutedEventArgs e)
        {
            var rectangleSelectionWindow = new RectangleSelectionWindow();
            rectangleSelectionWindow.Show();
        }
    }
}