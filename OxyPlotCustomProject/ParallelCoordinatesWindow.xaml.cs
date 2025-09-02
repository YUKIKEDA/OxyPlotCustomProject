using System.Windows;

namespace OxyPlotCustomProject
{
    /// <summary>
    /// Interaction logic for ParallelCoordinatesWindow.xaml
    /// </summary>
    public partial class ParallelCoordinatesWindow : Window
    {
        public ParallelCoordinatesWindow()
        {
            InitializeComponent();
            this.DataContext = new ParallelCoordinatesViewModel();
        }
    }
}