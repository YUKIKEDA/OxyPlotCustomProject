using System;
using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Series;

namespace OxyPlotCustomProject
{
    /// <summary>
    /// クリック可能な散布図シリーズ
    /// グラフ内の任意の点をクリックすると、その場所に新しい点をプロットできます
    /// クリックされた点は別の色で表示されます
    /// </summary>
    public class ClickableScatterSeries : ScatterSeries
    {
        /// <summary>
        /// 点をクリックしたときに発生するイベント
        /// </summary>
        public event EventHandler<PointClickedEventArgs>? PointClicked;

        /// <summary>
        /// 新しい点を追加したときに発生するイベント
        /// </summary>
        public event EventHandler<PointAddedEventArgs>? PointAdded;

        /// <summary>
        /// クリック可能かどうかを示すフラグ
        /// </summary>
        public bool IsClickable { get; set; } = true;

        /// <summary>
        /// 新しい点のマーカーサイズ
        /// </summary>
        public double NewPointMarkerSize { get; set; } = 6.0;

        /// <summary>
        /// 新しい点のマーカー色
        /// </summary>
        public OxyColor NewPointMarkerColor { get; set; } = OxyColors.Red;

        /// <summary>
        /// 新しい点のマーカータイプ
        /// </summary>
        public MarkerType NewPointMarkerType { get; set; } = MarkerType.Circle;

        /// <summary>
        /// クリックされた点のマーカー色
        /// </summary>
        public OxyColor ClickedPointMarkerColor { get; set; } = OxyColors.Gold;

        /// <summary>
        /// クリックされた点のマーカーサイズ
        /// </summary>
        public double ClickedPointMarkerSize { get; set; } = 10.0;

        /// <summary>
        /// クリックされた点のインデックス
        /// </summary>
        private int _clickedPointIndex = -1;

        /// <summary>
        /// 各点の色情報を保持する辞書
        /// </summary>
        private Dictionary<int, OxyColor> _pointColors = new Dictionary<int, OxyColor>();

        /// <summary>
        /// 新しい <see cref="ClickableScatterSeries"/> のインスタンスを初期化します
        /// </summary>
        public ClickableScatterSeries()
        {
            this.MarkerType = MarkerType.Circle;
            this.MarkerSize = 8.0;
            this.MarkerFill = OxyColors.Blue;
            this.MarkerStroke = OxyColors.Black;
            this.MarkerStrokeThickness = 1.0;
        }

        /// <summary>
        /// 点を追加する際に色情報も初期化します
        /// </summary>
        /// <param name="point">追加する点</param>
        public void AddPoint(ScatterPoint point)
        {
            this.Points.Add(point);
            var index = this.Points.Count - 1;
            _pointColors[index] = this.MarkerFill;
        }

        /// <summary>
        /// 初期化時にすべての点の色を設定します
        /// </summary>
        public void InitializePointColors()
        {
            _pointColors.Clear();
            for (int i = 0; i < this.Points.Count; i++)
            {
                _pointColors[i] = this.MarkerFill;
            }
        }

        /// <summary>
        /// 指定されたインデックスの点の元の色を取得します
        /// </summary>
        /// <param name="index">点のインデックス</param>
        /// <returns>元の色</returns>
        private OxyColor GetOriginalPointColor(int index)
        {
            // 初期点（最初の4つ）は青色、それ以降は赤色
            return index < 4 ? this.MarkerFill : this.NewPointMarkerColor;
        }

        /// <summary>
        /// 指定されたスクリーン座標でクリックイベントを処理します
        /// </summary>
        /// <param name="screenPoint">クリックされたスクリーン座標</param>
        /// <returns>クリックが処理された場合はtrue、そうでなければfalse</returns>
        public bool HandleClick(ScreenPoint screenPoint)
        {
            if (!this.IsClickable)
                return false;

            // プロットエリア内かどうかをチェック
            if (!IsPointInPlotArea(screenPoint))
                return false;

            // スクリーン座標をデータ座標に変換
            var dataPoint = this.InverseTransform(screenPoint);
            
            // データ座標が有効かチェック
            if (double.IsNaN(dataPoint.X) || double.IsNaN(dataPoint.Y))
                return false;

            // 既存の点がクリックされたかチェック
            var clickedPoint = FindNearestPoint(screenPoint, 10.0); // 10ピクセル以内
            if (clickedPoint != null)
            {
                // クリックされた点のインデックスを取得
                var clickedIndex = this.Points.IndexOf(clickedPoint);
                if (clickedIndex >= 0)
                {
                    // 同じ点をクリックした場合は何もしない
                    if (clickedIndex == _clickedPointIndex)
                    {
                        OnPointClicked(new PointClickedEventArgs(clickedPoint));
                        return true;
                    }
                    
                    // 前回クリックされた点の色をリセット（元の色に戻す）
                    if (_clickedPointIndex >= 0 && _clickedPointIndex < this.Points.Count)
                    {
                        // 元の色を取得（初期点は青、追加点は赤）
                        var originalColor = GetOriginalPointColor(_clickedPointIndex);
                        _pointColors[_clickedPointIndex] = originalColor;
                    }
                    
                    // 新しいクリックされた点の色を保存
                    if (!_pointColors.ContainsKey(clickedIndex))
                    {
                        var originalColor = GetOriginalPointColor(clickedIndex);
                        _pointColors[clickedIndex] = originalColor;
                    }
                    
                    // クリックされた点の色を変更
                    _pointColors[clickedIndex] = this.ClickedPointMarkerColor;
                    
                    // 新しいクリックされた点のインデックスを保存
                    _clickedPointIndex = clickedIndex;
                    
                    // プロットを更新して色変更を反映
                    this.PlotModel?.InvalidatePlot(true);
                }
                
                // 既存の点がクリックされた場合
                OnPointClicked(new PointClickedEventArgs(clickedPoint));
                return true;
            }
            else
            {
                // 新しい点を追加
                var newPoint = new ScatterPoint(dataPoint.X, dataPoint.Y, this.NewPointMarkerSize);
                this.Points.Add(newPoint);
                
                // 新しい点の色を初期化（新しく追加された点は赤色）
                var newIndex = this.Points.Count - 1;
                _pointColors[newIndex] = this.NewPointMarkerColor;
                
                // イベントを発生
                OnPointAdded(new PointAddedEventArgs(newPoint));
                
                // プロットを更新
                this.PlotModel?.InvalidatePlot(true);
                
                return true;
            }
        }

        /// <summary>
        /// 指定されたスクリーン座標がプロットエリア内かどうかを判定します
        /// </summary>
        /// <param name="screenPoint">スクリーン座標</param>
        /// <returns>プロットエリア内の場合はtrue</returns>
        private bool IsPointInPlotArea(ScreenPoint screenPoint)
        {
            if (this.PlotModel == null)
                return false;

            // プロットエリアの境界を取得
            var plotArea = this.PlotModel.PlotArea;
            
            // プロットエリア内かどうかをチェック
            return screenPoint.X >= plotArea.Left &&
                   screenPoint.X <= plotArea.Right &&
                   screenPoint.Y >= plotArea.Top &&
                   screenPoint.Y <= plotArea.Bottom;
        }

        /// <summary>
        /// 指定されたスクリーン座標に最も近い点を検索します
        /// </summary>
        /// <param name="screenPoint">スクリーン座標</param>
        /// <param name="tolerance">許容距離（ピクセル）</param>
        /// <returns>最も近い点、見つからない場合はnull</returns>
        private ScatterPoint? FindNearestPoint(ScreenPoint screenPoint, double tolerance)
        {
            ScatterPoint? nearestPoint = null;
            double minDistance = double.MaxValue;

            foreach (var point in this.Points)
            {
                var pointScreenPos = this.Transform(point.X, point.Y);
                var distance = screenPoint.DistanceTo(pointScreenPos);
                
                if (distance <= tolerance && distance < minDistance)
                {
                    minDistance = distance;
                    nearestPoint = point;
                }
            }

            return nearestPoint;
        }

        /// <summary>
        /// PointClickedイベントを発生させます
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected virtual void OnPointClicked(PointClickedEventArgs e)
        {
            PointClicked?.Invoke(this, e);
        }

        /// <summary>
        /// PointAddedイベントを発生させます
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected virtual void OnPointAdded(PointAddedEventArgs e)
        {
            PointAdded?.Invoke(this, e);
        }

        /// <summary>
        /// 指定されたインデックスの点を削除します
        /// </summary>
        /// <param name="index">削除する点のインデックス</param>
        public void RemovePointAt(int index)
        {
            if (index >= 0 && index < this.Points.Count)
            {
                this.Points.RemoveAt(index);
                this.PlotModel?.InvalidatePlot(true);
            }
        }

        /// <summary>
        /// すべての点をクリアします
        /// </summary>
        public void ClearPoints()
        {
            this.Points.Clear();
            _pointColors.Clear();
            _clickedPointIndex = -1;
            this.PlotModel?.InvalidatePlot(true);
        }

        /// <summary>
        /// シリーズを描画します（各点の色を個別に設定）
        /// </summary>
        /// <param name="rc">描画コンテキスト</param>
        public override void Render(IRenderContext rc)
        {
            if (this.PlotModel == null)
                return;

            var actualPoints = this.ActualPointsList;
            if (actualPoints == null || actualPoints.Count == 0)
                return;

            // 各点を個別の色で描画
            for (int i = 0; i < actualPoints.Count; i++)
            {
                var point = actualPoints[i];
                if (point == null || double.IsNaN(point.X) || double.IsNaN(point.Y))
                    continue;

                var screenPoint = this.Transform(point.X, point.Y);
                
                // 点の色を決定
                var pointColor = _pointColors.ContainsKey(i) ? _pointColors[i] : GetOriginalPointColor(i);
                var pointSize = this.MarkerSize;
                
                if (i == _clickedPointIndex)
                {
                    pointSize = this.ClickedPointMarkerSize;
                }

                // マーカーを描画
                rc.DrawMarker(
                    screenPoint,
                    this.MarkerType,
                    this.MarkerOutline,
                    pointSize,
                    pointColor,
                    this.MarkerStroke,
                    this.MarkerStrokeThickness,
                    this.EdgeRenderingMode);
            }
        }
    }

    /// <summary>
    /// 点がクリックされたときのイベント引数
    /// </summary>
    public class PointClickedEventArgs : EventArgs
    {
        /// <summary>
        /// クリックされた点
        /// </summary>
        public ScatterPoint Point { get; }

        /// <summary>
        /// 新しい <see cref="PointClickedEventArgs"/> のインスタンスを初期化します
        /// </summary>
        /// <param name="point">クリックされた点</param>
        public PointClickedEventArgs(ScatterPoint point)
        {
            this.Point = point;
        }
    }

    /// <summary>
    /// 新しい点が追加されたときのイベント引数
    /// </summary>
    public class PointAddedEventArgs : EventArgs
    {
        /// <summary>
        /// 追加された点
        /// </summary>
        public ScatterPoint Point { get; }

        /// <summary>
        /// 新しい <see cref="PointAddedEventArgs"/> のインスタンスを初期化します
        /// </summary>
        /// <param name="point">追加された点</param>
        public PointAddedEventArgs(ScatterPoint point)
        {
            this.Point = point;
        }
    }
}
