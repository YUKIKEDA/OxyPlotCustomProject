using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OxyPlotCustomProject
{
    /// <summary>
    /// Parallel Coordinates Plot専用のカスタムシリーズ
    /// </summary>
    public class ParallelCoordinatesSeries : XYAxisSeries
    {
        /// <summary>
        /// 次元（軸）のリスト - 各軸に値の配列を持つ
        /// </summary>
        public List<ParallelDimension> Dimensions { get; set; }

        /// <summary>
        /// 線の色
        /// </summary>
        public OxyColor LineColor { get; set; }

        /// <summary>
        /// 線の太さ
        /// </summary>
        public double LineThickness { get; set; }

        /// <summary>
        /// 線の透明度
        /// </summary>
        public double LineOpacity { get; set; }


        /// <summary>
        /// 軸ラベルを上部に表示するかどうか
        /// </summary>
        public bool ShowAxisLabelsTop { get; set; }

        /// <summary>
        /// 軸ラベルを下部に表示するかどうか
        /// </summary>
        public bool ShowAxisLabelsBottom { get; set; }

        public ParallelCoordinatesSeries()
        {
            Dimensions = new List<ParallelDimension>();
            LineColor = OxyColors.Blue;
            LineThickness = 1.0;
            LineOpacity = 0.7;
            ShowAxisLabelsTop = true;
            ShowAxisLabelsBottom = false;
        }

        public override void Render(IRenderContext rc)
        {
            if (Dimensions == null || Dimensions.Count == 0)
                return;

            // 各軸の描画
            RenderAxes(rc);

            // データ線の描画
            RenderDataLines(rc);
        }

        private void RenderAxes(IRenderContext rc)
        {
            var axisColor = OxyColors.Black;
            var axisThickness = 1.0;
            
            // 上下に余白を設ける
            double plotAreaMargin = 30.0;

            for (int i = 0; i < Dimensions.Count; i++)
            {
                var dimension = Dimensions[i];
                double x = XAxis.Transform(i);

                // 軸の垂直線を描画（余白を考慮）
                var topPoint = new ScreenPoint(x, PlotModel.PlotArea.Top + plotAreaMargin);
                var bottomPoint = new ScreenPoint(x, PlotModel.PlotArea.Bottom - plotAreaMargin);

                rc.DrawLine(new[] { topPoint, bottomPoint }, axisColor, axisThickness, EdgeRenderingMode.Automatic);

                // 軸のタイトルを上部に描画
                if (ShowAxisLabelsTop)
                {
                    var titleTopPoint = new ScreenPoint(x, PlotModel.PlotArea.Top + plotAreaMargin - 20);
                    rc.DrawText(titleTopPoint, dimension.Label, OxyColors.Black, fontFamily: "Arial", fontSize: 12, 
                        fontWeight: OxyPlot.FontWeights.Bold, rotation: 0, 
                        horizontalAlignment: OxyPlot.HorizontalAlignment.Center, 
                        verticalAlignment: OxyPlot.VerticalAlignment.Bottom);
                }

                // 軸のタイトルを下部に描画（オプション）
                if (ShowAxisLabelsBottom)
                {
                    var titleBottomPoint = new ScreenPoint(x, PlotModel.PlotArea.Bottom - plotAreaMargin + 20);
                    rc.DrawText(titleBottomPoint, dimension.Label, OxyColors.Gray, fontFamily: "Arial", fontSize: 10, 
                        fontWeight: OxyPlot.FontWeights.Normal, rotation: 0, 
                        horizontalAlignment: OxyPlot.HorizontalAlignment.Center, 
                        verticalAlignment: OxyPlot.VerticalAlignment.Top);
                }

                // 軸の目盛りを描画
                RenderAxisTicks(rc, i, dimension);
            }
        }

        private void RenderAxisTicks(IRenderContext rc, int axisIndex, ParallelDimension dimension)
        {
            double x = XAxis.Transform(axisIndex);
            int tickCount = 5;
            
            // 上下に余白を設けるため、プロット領域を少し縮小
            double plotAreaMargin = 30.0;
            double availableHeight = PlotModel.PlotArea.Height - (2 * plotAreaMargin);
            double plotTop = PlotModel.PlotArea.Top + plotAreaMargin;
            double plotBottom = PlotModel.PlotArea.Bottom - plotAreaMargin;
            
            for (int t = 0; t <= tickCount; t++)
            {
                double value = dimension.Range[0] + (dimension.Range[1] - dimension.Range[0]) * t / tickCount;
                double normalizedValue = (value - dimension.Range[0]) / (dimension.Range[1] - dimension.Range[0]);
                double y = plotBottom - normalizedValue * availableHeight;

                // 目盛り線
                rc.DrawLine(new[] { 
                    new ScreenPoint(x - 5, y), 
                    new ScreenPoint(x + 5, y) 
                }, OxyColors.Black, 1.0, EdgeRenderingMode.Automatic);

                // 目盛りラベル
                rc.DrawText(new ScreenPoint(x + 10, y), value.ToString("F1"), OxyColors.Black, 
                    fontFamily: "Arial", fontSize: 10, fontWeight: OxyPlot.FontWeights.Normal, 
                    rotation: 0, horizontalAlignment: OxyPlot.HorizontalAlignment.Left, 
                    verticalAlignment: OxyPlot.VerticalAlignment.Middle);
            }
        }

        private void RenderDataLines(IRenderContext rc)
        {
            var color = OxyColor.FromArgb((byte)(LineOpacity * 255), LineColor.R, LineColor.G, LineColor.B);
            
            // 上下に余白を設ける
            double plotAreaMargin = 30.0;
            double availableHeight = PlotModel.PlotArea.Height - (2 * plotAreaMargin);
            double plotBottom = PlotModel.PlotArea.Bottom - plotAreaMargin;

            if (Dimensions.Count == 0)
                return;

            // 各次元の値の数を取得（すべて同じ数でなければならない）
            int valueCount = Dimensions[0].Values.Length;
            
            // 各値（線）について描画
            for (int lineIndex = 0; lineIndex < valueCount; lineIndex++)
            {
                var screenPoints = new List<ScreenPoint>();

                // 各次元の値を取得してスクリーン座標に変換
                for (int dimIndex = 0; dimIndex < Dimensions.Count; dimIndex++)
                {
                    var dimension = Dimensions[dimIndex];
                    
                    if (lineIndex >= dimension.Values.Length)
                        break;

                    double value = dimension.Values[lineIndex];
                    double normalizedValue = (value - dimension.Range[0]) / (dimension.Range[1] - dimension.Range[0]);
                    
                    // Y座標を画面座標に変換（余白を考慮）
                    double y = plotBottom - normalizedValue * availableHeight;
                    double x = XAxis.Transform(dimIndex);

                    screenPoints.Add(new ScreenPoint(x, y));
                }

                // 線分として描画
                if (screenPoints.Count > 1)
                {
                    for (int i = 0; i < screenPoints.Count - 1; i++)
                    {
                        rc.DrawLine(new[] { screenPoints[i], screenPoints[i + 1] }, 
                            color, LineThickness, EdgeRenderingMode.Automatic);
                    }
                }
            }
        }

        protected override void UpdateData()
        {
            // データの更新処理
        }

        public override TrackerHitResult? GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            // 最寄りポイントの取得（簡略化）
            return null;
        }
    }

    /// <summary>
    /// Parallel Coordinates用の次元（軸）情報
    /// </summary>
    public class ParallelDimension
    {
        /// <summary>
        /// 軸のラベル
        /// </summary>
        public string Label { get; set; }
        
        /// <summary>
        /// 値の配列
        /// </summary>
        public double[] Values { get; set; }
        
        /// <summary>
        /// 値の範囲 [最小値, 最大値]
        /// </summary>
        public double[] Range { get; set; }
        
        /// <summary>
        /// カスタム目盛り値（オプション）
        /// </summary>
        public double[]? TickVals { get; set; }
        
        /// <summary>
        /// カスタム目盛りラベル（オプション）
        /// </summary>
        public string[]? TickText { get; set; }

        public ParallelDimension(string label, double[] values)
        {
            Label = label;
            Values = values;
            
            // 自動的に範囲を計算
            if (values.Length > 0)
            {
                double min = values.Min();
                double max = values.Max();
                double range = max - min;
                Range = new double[] { min - range * 0.1, max + range * 0.1 };
            }
            else
            {
                Range = new double[] { 0, 1 };
            }
        }

        public ParallelDimension(string label, double[] values, double[] range)
        {
            Label = label;
            Values = values;
            Range = range;
        }
    }
}