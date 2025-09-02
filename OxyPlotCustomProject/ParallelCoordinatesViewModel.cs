using OxyPlot;
using OxyPlot.Legends;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OxyPlotCustomProject
{
    public class ParallelCoordinatesViewModel
    {
        public PlotModel PlotModel { get; }

        public ParallelCoordinatesViewModel()
        {
            this.PlotModel = new PlotModel { Title = "Parallel Coordinates Plot Demo" };

            // 軸を非表示にする（カスタムシリーズで描画）
            this.PlotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                IsAxisVisible = false,
                Minimum = -0.5,
                Maximum = 3.5
            });

            this.PlotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                IsAxisVisible = false,
                Minimum = 0,
                Maximum = 8
            });

            CreateSampleData();
        }

        private void CreateSampleData()
        {
            // 正しいParallel Coordinatesのデータ構造
            // 各軸に値の配列を持つ
            var dimensions = new List<ParallelDimension>
            {
                new ParallelDimension("Sepal_Length", new double[] { 50.1, 40.9, 40.7, 70.0, 60.4, 60.9, 60.3, 50.8, 70.1 }),
                new ParallelDimension("Sepal_Width", new double[] { 3.5, 3.0, 3.2, 3.2, 3.2, 3.1, 3.3, 2.7, 3.0 }),
                new ParallelDimension("Petal_Length", new double[] { 1.4, 1.4, 1.3, 4.7, 4.5, 4.9, 6.0, 5.1, 5.9 }),
                new ParallelDimension("Petal_Width", new double[] { 0.2, 0.2, 0.2, 1.4, 1.5, 1.5, 2.5, 1.9, 2.1 })
            };

            // シリーズを作成
            var series = new ParallelCoordinatesSeries
            {
                Title = "Iris Dataset",
                LineColor = OxyColors.SteelBlue,
                LineThickness = 1.5,
                LineOpacity = 0.7,
                Dimensions = dimensions,
                ShowAxisLabelsTop = true,
                ShowAxisLabelsBottom = false
            };

            this.PlotModel.Series.Add(series);
        }
    }
}