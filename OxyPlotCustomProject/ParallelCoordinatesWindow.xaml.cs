using System.Windows;

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
        }
    }
}