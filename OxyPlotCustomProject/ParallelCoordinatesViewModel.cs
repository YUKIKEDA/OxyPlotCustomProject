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

            CreateSampleData();
        }

        private void CreateSampleData()
        {
            // より多くのサンプルデータを生成（実際のIrisデータセットに近い量）
            var sepalLengths = new List<double>();
            var sepalWidths = new List<double>();
            var petalLengths = new List<double>();
            var petalWidths = new List<double>();
            var speciesIdsList = new List<double>();
            
            var random = new Random(42); // 固定シードで再現可能

            // Setosa データ（30個）
            for (int i = 0; i < 30; i++)
            {
                sepalLengths.Add(4.5 + random.NextDouble() * 1.2); // 4.5-5.7
                sepalWidths.Add(2.9 + random.NextDouble() * 1.0);  // 2.9-3.9
                petalLengths.Add(1.0 + random.NextDouble() * 0.8); // 1.0-1.8
                petalWidths.Add(0.1 + random.NextDouble() * 0.3);  // 0.1-0.4
                speciesIdsList.Add(0.0); // Setosa
            }

            // Versicolor データ（30個）
            for (int i = 0; i < 30; i++)
            {
                sepalLengths.Add(5.5 + random.NextDouble() * 1.5); // 5.5-7.0
                sepalWidths.Add(2.5 + random.NextDouble() * 0.8);  // 2.5-3.3
                petalLengths.Add(3.8 + random.NextDouble() * 1.4); // 3.8-5.2
                petalWidths.Add(1.0 + random.NextDouble() * 0.8);  // 1.0-1.8
                speciesIdsList.Add(0.5); // Versicolor
            }

            // Virginica データ（30個）
            for (int i = 0; i < 30; i++)
            {
                sepalLengths.Add(6.0 + random.NextDouble() * 1.5); // 6.0-7.5
                sepalWidths.Add(2.7 + random.NextDouble() * 0.8);  // 2.7-3.5
                petalLengths.Add(4.8 + random.NextDouble() * 1.7); // 4.8-6.5
                petalWidths.Add(1.8 + random.NextDouble() * 0.7);  // 1.8-2.5
                speciesIdsList.Add(1.0); // Virginica
            }

            var dimensions = new List<ParallelDimension>
            {
                new ParallelDimension("Sepal_Length", sepalLengths.ToArray()),
                new ParallelDimension("Sepal_Width", sepalWidths.ToArray()),
                new ParallelDimension("Petal_Length", petalLengths.ToArray()),
                new ParallelDimension("Petal_Width", petalWidths.ToArray())
            };

            // カラースケール（より鮮やかな色で区別しやすく）
            var colorScale = new List<ColorScalePoint>
            {
                new ColorScalePoint(0.0, OxyColors.Red),      // Setosa = 赤
                new ColorScalePoint(0.5, OxyColors.Green),    // Versicolor = 緑
                new ColorScalePoint(1.0, OxyColors.Blue)      // Virginica = 青
            };

            // シリーズを作成
            var series = new ParallelCoordinatesSeries
            {
                Title = $"Iris Dataset ({speciesIdsList.Count} samples - Color by Species)",
                LineColor = OxyColors.SteelBlue, // デフォルト色（使用されない）
                LineThickness = 1.5, // 線が多いので少し細く
                LineOpacity = 0.7,   // 線が多いので少し透明に
                Dimensions = dimensions,
                ColorValues = speciesIdsList.ToArray(),
                ColorScale = colorScale,
                ShowAxisLabelsTop = true,
                ShowAxisLabelsBottom = false
            };

            this.PlotModel.Series.Add(series);
        }

        /// <summary>
        /// ハイライトと選択状態をリセットします
        /// </summary>
        public void ResetHighlightAndSelection()
        {
            var parallelSeries = this.PlotModel.Series.OfType<ParallelCoordinatesSeries>().FirstOrDefault();
            if (parallelSeries != null)
            {
                parallelSeries.ResetHighlightAndSelection();
            }
        }
    }
}