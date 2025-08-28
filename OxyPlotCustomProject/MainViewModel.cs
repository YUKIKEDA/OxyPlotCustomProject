using System;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Legends;

namespace OxyPlotCustomProject
{
    public class MainViewModel
    {
        public PlotModel PlotModel { get; }

        public MainViewModel()
        {
            this.PlotModel = new PlotModel { Title = "CustomScatterLineSeries Demo" };

            // 凡例の表示はシリーズの Title を設定し、明示的に Legend を追加することで行います
            // OxyPlot のバージョンにより PlotModel に Legends コレクションが用意されているのでそれを使う
            var legend = new Legend
            {
                LegendTitle = "凡例",
                LegendPosition = LegendPosition.TopRight,
                LegendPlacement = LegendPlacement.Outside
            };
            this.PlotModel.Legends.Add(legend);

            var series = new CustomScatterLineSeries
            {
                LineColor = OxyColors.SteelBlue,
                LineThickness = 1.5,
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColors.Black,
                MarkerSize = 8 // Use series-level MarkerSize
            };

            // シリーズ名（凡例に表示されるラベル）
            series.Title = "サンプル系列";

            // Add some sample points with error bars
            // Pass double.NaN for per-point size so the series' MarkerSize is used
            series.Points.Add(new ScatterErrorPoint(0, 0, 0.4, 0.2, double.NaN, 0, null));
            series.Points.Add(new ScatterErrorPoint(1, 1, 0.3, 0.4, double.NaN, 0, null));
            series.Points.Add(new ScatterErrorPoint(2, 0.5, 0.2, 0.3, double.NaN, 0, null));
            series.Points.Add(new ScatterErrorPoint(3, 1.2, 0.5, 0.2, double.NaN, 0, null));

            this.PlotModel.Series.Add(series);
        }
    }
}