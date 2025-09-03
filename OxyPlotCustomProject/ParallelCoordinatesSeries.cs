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

        /// <summary>
        /// ハイライトされた線のインデックス（-1は未選択）
        /// </summary>
        public int HighlightedLineIndex { get; set; }

        /// <summary>
        /// 選択された線のインデックス（-1は未選択）
        /// </summary>
        public int SelectedLineIndex { get; set; }

        /// <summary>
        /// ハイライト時の線の太さ
        /// </summary>
        public double HighlightLineThickness { get; set; }

        /// <summary>
        /// 選択時の線の太さ
        /// </summary>
        public double SelectedLineThickness { get; set; }

        /// <summary>
        /// ハイライト時の線の透明度
        /// </summary>
        public double HighlightLineOpacity { get; set; }

        /// <summary>
        /// 非選択線の透明度（選択がある場合）
        /// </summary>
        public double UnselectedLineOpacity { get; set; }

        /// <summary>
        /// 軸の目盛り数
        /// </summary>
        public int AxisTickCount { get; set; }

        /// <summary>
        /// 固定表示中のツールチップ情報
        /// </summary>
        public FixedTooltip? FixedTooltipInfo { get; set; }

        public ParallelCoordinatesSeries()
        {
            Dimensions = new List<ParallelDimension>();
            LineColor = OxyColors.Blue;
            LineThickness = 1.0;
            LineOpacity = 0.7;
            ShowAxisLabelsTop = true;
            ShowAxisLabelsBottom = false;
            
            // インタラクション関連のデフォルト値
            HighlightedLineIndex = -1;
            SelectedLineIndex = -1;
            HighlightLineThickness = 3.0;
            SelectedLineThickness = 2.5;
            HighlightLineOpacity = 1.0;
            UnselectedLineOpacity = 0.3;
            AxisTickCount = 5;
        }

        public override void Render(IRenderContext rc)
        {
            if (Dimensions == null || Dimensions.Count == 0)
                return;

            // 各軸の描画
            RenderAxes(rc);

            // データ線の描画
            RenderDataLines(rc);

            // 固定ツールチップの描画
            RenderFixedTooltip(rc);
        }

        protected override void UpdateData()
        {
            // データの更新処理
        }

        public override TrackerHitResult? GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            // トラッカー機能を完全に無効化するため、ハイライト処理のみ実行してnullを返す
            if (Dimensions == null || Dimensions.Count == 0)
                return null;

            double plotAreaMargin = 30.0;
            double availableHeight = PlotModel.PlotArea.Height - (2 * plotAreaMargin);
            double plotBottom = PlotModel.PlotArea.Bottom - plotAreaMargin;

            int valueCount = Dimensions[0].Values.Length;
            double minDistance = double.MaxValue;
            int nearestLineIndex = -1;

            // 各線について距離をチェック
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
                    
                    double y = plotBottom - normalizedValue * availableHeight;
                    double x = XAxis.Transform(dimIndex);

                    screenPoints.Add(new ScreenPoint(x, y));
                }

                // 線分との距離を計算
                for (int i = 0; i < screenPoints.Count - 1; i++)
                {
                    double distance = DistanceToLineSegment(point, screenPoints[i], screenPoints[i + 1]);
                    if (distance < minDistance && distance < 20) // 20ピクセル以内（感度向上）
                    {
                        minDistance = distance;
                        nearestLineIndex = lineIndex;
                    }
                }
            }

            if (nearestLineIndex >= 0)
            {
                // ハイライト状態を更新
                var oldHighlight = HighlightedLineIndex;
                HighlightedLineIndex = nearestLineIndex;
                
                // 再描画が必要な場合
                if (oldHighlight != HighlightedLineIndex)
                {
                    PlotModel?.InvalidatePlot(false);
                }
            }
            else
            {
                // ハイライトをクリア
                if (HighlightedLineIndex >= 0)
                {
                    HighlightedLineIndex = -1;
                    PlotModel?.InvalidatePlot(false);
                }
            }

            // トラッカーを完全に無効化するためnullを返す
            return null;
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
                    var titleTopPoint = new ScreenPoint(x, PlotModel.PlotArea.Top + plotAreaMargin - 10);
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
            int tickCount = AxisTickCount;
            
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

                // この線の色と太さを決定
                var (lineColor, thickness) = GetLineVisualProperties(lineIndex);

                // 線分として描画
                if (screenPoints.Count > 1)
                {
                    for (int i = 0; i < screenPoints.Count - 1; i++)
                    {
                        rc.DrawLine(new[] { screenPoints[i], screenPoints[i + 1] }, 
                            lineColor, thickness, EdgeRenderingMode.Automatic);
                    }
                }
            }
        }

        /// <summary>
        /// 指定したライン番号の視覚的プロパティ（色、太さ）を取得します
        /// </summary>
        private (OxyColor color, double thickness) GetLineVisualProperties(int lineIndex)
        {
            var baseColor = GetBaseLineColor(lineIndex);
            double opacity = LineOpacity;
            double thickness = LineThickness;

            // ハイライト状態
            if (lineIndex == HighlightedLineIndex)
            {
                opacity = HighlightLineOpacity;
                thickness = HighlightLineThickness;
            }
            // 選択状態
            else if (lineIndex == SelectedLineIndex)
            {
                opacity = LineOpacity; // 選択された線は通常の透明度
                thickness = SelectedLineThickness;
            }
            // 他の線が選択されている場合、非選択線を薄く表示
            else if (SelectedLineIndex >= 0)
            {
                opacity = UnselectedLineOpacity;
            }

            var finalColor = OxyColor.FromArgb((byte)(opacity * 255), baseColor.R, baseColor.G, baseColor.B);
            return (finalColor, thickness);
        }

        /// <summary>
        /// 指定したライン番号の基本色を取得します
        /// </summary>
        private OxyColor GetBaseLineColor(int lineIndex)
        {
            // ColorValuesとColorScaleが設定されている場合は多色
            if (ColorValues != null && ColorScale != null && lineIndex < ColorValues.Length)
            {
                double colorValue = ColorValues[lineIndex];
                return InterpolateBaseColor(colorValue);
            }

            // デフォルトは単色
            return LineColor;
        }

        /// <summary>
        /// カラースケールから基本色を補間します
        /// </summary>
        private OxyColor InterpolateBaseColor(double value)
        {
            if (ColorScale == null || ColorScale.Count == 0)
                return LineColor;

            // 値がカラースケールの範囲外の場合
            if (value <= ColorScale[0].Value)
                return ColorScale[0].Color;
            if (value >= ColorScale[ColorScale.Count - 1].Value)
                return ColorScale[ColorScale.Count - 1].Color;

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
                    
                    return OxyColor.FromRgb(r, g, b);
                }
            }

            return LineColor;
        }

        /// <summary>
        /// 点から線分までの距離を計算
        /// </summary>
        private double DistanceToLineSegment(ScreenPoint point, ScreenPoint lineStart, ScreenPoint lineEnd)
        {
            double dx = lineEnd.X - lineStart.X;
            double dy = lineEnd.Y - lineStart.Y;
            
            if (Math.Abs(dx) < 1e-10 && Math.Abs(dy) < 1e-10)
            {
                // 線分が点の場合
                return Math.Sqrt(Math.Pow(point.X - lineStart.X, 2) + Math.Pow(point.Y - lineStart.Y, 2));
            }

            double t = Math.Max(0, Math.Min(1, ((point.X - lineStart.X) * dx + (point.Y - lineStart.Y) * dy) / (dx * dx + dy * dy)));
            
            double projX = lineStart.X + t * dx;
            double projY = lineStart.Y + t * dy;
            
            return Math.Sqrt(Math.Pow(point.X - projX, 2) + Math.Pow(point.Y - projY, 2));
        }

        /// <summary>
        /// マウスクリック時の処理
        /// </summary>
        public void HandleMouseDown(ScreenPoint point)
        {
            var lineIndex = GetNearestLineIndex(point);
            if (lineIndex >= 0)
            {
                // 選択状態を設定（同じ線をクリックした場合は選択解除）
                if (SelectedLineIndex == lineIndex)
                {
                    SelectedLineIndex = -1;
                    FixedTooltipInfo = null;
                }
                else
                {
                    SelectedLineIndex = lineIndex;
                    // 固定ツールチップを設定
                    FixedTooltipInfo = new FixedTooltip
                    {
                        Position = point,
                        LineIndex = lineIndex,
                        Text = CreateTooltipText(lineIndex)
                    };
                }

                PlotModel?.InvalidatePlot(false);
            }
            else
            {
                // クリックした場所に線がない場合は選択と固定ツールチップをクリア
                if (SelectedLineIndex >= 0 || FixedTooltipInfo != null)
                {
                    SelectedLineIndex = -1;
                    FixedTooltipInfo = null;
                    PlotModel?.InvalidatePlot(false);
                }
            }
        }

        /// <summary>
        /// 指定した点に最も近い線のインデックスを取得します
        /// </summary>
        private int GetNearestLineIndex(ScreenPoint point)
        {
            if (Dimensions == null || Dimensions.Count == 0)
                return -1;

            double plotAreaMargin = 30.0;
            double availableHeight = PlotModel.PlotArea.Height - (2 * plotAreaMargin);
            double plotBottom = PlotModel.PlotArea.Bottom - plotAreaMargin;

            int valueCount = Dimensions[0].Values.Length;
            double minDistance = double.MaxValue;
            int nearestLineIndex = -1;

            // 各線について距離をチェック
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
                    
                    double y = plotBottom - normalizedValue * availableHeight;
                    double x = XAxis.Transform(dimIndex);

                    screenPoints.Add(new ScreenPoint(x, y));
                }

                // 線分との距離を計算
                for (int i = 0; i < screenPoints.Count - 1; i++)
                {
                    double distance = DistanceToLineSegment(point, screenPoints[i], screenPoints[i + 1]);
                    if (distance < minDistance && distance < 20) // 20ピクセル以内（感度向上）
                    {
                        minDistance = distance;
                        nearestLineIndex = lineIndex;
                    }
                }
            }

            return nearestLineIndex;
        }

        /// <summary>
        /// ハイライトと選択状態をリセットします
        /// </summary>
        public void ResetHighlightAndSelection()
        {
            bool needsRedraw = false;

            // ハイライトをクリア
            if (HighlightedLineIndex >= 0)
            {
                HighlightedLineIndex = -1;
                needsRedraw = true;
            }

            // 選択をクリア
            if (SelectedLineIndex >= 0)
            {
                SelectedLineIndex = -1;
                needsRedraw = true;
            }

            // 固定ツールチップもクリア
            if (FixedTooltipInfo != null)
            {
                FixedTooltipInfo = null;
                needsRedraw = true;
            }

            // 再描画
            if (needsRedraw)
            {
                PlotModel?.InvalidatePlot(false);
            }
        }

        /// <summary>
        /// 指定した線のインデックスに対してツールチップテキストを作成します
        /// </summary>
        private string CreateTooltipText(int lineIndex)
        {
            if (Dimensions == null || Dimensions.Count == 0 || lineIndex < 0)
                return $"Line {lineIndex}";

            var tooltipLines = new List<string>();
            tooltipLines.Add($"Data Point {lineIndex}");
            tooltipLines.Add(""); // 空行

            // 各次元の値を表示
            for (int dimIndex = 0; dimIndex < Dimensions.Count; dimIndex++)
            {
                var dimension = Dimensions[dimIndex];
                if (lineIndex < dimension.Values.Length)
                {
                    double value = dimension.Values[lineIndex];
                    tooltipLines.Add($"{dimension.Label}: {value:F2}");
                }
            }

            // カラー値がある場合は表示
            if (ColorValues != null && lineIndex < ColorValues.Length)
            {
                tooltipLines.Add(""); // 空行
                tooltipLines.Add($"Color Value: {ColorValues[lineIndex]:F2}");
            }

            return string.Join("\n", tooltipLines);
        }

        /// <summary>
        /// 固定ツールチップを描画します
        /// </summary>
        private void RenderFixedTooltip(IRenderContext rc)
        {
            if (FixedTooltipInfo == null)
                return;

            var lines = FixedTooltipInfo.Text.Split('\n');
            double lineHeight = 16;
            double padding = 8;
            double fontSize = 11;

            // ツールチップのサイズを計算
            double maxWidth = 0;
            foreach (var line in lines)
            {
                var size = rc.MeasureText(line, "Arial", fontSize, OxyPlot.FontWeights.Normal);
                maxWidth = Math.Max(maxWidth, size.Width);
            }

            double tooltipWidth = maxWidth + 2 * padding;
            double tooltipHeight = lines.Length * lineHeight + 2 * padding;

            // ツールチップの位置を調整（軸と重ならないように）
            double clickX = FixedTooltipInfo.Position.X;
            double clickY = FixedTooltipInfo.Position.Y;
            
            // 軸の位置を避けるため、軸間の中央付近に配置を試みる
            double bestX = clickX + 15;
            double bestY = clickY - tooltipHeight - 15;
            
            // 各軸の位置をチェックして、軸から離れた位置を選択
            double minDistanceFromAxis = double.MaxValue;
            for (int dimIndex = 0; dimIndex < Dimensions.Count; dimIndex++)
            {
                double axisX = XAxis.Transform(dimIndex);
                double distance = Math.Abs(bestX + tooltipWidth / 2 - axisX);
                
                // 軸に近すぎる場合は位置を調整
                if (distance < tooltipWidth / 2 + 10)
                {
                    if (dimIndex < Dimensions.Count - 1)
                    {
                        // 次の軸との中間点に移動
                        double nextAxisX = XAxis.Transform(dimIndex + 1);
                        bestX = (axisX + nextAxisX - tooltipWidth) / 2;
                    }
                    else if (dimIndex > 0)
                    {
                        // 前の軸との中間点に移動
                        double prevAxisX = XAxis.Transform(dimIndex - 1);
                        bestX = (prevAxisX + axisX - tooltipWidth) / 2;
                    }
                }
                
                minDistanceFromAxis = Math.Min(minDistanceFromAxis, distance);
            }
            
            // 画面外に出ないように最終調整
            if (bestX + tooltipWidth > PlotModel.PlotArea.Right)
                bestX = PlotModel.PlotArea.Right - tooltipWidth - 5;
            if (bestX < PlotModel.PlotArea.Left)
                bestX = PlotModel.PlotArea.Left + 5;
                
            if (bestY < PlotModel.PlotArea.Top)
                bestY = clickY + 20;
            if (bestY + tooltipHeight > PlotModel.PlotArea.Bottom)
                bestY = PlotModel.PlotArea.Bottom - tooltipHeight - 5;
            
            double x = bestX;
            double y = bestY;

            // 背景を描画
            var backgroundRect = new OxyRect(x, y, tooltipWidth, tooltipHeight);
            rc.DrawRectangle(backgroundRect, OxyColor.FromArgb(240, 255, 255, 255), OxyColors.Gray, 1, EdgeRenderingMode.Automatic);

            // テキストを描画
            for (int i = 0; i < lines.Length; i++)
            {
                var textPosition = new ScreenPoint(x + padding, y + padding + i * lineHeight);
                rc.DrawText(textPosition, lines[i], OxyColors.Black, "Arial", fontSize, 
                    OxyPlot.FontWeights.Normal, 0, OxyPlot.HorizontalAlignment.Left, 
                    OxyPlot.VerticalAlignment.Top);
            }
        }
    }

    /// <summary>
    /// 固定ツールチップの情報
    /// </summary>
    public class FixedTooltip
    {
        /// <summary>
        /// ツールチップの表示位置
        /// </summary>
        public ScreenPoint Position { get; set; }

        /// <summary>
        /// 対象の線のインデックス
        /// </summary>
        public int LineIndex { get; set; }

        /// <summary>
        /// 表示するテキスト
        /// </summary>
        public string Text { get; set; } = string.Empty;
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