using OxyPlot;
using OxyPlot.Legends;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows;

namespace OxyPlotCustomProject
{
    public class ParallelCoordinatesViewModel : INotifyPropertyChanged
    {
        public PlotModel PlotModel { get; }
        
        public ICommand ResetCommand { get; }
        public ICommand MouseMoveCommand { get; }
        public ICommand MouseDownCommand { get; }
        
        // ツールチップカスタマイズ用のコマンド
        public ICommand UseDefaultTooltipCommand { get; }
        public ICommand UseCustomTooltipCommand { get; }
        public ICommand ChangeTooltipStyleCommand { get; }
        
        public event PropertyChangedEventHandler? PropertyChanged;
        
        private ParallelCoordinatesSeries? _currentSeries;

        public ParallelCoordinatesViewModel()
        {
            this.PlotModel = new PlotModel { Title = "Parallel Coordinates Plot Demo" };
            
            // コマンドを初期化
            ResetCommand = new RelayCommand(ResetHighlightAndSelection);
            MouseMoveCommand = new RelayCommand<ScreenPoint>(HandleMouseMove);
            MouseDownCommand = new RelayCommand<ScreenPoint>(HandleMouseDown);
            
            // ツールチップカスタマイズ用コマンドを初期化
            UseDefaultTooltipCommand = new RelayCommand(UseDefaultTooltip);
            UseCustomTooltipCommand = new RelayCommand(UseCustomTooltip);
            ChangeTooltipStyleCommand = new RelayCommand(ChangeTooltipStyle);

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

            _currentSeries = series;
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

        /// <summary>
        /// マウス移動時の処理
        /// </summary>
        /// <param name="screenPoint">スクリーン座標</param>
        private void HandleMouseMove(ScreenPoint screenPoint)
        {
            var parallelSeries = this.PlotModel.Series.OfType<ParallelCoordinatesSeries>().FirstOrDefault();
            if (parallelSeries != null)
            {
                parallelSeries.GetNearestPoint(screenPoint, false);
            }
        }

        /// <summary>
        /// マウスクリック時の処理
        /// </summary>
        /// <param name="screenPoint">スクリーン座標</param>
        private void HandleMouseDown(ScreenPoint screenPoint)
        {
            var parallelSeries = this.PlotModel.Series.OfType<ParallelCoordinatesSeries>().FirstOrDefault();
            if (parallelSeries != null)
            {
                parallelSeries.HandleMouseDown(screenPoint);
            }
        }

        /// <summary>
        /// デフォルトのツールチップを使用します
        /// </summary>
        private void UseDefaultTooltip()
        {
            if (_currentSeries != null)
            {
                _currentSeries.CustomTooltipFormatter = null;
                _currentSeries.TooltipStyle = new TooltipStyle();
                PlotModel.InvalidatePlot(false);
            }
        }

        /// <summary>
        /// カスタムツールチップを使用します
        /// </summary>
        private void UseCustomTooltip()
        {
            if (_currentSeries != null)
            {
                // カスタムフォーマッターを設定
                _currentSeries.CustomTooltipFormatter = (lineIndex, dimensions, colorValues) =>
                {
                    var lines = new List<string>();
                    
                    // ヘッダー（種類名を表示）
                    if (colorValues != null && lineIndex < colorValues.Length)
                    {
                        string speciesName = colorValues[lineIndex] switch
                        {
                            0.0 => "Iris Setosa",
                            0.5 => "Iris Versicolor", 
                            1.0 => "Iris Virginica",
                            _ => "Unknown Species"
                        };
                        lines.Add($"🌸 {speciesName}");
                    }
                    else
                    {
                        lines.Add($"📊 データポイント {lineIndex}");
                    }
                    
                    lines.Add("━━━━━━━━━━━━━━━");
                    
                    // 各次元の値を日本語で表示
                    var dimensionLabels = new[] { "萼片の長さ", "萼片の幅", "花弁の長さ", "花弁の幅" };
                    var units = new[] { "cm", "cm", "cm", "cm" };
                    
                    for (int i = 0; i < Math.Min(dimensions.Length, dimensionLabels.Length); i++)
                    {
                        if (lineIndex < dimensions[i].Values.Length)
                        {
                            double value = dimensions[i].Values[lineIndex];
                            lines.Add($"• {dimensionLabels[i]}: {value:F1} {units[i]}");
                        }
                    }
                    
                    return string.Join("\n", lines);
                };
                
                PlotModel.InvalidatePlot(false);
            }
        }

        /// <summary>
        /// ツールチップのスタイルを変更します
        /// </summary>
        private void ChangeTooltipStyle()
        {
            if (_currentSeries != null)
            {
                // スタイルを変更
                _currentSeries.TooltipStyle = new TooltipStyle
                {
                    BackgroundColor = OxyColor.FromArgb(220, 30, 30, 60),  // 半透明の濃い青
                    BorderColor = OxyColors.Gold,
                    BorderThickness = 2.0,
                    TextColor = OxyColors.White,
                    FontWeight = 700 // Bold weight
                };
                
                PlotModel.InvalidatePlot(false);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}