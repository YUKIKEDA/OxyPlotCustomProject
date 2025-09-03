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
    public class ParallelCoordinatesSeries : ItemsSeries
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

        /// <summary>
        /// プロットエリアの余白（ピクセル）
        /// </summary>
        public double PlotAreaMargin { get; set; }

        /// <summary>
        /// 水平方向の余白（ピクセル）
        /// </summary>
        public double HorizontalMargin { get; set; }

        /// <summary>
        /// マウス感度の閾値（ピクセル）
        /// </summary>
        public double MouseSensitivity { get; set; }

        /// <summary>
        /// 軸タイトルのフォントサイズ
        /// </summary>
        public double AxisTitleFontSize { get; set; }

        /// <summary>
        /// 軸ラベルのフォントサイズ
        /// </summary>
        public double AxisLabelFontSize { get; set; }

        /// <summary>
        /// ツールチップのフォントサイズ
        /// </summary>
        public double TooltipFontSize { get; set; }

        /// <summary>
        /// ツールチップの行の高さ（ピクセル）
        /// </summary>
        public double TooltipLineHeight { get; set; }

        /// <summary>
        /// ツールチップのパディング（ピクセル）
        /// </summary>
        public double TooltipPadding { get; set; }

        /// <summary>
        /// 目盛り線の長さ（ピクセル）
        /// </summary>
        public double TickLength { get; set; }

        /// <summary>
        /// ツールチップの位置オフセット（ピクセル）
        /// </summary>
        public double TooltipOffset { get; set; }

        /// <summary>
        /// 軸タイトルの縦オフセット（ピクセル）
        /// </summary>
        public double AxisTitleVerticalOffset { get; set; }

        /// <summary>
        /// 軸ラベルの縦オフセット（ピクセル）
        /// </summary>
        public double AxisLabelVerticalOffset { get; set; }

        /// <summary>
        /// 目盛りラベルの水平オフセット（ピクセル）
        /// </summary>
        public double TickLabelHorizontalOffset { get; set; }

        /// <summary>
        /// フォントファミリー
        /// </summary>
        public string FontFamily { get; set; }

        /// <summary>
        /// 軸の色
        /// </summary>
        public OxyColor AxisColor { get; set; }

        /// <summary>
        /// 軸の太さ
        /// </summary>
        public double AxisThickness { get; set; }

        /// <summary>
        /// ParallelCoordinatesSeriesの新しいインスタンスを初期化します
        /// </summary>
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

            // レイアウト関連のデフォルト値
            PlotAreaMargin = 30.0;
            HorizontalMargin = 40.0;
            MouseSensitivity = 20.0;
            AxisTitleFontSize = 12.0;
            AxisLabelFontSize = 10.0;
            TooltipFontSize = 11.0;
            TooltipLineHeight = 16.0;
            TooltipPadding = 8.0;
            TickLength = 5.0;
            TooltipOffset = 15.0;
            AxisTitleVerticalOffset = 10.0;
            AxisLabelVerticalOffset = 20.0;
            TickLabelHorizontalOffset = 10.0;
            FontFamily = "Arial";
            
            // 軸のデフォルト値
            AxisColor = OxyColors.Black;
            AxisThickness = 1.0;
        }

        /// <summary>
        /// シリーズをレンダリングします
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        public override void Render(IRenderContext rc)
        {
            if (Dimensions == null || Dimensions.Count == 0)
            {
                return;
            }

            // 各軸の描画
            RenderAxes(rc);

            // データ線の描画
            RenderDataLines(rc);

            // 固定ツールチップの描画
            RenderFixedTooltip(rc);
        }

        /// <summary>
        /// データを更新します
        /// </summary>
        protected override void UpdateData()
        {
            // データの更新処理
        }

        /// <summary>
        /// 指定した点に最も近いデータポイントを取得します
        /// このメソッドはハイライト機能のみを提供し、トラッカー機能は無効化されています
        /// </summary>
        /// <param name="point">スクリーン座標の点</param>
        /// <param name="interpolate">補間を行うかどうか（使用されません）</param>
        /// <returns>常にnullを返します（トラッカー機能無効化のため）</returns>
        public override TrackerHitResult? GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            // ハイライト処理を実行
            UpdateHighlightState(point);

            // トラッカー機能は無効化するためnullを返す
            return null;
        }

        /// <summary>
        /// 軸が必要かどうかを判定します
        /// </summary>
        /// <returns>常にfalse（独自の軸描画を使用するため）</returns>
        protected override bool AreAxesRequired() => false;
        
        /// <summary>
        /// 軸を確保します（使用されません）
        /// </summary>
        protected override void EnsureAxes() { }
        
        /// <summary>
        /// 指定した軸を使用しているかどうかを判定します
        /// </summary>
        /// <param name="axis">確認する軸</param>
        /// <returns>常にfalse（独自の軸描画を使用するため）</returns>
        protected override bool IsUsing(OxyPlot.Axes.Axis axis) => false;
        
        /// <summary>
        /// 軸の最大値・最小値を更新します（使用されません）
        /// </summary>
        protected override void UpdateAxisMaxMin() { }
        
        /// <summary>
        /// 最大値・最小値を更新します（使用されません）
        /// </summary>
        protected override void UpdateMaxMin() { }
        
        /// <summary>
        /// デフォルト値を設定します（使用されません）
        /// </summary>
        protected override void SetDefaultValues() { }

        /// <summary>
        /// 凡例シンボルを描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        /// <param name="legendBox">凡例の矩形</param>
        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
        }

        /// <summary>
        /// 軸を描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        private void RenderAxes(IRenderContext rc)
        {
            for (int i = 0; i < Dimensions.Count; i++)
            {
                var dimension = Dimensions[i];
                // 水平方向の利用可能な幅を計算（左右の余白を除く）
                double availableWidth = PlotModel.PlotArea.Width - (2 * HorizontalMargin);
                // 各軸のX座標を計算（軸間隔を等分に配置）
                double x = PlotModel.PlotArea.Left + HorizontalMargin + (availableWidth * i / (Dimensions.Count - 1));

                // 軸の垂直線を描画（余白を考慮）
                var topPoint = new ScreenPoint(x, PlotModel.PlotArea.Top + PlotAreaMargin);
                var bottomPoint = new ScreenPoint(x, PlotModel.PlotArea.Bottom - PlotAreaMargin);

                rc.DrawLine(new[] { topPoint, bottomPoint }, AxisColor, AxisThickness, EdgeRenderingMode.Automatic);

                // 軸のラベルを描画
                RenderAxisLabels(rc, i, dimension, x);

                // 軸の目盛りを描画
                RenderAxisTicks(rc, i, dimension);
            }
        }

        /// <summary>
        /// 軸のラベルを描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        /// <param name="axisIndex">軸のインデックス</param>
        /// <param name="dimension">次元情報</param>
        /// <param name="x">軸のX座標</param>
        private void RenderAxisLabels(IRenderContext rc, int axisIndex, ParallelDimension dimension, double x)
        {
            // 軸のタイトルを上部に描画
            if (ShowAxisLabelsTop)
            {
                var titleTopPoint = new ScreenPoint(x, PlotModel.PlotArea.Top + PlotAreaMargin - AxisTitleVerticalOffset);
                // TODO 色やアライメントを設定できるようにする
                rc.DrawText(
                    titleTopPoint, 
                    dimension.Label, 
                    OxyColors.Black, 
                    fontFamily: FontFamily, 
                    fontSize: AxisTitleFontSize, 
                    fontWeight: OxyPlot.FontWeights.Bold, 
                    rotation: 0, 
                    horizontalAlignment: OxyPlot.HorizontalAlignment.Center, 
                    verticalAlignment: OxyPlot.VerticalAlignment.Bottom
                );
            }

            // 軸のタイトルを下部に描画（オプション）
            if (ShowAxisLabelsBottom)
            {
                var titleBottomPoint = new ScreenPoint(x, PlotModel.PlotArea.Bottom - PlotAreaMargin + AxisLabelVerticalOffset);
                // TODO 色やアライメントを設定できるようにする
                rc.DrawText(
                    titleBottomPoint, 
                    dimension.Label, 
                    OxyColors.Gray, 
                    fontFamily: FontFamily, 
                    fontSize: AxisLabelFontSize, 
                    fontWeight: OxyPlot.FontWeights.Normal, 
                    rotation: 0, 
                    horizontalAlignment: OxyPlot.HorizontalAlignment.Center, 
                    verticalAlignment: OxyPlot.VerticalAlignment.Top);
            }
        }

        /// <summary>
        /// 軸の目盛りを描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        /// <param name="axisIndex">軸のインデックス</param>
        /// <param name="dimension">次元情報</param>
        private void RenderAxisTicks(IRenderContext rc, int axisIndex, ParallelDimension dimension)
        {
            // 水平方向の利用可能な幅を計算（左右の余白を除く）
            double availableWidth = PlotModel.PlotArea.Width - (2 * HorizontalMargin);
            // 指定された軸のX座標を計算（軸間隔を等分に配置）
            double x = PlotModel.PlotArea.Left + HorizontalMargin + (availableWidth * axisIndex / (Dimensions.Count - 1));
            
            // 垂直方向の利用可能な高さを計算（上下の余白を除く）
            double availableHeight = PlotModel.PlotArea.Height - (2 * PlotAreaMargin);
            // プロット領域の上下端の座標を計算
            double plotTop = PlotModel.PlotArea.Top + PlotAreaMargin;
            double plotBottom = PlotModel.PlotArea.Bottom - PlotAreaMargin;
            
            for (int t = 0; t <= AxisTickCount; t++)
            {
                // 目盛りの値を計算（最小値から最大値まで等間隔で分割）
                double value = dimension.Range[0] + (dimension.Range[1] - dimension.Range[0]) * t / AxisTickCount;
                // 値を0-1の範囲に正規化（最小値=0, 最大値=1）
                double normalizedValue = (value - dimension.Range[0]) / (dimension.Range[1] - dimension.Range[0]);
                // 正規化された値をY座標に変換（下から上に向かって配置）
                double y = plotBottom - normalizedValue * availableHeight;

                // 目盛り線
                rc.DrawLine(
                    new[] 
                    { 
                        new ScreenPoint(x - TickLength, y), 
                        new ScreenPoint(x + TickLength, y) 
                    }, 
                    OxyColors.Black, 
                    1.0, 
                    EdgeRenderingMode.Automatic
                );

                // TODO 色やアライメントを設定できるようにする
                // 目盛りラベル
                rc.DrawText(
                    new ScreenPoint(x + TickLabelHorizontalOffset, y), 
                    value.ToString("F1"), 
                    OxyColors.Black, 
                    fontFamily: FontFamily, 
                    fontSize: AxisLabelFontSize, 
                    fontWeight: OxyPlot.FontWeights.Normal, 
                    rotation: 0, 
                    horizontalAlignment: OxyPlot.HorizontalAlignment.Left, 
                    verticalAlignment: OxyPlot.VerticalAlignment.Middle
                );
            }
        }

        /// <summary>
        /// データ線を描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        private void RenderDataLines(IRenderContext rc)
        {
            // 上下に余白を設ける
            double availableHeight = PlotModel.PlotArea.Height - (2 * PlotAreaMargin);
            double plotBottom = PlotModel.PlotArea.Bottom - PlotAreaMargin;

            if (Dimensions.Count == 0)
            {
                return;
            }

            // TODO 各次元のデータ数が異なる場合のハンドリングを考える
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
                    {
                        break;
                    }

                    double value = dimension.Values[lineIndex];
                    double normalizedValue = (value - dimension.Range[0]) / (dimension.Range[1] - dimension.Range[0]);
                    
                    // Y座標を画面座標に変換（余白を考慮）
                    double y = plotBottom - normalizedValue * availableHeight;
                    double availableWidth = PlotModel.PlotArea.Width - (2 * HorizontalMargin);
                    double x = PlotModel.PlotArea.Left + HorizontalMargin + (availableWidth * dimIndex / (Dimensions.Count - 1));

                    screenPoints.Add(new ScreenPoint(x, y));
                }

                // この線の色と太さを決定
                var (lineColor, thickness) = GetLineVisualProperties(lineIndex);

                // 線分として描画
                if (screenPoints.Count > 1)
                {
                    for (int i = 0; i < screenPoints.Count - 1; i++)
                    {
                        rc.DrawLine(
                            new[] 
                            { 
                                screenPoints[i], 
                                screenPoints[i + 1] 
                            }, 
                            lineColor, 
                            thickness, 
                            EdgeRenderingMode.Automatic
                        );
                    }
                }
            }
        }

        /// <summary>
        /// 指定したライン番号の視覚的プロパティ（色、太さ）を取得します
        /// </summary>
        /// <param name="lineIndex">ラインのインデックス</param>
        /// <returns>色と太さのタプル</returns>
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
        /// <param name="lineIndex">ラインのインデックス</param>
        /// <returns>基本色</returns>
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
        /// <param name="value">補間する値</param>
        /// <returns>補間された色</returns>
        private OxyColor InterpolateBaseColor(double value)
        {
            if (ColorScale == null || ColorScale.Count == 0)
            {
                return LineColor;
            }

            // 値がカラースケールの範囲外の場合
            if (value <= ColorScale[0].Value)
            {
                return ColorScale[0].Color;
            }
            if (value >= ColorScale[ColorScale.Count - 1].Value)
            {
                return ColorScale[ColorScale.Count - 1].Color;
            }

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
        /// 固定ツールチップを描画します
        /// </summary>
        /// <param name="rc">レンダリングコンテキスト</param>
        private void RenderFixedTooltip(IRenderContext rc)
        {
            if (FixedTooltipInfo == null)
            {
                return;
            }

            var lines = FixedTooltipInfo.Text.Split('\n');

            // ツールチップのサイズを計算
            double maxWidth = 0;
            foreach (var line in lines)
            {
                var size = rc.MeasureText(line, FontFamily, TooltipFontSize, OxyPlot.FontWeights.Normal);
                maxWidth = Math.Max(maxWidth, size.Width);
            }

            double tooltipWidth = maxWidth + 2 * TooltipPadding;
            double tooltipHeight = lines.Length * TooltipLineHeight + 2 * TooltipPadding;

            // ツールチップの位置を計算
            var position = CalculateTooltipPosition(FixedTooltipInfo.Position, tooltipWidth, tooltipHeight);
            double x = position.X;
            double y = position.Y;

            // TODO 背景色や線の太さ、色を設定できるようにする
            // 背景を描画
            var backgroundRect = new OxyRect(x, y, tooltipWidth, tooltipHeight);
            rc.DrawRectangle(
                backgroundRect, 
                OxyColor.FromArgb(240, 255, 255, 255), 
                OxyColors.Gray, 
                1, 
                EdgeRenderingMode.Automatic
            );

            // テキストを描画
            for (int i = 0; i < lines.Length; i++)
            {
                // TODO 色やアライメントを設定できるようにする
                var textPosition = new ScreenPoint(x + TooltipPadding, y + TooltipPadding + i * TooltipLineHeight);
                rc.DrawText(
                    textPosition, 
                    lines[i], 
                    OxyColors.Black, 
                    FontFamily, 
                    TooltipFontSize, 
                    OxyPlot.FontWeights.Normal, 
                    0, 
                    OxyPlot.HorizontalAlignment.Left, 
                    OxyPlot.VerticalAlignment.Top
                );
            }
        }

        /// <summary>
        /// ツールチップの最適な位置を計算します
        /// </summary>
        /// <param name="clickPosition">クリック位置</param>
        /// <param name="tooltipWidth">ツールチップの幅</param>
        /// <param name="tooltipHeight">ツールチップの高さ</param>
        /// <returns>ツールチップの位置</returns>
        private ScreenPoint CalculateTooltipPosition(ScreenPoint clickPosition, double tooltipWidth, double tooltipHeight)
        {
            double clickX = clickPosition.X;
            double clickY = clickPosition.Y;

            // 軸の位置を避けるため、軸間の中央付近に配置を試みる
            
            // 初期位置：クリック位置から少しオフセットして配置
            // X座標：右側にオフセット、Y座標：上側にオフセット（ツールチップがクリック位置を隠さないように）
            double bestX = clickX + TooltipOffset;
            double bestY = clickY - tooltipHeight - TooltipOffset;
            
            // 各軸の位置をチェックして、軸から離れた位置を選択
            // 軸とツールチップが重ならないようにするため
            double minDistanceFromAxis = double.MaxValue;
            for (int dimIndex = 0; dimIndex < Dimensions.Count; dimIndex++)
            {
                // プロットエリア内で軸が配置される幅を計算
                // 左右のマージンを除いた利用可能な幅
                double availableWidth = PlotModel.PlotArea.Width - (2 * HorizontalMargin);
                
                // 現在の軸のX座標を計算
                // 軸は等間隔で配置される（0番目から最後まで均等に分散）
                double axisX = PlotModel.PlotArea.Left + HorizontalMargin + (availableWidth * dimIndex / (Dimensions.Count - 1));
                
                // ツールチップの中心と軸の距離を計算
                // ツールチップの中心位置 = bestX + tooltipWidth / 2
                double distance = Math.Abs(bestX + tooltipWidth / 2 - axisX);
                
                // 軸に近すぎる場合（ツールチップが軸と重なる可能性がある場合）は位置を調整
                // 判定条件：距離 < ツールチップの半分の幅 + 軸ラベルのオフセット
                if (distance < tooltipWidth / 2 + TickLabelHorizontalOffset)
                {
                    if (dimIndex < Dimensions.Count - 1)
                    {
                        // 次の軸との中間点に移動
                        // 現在の軸と次の軸の間の中央にツールチップを配置
                        double nextAxisX = PlotModel.PlotArea.Left + HorizontalMargin + (availableWidth * (dimIndex + 1) / (Dimensions.Count - 1));
                        bestX = (axisX + nextAxisX - tooltipWidth) / 2;
                    }
                    else if (dimIndex > 0)
                    {
                        // 前の軸との中間点に移動
                        // 最後の軸の場合は、前の軸との間の中央に配置
                        double prevAxisX = PlotModel.PlotArea.Left + HorizontalMargin + (availableWidth * (dimIndex - 1) / (Dimensions.Count - 1));
                        bestX = (prevAxisX + axisX - tooltipWidth) / 2;
                    }
                }
                
                // 最小距離を更新（デバッグ用）
                minDistanceFromAxis = Math.Min(minDistanceFromAxis, distance);
            }
            
            // 画面外に出ないように最終調整
            // 右端を超える場合：右端からツールチップ幅とティック長を引いた位置に調整
            if (bestX + tooltipWidth > PlotModel.PlotArea.Right)
            {
                bestX = PlotModel.PlotArea.Right - tooltipWidth - TickLength;
            }
            // 左端を超える場合：左端からティック長分オフセットした位置に調整
            if (bestX < PlotModel.PlotArea.Left)
            {
                bestX = PlotModel.PlotArea.Left + TickLength;
            }
            
            // Y座標の調整
            // 上端を超える場合：クリック位置の下側に表示
            if (bestY < PlotModel.PlotArea.Top)
            {
                bestY = clickY + TooltipOffset;
            }
            // 下端を超える場合：下端からツールチップ高さとティック長を引いた位置に調整
            if (bestY + tooltipHeight > PlotModel.PlotArea.Bottom)
            {
                bestY = PlotModel.PlotArea.Bottom - tooltipHeight - TickLength;
            }
            
            return new ScreenPoint(bestX, bestY);
        }

        /// <summary>
        /// 指定した点に基づいてハイライト状態を更新します
        /// </summary>
        /// <param name="point">スクリーン座標の点</param>
        private void UpdateHighlightState(ScreenPoint point)
        {
            int nearestLineIndex = FindNearestLineIndex(point);

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
        }

        /// <summary>
        /// 点から線分までの距離を計算します
        /// </summary>
        /// <param name="point">点の座標</param>
        /// <param name="lineStart">線分の開始点</param>
        /// <param name="lineEnd">線分の終了点</param>
        /// <returns>点から線分までの最短距離</returns>
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
        /// マウスクリック時の処理を行います
        /// </summary>
        /// <param name="point">クリックされたスクリーン座標</param>
        public void HandleMouseDown(ScreenPoint point)
        {
            var lineIndex = FindNearestLineIndex(point);
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
        /// 指定した点に最も近い線のインデックスを検索します
        /// </summary>
        /// <param name="point">スクリーン座標の点</param>
        /// <returns>最も近い線のインデックス（見つからない場合は-1）</returns>
        private int FindNearestLineIndex(ScreenPoint point)
        {
            if (Dimensions == null || Dimensions.Count == 0)
            {
                return -1;
            }

            double availableHeight = PlotModel.PlotArea.Height - (2 * PlotAreaMargin);
            double plotBottom = PlotModel.PlotArea.Bottom - PlotAreaMargin;

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
                    {
                        break;
                    }

                    double value = dimension.Values[lineIndex];
                    double normalizedValue = (value - dimension.Range[0]) / (dimension.Range[1] - dimension.Range[0]);
                    
                    double y = plotBottom - normalizedValue * availableHeight;
                    double availableWidth = PlotModel.PlotArea.Width - (2 * HorizontalMargin);
                    double x = PlotModel.PlotArea.Left + HorizontalMargin + (availableWidth * dimIndex / (Dimensions.Count - 1));

                    screenPoints.Add(new ScreenPoint(x, y));
                }

                // 線分との距離を計算
                for (int i = 0; i < screenPoints.Count - 1; i++)
                {
                    double distance = DistanceToLineSegment(point, screenPoints[i], screenPoints[i + 1]);
                    if (distance < minDistance && distance < MouseSensitivity)
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
        /// <param name="lineIndex">線のインデックス</param>
        /// <returns>ツールチップテキスト</returns>
        private string CreateTooltipText(int lineIndex)
        {
            if (Dimensions == null || Dimensions.Count == 0 || lineIndex < 0)
            {
                return $"Line {lineIndex}";
            }

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

        /// <summary>
        /// ParallelDimensionの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="label">軸のラベル</param>
        /// <param name="values">値の配列</param>
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

        /// <summary>
        /// ParallelDimensionの新しいインスタンスを初期化します（範囲指定版）
        /// </summary>
        /// <param name="label">軸のラベル</param>
        /// <param name="values">値の配列</param>
        /// <param name="range">値の範囲 [最小値, 最大値]</param>
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

        /// <summary>
        /// ColorScalePointの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="value">カラースケールの値</param>
        /// <param name="color">その値での色</param>
        public ColorScalePoint(double value, OxyColor color)
        {
            Value = value;
            Color = color;
        }
    }
}