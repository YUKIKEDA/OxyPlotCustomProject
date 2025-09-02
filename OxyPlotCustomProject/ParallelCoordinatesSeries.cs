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
        /// 線の色（単色の場合）
        /// </summary>
        public OxyColor LineColor { get; set; }

        /// <summary>
        /// 各線の色の値の配列（多色の場合）- カテゴリIDや数値
        /// </summary>
        public double[]? ColorValues { get; set; }

        /// <summary>
        /// カラースケール（色の値から色への変換）
        /// </summary>
        public List<ColorScalePoint>? ColorScale { get; set; }

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

                // この線の色を決定
                OxyColor lineColor = GetLineColor(lineIndex);

                // 線分として描画
                if (screenPoints.Count > 1)
                {
                    for (int i = 0; i < screenPoints.Count - 1; i++)
                    {
                        rc.DrawLine(new[] { screenPoints[i], screenPoints[i + 1] }, 
                            lineColor, LineThickness, EdgeRenderingMode.Automatic);
                    }
                }
            }
        }

        /// <summary>
        /// 指定したライン番号の色を取得します
        /// </summary>
        private OxyColor GetLineColor(int lineIndex)
        {
            // ColorValuesとColorScaleが設定されている場合は多色
            if (ColorValues != null && ColorScale != null && lineIndex < ColorValues.Length)
            {
                double colorValue = ColorValues[lineIndex];
                return InterpolateColor(colorValue);
            }

            // デフォルトは単色（透明度を適用）
            return OxyColor.FromArgb((byte)(LineOpacity * 255), LineColor.R, LineColor.G, LineColor.B);
        }

        /// <summary>
        /// カラースケールから色を補間します
        /// </summary>
        private OxyColor InterpolateColor(double value)
        {
            if (ColorScale == null || ColorScale.Count == 0)
                return LineColor;

            // 値がカラースケールの範囲外の場合
            if (value <= ColorScale[0].Value)
                return OxyColor.FromArgb((byte)(LineOpacity * 255), ColorScale[0].Color.R, ColorScale[0].Color.G, ColorScale[0].Color.B);
            if (value >= ColorScale[ColorScale.Count - 1].Value)
                return OxyColor.FromArgb((byte)(LineOpacity * 255), ColorScale[ColorScale.Count - 1].Color.R, ColorScale[ColorScale.Count - 1].Color.G, ColorScale[ColorScale.Count - 1].Color.B);

            // 値が範囲内の場合、線形補間
            for (int i = 0; i < ColorScale.Count - 1; i++)
            {
                var p1 = ColorScale[i];
                var p2 = ColorScale[i + 1];

                if (value >= p1.Value && value <= p2.Value)
                {
                    double t = (value - p1.Value) / (p2.Value - p1.Value);
                    
                    byte r = (byte)(p1.Color.R + t * (p2.Color.R - p1.Color.R));
                    byte g = (byte)(p1.Color.G + t * (p2.Color.G - p1.Color.G));
                    byte b = (byte)(p1.Color.B + t * (p2.Color.B - p1.Color.B));
                    
                    return OxyColor.FromArgb((byte)(LineOpacity * 255), r, g, b);
                }
            }

            return LineColor;
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

    /// <summary>
    /// カラースケールのポイント
    /// </summary>
    public class ColorScalePoint
    {
        /// <summary>
        /// カラースケールの値（0.0-1.0）
        /// </summary>
        public double Value { get; set; }
        
        /// <summary>
        /// その値での色
        /// </summary>
        public OxyColor Color { get; set; }

        public ColorScalePoint(double value, OxyColor color)
        {
            Value = value;
            Color = color;
        }
    }
}