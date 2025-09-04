using OxyPlot;
using OxyPlot.Series;

namespace OxyPlotCustomProject.ClickableScatterSeries
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
        /// クリック検出の許容距離（ピクセル）
        /// </summary>
        public double ClickTolerance { get; set; } = 10.0;

        /// <summary>
        /// 初期点の数（初期点設定時に自動的に決定される）
        /// </summary>
        private int _initialPointCount = 0;

        /// <summary>
        /// クリックされた点のインデックス
        /// </summary>
        private int _clickedPointIndex = -1;

        /// <summary>
        /// 各点の色情報を保持する辞書
        /// </summary>
        private readonly Dictionary<int, OxyColor> _pointColors = [];

        /// <summary>
        /// 新しい <see cref="ClickableScatterSeries"/> のインスタンスを初期化します
        /// </summary>
        public ClickableScatterSeries()
        {
            MarkerType = MarkerType.Circle;
            MarkerSize = 8.0;
            MarkerFill = OxyColors.Blue;
            MarkerStroke = OxyColors.Black;
            MarkerStrokeThickness = 1.0;
        }

        /// <summary>
        /// 初期点を設定します（この時点で初期点の数が自動的に決定されます）
        /// </summary>
        /// <param name="initialPoints">初期点のコレクション</param>
        public void SetInitialPoints(IEnumerable<ScatterPoint> initialPoints)
        {
            // 既存の点をクリア
            Points.Clear();
            _pointColors.Clear();
            _clickedPointIndex = -1;
            
            // 初期点を追加
            foreach (var point in initialPoints)
            {
                Points.Add(point);
                _pointColors[Points.Count - 1] = MarkerFill;
            }
            
            // 初期点の数を設定
            _initialPointCount = Points.Count;
        }

        /// <summary>
        /// 指定されたインデックスの点の元の色を取得します
        /// </summary>
        /// <param name="index">点のインデックス</param>
        /// <returns>元の色</returns>
        private OxyColor GetOriginalPointColor(int index)
        {
            // 初期点は青色、それ以降は赤色
            return index < _initialPointCount ? MarkerFill : NewPointMarkerColor;
        }

        /// <summary>
        /// 指定されたスクリーン座標でクリックイベントを処理します
        /// </summary>
        /// <param name="screenPoint">クリックされたスクリーン座標</param>
        /// <returns>クリックが処理された場合はtrue、そうでなければfalse</returns>
        public bool HandleClick(ScreenPoint screenPoint)
        {
            if (!IsClickable)
            {
                return false;
            }

            // プロットエリア内かどうかをチェック
            if (!IsPointInPlotArea(screenPoint))
            {
                return false;
            }

            // スクリーン座標をデータ座標に変換
            var dataPoint = InverseTransform(screenPoint);
            
            // データ座標が有効かチェック
            if (double.IsNaN(dataPoint.X) || double.IsNaN(dataPoint.Y))
            {
                return false;
            }

            // 既存の点がクリックされたかチェック
            var clickedPoint = FindNearestPoint(screenPoint, ClickTolerance);
            if (clickedPoint != null)
            {
                // クリックされた点のインデックスを取得
                var clickedIndex = Points.IndexOf(clickedPoint);
                if (clickedIndex >= 0)
                {
                    // 同じ点をクリックした場合は何もしない
                    if (clickedIndex == _clickedPointIndex)
                    {
                        OnPointClicked(new PointClickedEventArgs(clickedPoint));
                        return true;
                    }
                    
                    // 前回クリックされた点の色をリセット（元の色に戻す）
                    if (_clickedPointIndex >= 0 && _clickedPointIndex < Points.Count)
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
                    _pointColors[clickedIndex] = ClickedPointMarkerColor;
                    
                    // 新しいクリックされた点のインデックスを保存
                    _clickedPointIndex = clickedIndex;
                    
                    // プロットを更新して色変更を反映
                    PlotModel?.InvalidatePlot(true);
                }
                
                // 既存の点がクリックされた場合
                OnPointClicked(new PointClickedEventArgs(clickedPoint));
                return true;
            }
            else
            {
                // 新しい点を追加
                var newPoint = new ScatterPoint(dataPoint.X, dataPoint.Y, NewPointMarkerSize);
                Points.Add(newPoint);
                
                // 新しい点の色を初期化（新しく追加された点は赤色）
                var newIndex = Points.Count - 1;
                _pointColors[newIndex] = NewPointMarkerColor;
                
                // イベントを発生
                OnPointAdded(new PointAddedEventArgs(newPoint));
                
                // プロットを更新
                PlotModel?.InvalidatePlot(true);
                
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
            if (PlotModel == null)
            {
                return false;
            }

            // プロットエリアの境界を取得
            var plotArea = PlotModel.PlotArea;
            
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

            foreach (var point in Points)
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
            if (index >= 0 && index < Points.Count)
            {
                Points.RemoveAt(index);
                PlotModel?.InvalidatePlot(true);
            }
        }

        /// <summary>
        /// シリーズを描画します（各点の色を個別に設定）
        /// </summary>
        /// <param name="rc">描画コンテキスト</param>
        public override void Render(IRenderContext rc)
        {
            if (PlotModel == null)
            {
                return;
            }

            var actualPoints = ActualPointsList;
            if (actualPoints == null || actualPoints.Count == 0)
            {
                return;
            }

            // 各点を個別の色で描画
            for (int i = 0; i < actualPoints.Count; i++)
            {
                var point = actualPoints[i];
                if (point == null || double.IsNaN(point.X) || double.IsNaN(point.Y))
                {
                    continue;
                }

                var screenPoint = this.Transform(point.X, point.Y);
                
                // 点の色を決定
                var pointColor = _pointColors.TryGetValue(i, out OxyColor value) ? value : GetOriginalPointColor(i);
                var pointSize = MarkerSize;
                
                if (i == _clickedPointIndex)
                {
                    pointSize = ClickedPointMarkerSize;
                }

                // マーカーを描画
                rc.DrawMarker(
                    screenPoint,
                    MarkerType,
                    MarkerOutline,
                    pointSize,
                    pointColor,
                    MarkerStroke,
                    MarkerStrokeThickness,
                    EdgeRenderingMode);
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
            Point = point;
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
            Point = point;
        }
    }
}
