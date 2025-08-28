using System;
using OxyPlot;
using OxyPlot.Series;

namespace OxyPlotCustomProject
{
    public class MainViewModel
    {
        public PlotModel PlotModel { get; }

        public MainViewModel()
        {
            this.PlotModel = new PlotModel { Title = "CustomScatterLineSeries Demo" };

            var series = new CustomScatterLineSeries
            {
                LineColor = OxyColors.SteelBlue,
                LineThickness = 1.5,
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColors.White,
                MarkerSize = 4
            };

            // Add some sample points with error bars
            series.Points.Add(new ScatterErrorPoint(0, 0, 0.4, 0.2, 4, 0, null));
            series.Points.Add(new ScatterErrorPoint(1, 1, 0.3, 0.4, 4, 0, null));
            series.Points.Add(new ScatterErrorPoint(2, 0.5, 0.2, 0.3, 4, 0, null));
            series.Points.Add(new ScatterErrorPoint(3, 1.2, 0.5, 0.2, 4, 0, null));

            this.PlotModel.Series.Add(series);
        }
    }
}
