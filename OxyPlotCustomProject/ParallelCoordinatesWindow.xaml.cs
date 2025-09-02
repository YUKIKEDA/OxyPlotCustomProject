using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace OxyPlotCustomProject
{
    /// <summary>
    /// Interaction logic for ParallelCoordinatesWindow.xaml
    /// </summary>
    public partial class ParallelCoordinatesWindow : Window
    {
        private ParallelCoordinatesViewModel viewModel;

        public ParallelCoordinatesWindow()
        {
            InitializeComponent();
            viewModel = new ParallelCoordinatesViewModel();
            this.DataContext = viewModel;
            
            SetupDirectMouseHandling();
        }

        private void SetupDirectMouseHandling()
        {
            // PlotViewに直接マウスイベントを追加
            plotView.MouseMove += PlotView_MouseMove;
            plotView.MouseLeftButtonDown += PlotView_MouseLeftButtonDown;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ResetHighlightAndSelection();
        }

        private void PlotView_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(plotView);
            var screenPoint = new OxyPlot.ScreenPoint(position.X, position.Y);
            
            var parallelSeries = viewModel.PlotModel.Series.OfType<ParallelCoordinatesSeries>().FirstOrDefault();
            if (parallelSeries != null)
            {
                parallelSeries.GetNearestPoint(screenPoint, false);
            }
        }

        private void PlotView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(plotView);
            var screenPoint = new OxyPlot.ScreenPoint(position.X, position.Y);
            
            var parallelSeries = viewModel.PlotModel.Series.OfType<ParallelCoordinatesSeries>().FirstOrDefault();
            if (parallelSeries != null)
            {
                parallelSeries.HandleMouseDown(screenPoint);
                e.Handled = true;
            }
        }
    }
}