using OxyPlot;
using OxyPlot.Series;

namespace OxyPlotCustomProject.RectangleSelectionScatterSeries
{
    /// <summary>
    /// 矩形範囲選択機能付きの散布図シリーズ
    /// マウスドラッグで矩形を描画し、範囲内のポイントを選択できます
    /// </summary>
    public class RectangleSelectionScatterSeries : ScatterSeries
    {
        /// <summary>
        /// 範囲選択が変更されたときに発生するイベント
        /// </summary>
        public new event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

        /// <summary>
        /// 矩形選択が開始されたときに発生するイベント
        /// </summary>
        public event EventHandler<RectangleSelectionEventArgs>? RectangleSelectionStarted;

        /// <summary>
        /// 矩形選択が終了したときに発生するイベント
        /// </summary>
        public event EventHandler<RectangleSelectionEventArgs>? RectangleSelectionEnded;

        /// <summary>
        /// 範囲選択機能が有効かどうか
        /// </summary>
        public bool IsSelectionEnabled { get; set; } = true;

        /// <summary>
        /// 選択されたポイントの色
        /// </summary>
        public OxyColor SelectedPointColor { get; set; } = OxyColors.Red;

        /// <summary>
        /// 選択されたポイントのサイズ
        /// </summary>
        public double SelectedPointSize { get; set; } = 10.0;

        /// <summary>
        /// 選択されていないポイントの色
        /// </summary>
        public OxyColor UnselectedPointColor { get; set; } = OxyColors.Blue;

        /// <summary>
        /// 選択されていないポイントのサイズ
        /// </summary>
        public double UnselectedPointSize { get; set; } = 6.0;

        /// <summary>
        /// 矩形選択の境界線の色
        /// </summary>
        public OxyColor SelectionRectangleColor { get; set; } = OxyColors.Orange;

        /// <summary>
        /// 矩形選択の境界線の太さ
        /// </summary>
        public double SelectionRectangleThickness { get; set; } = 2.0;

        /// <summary>
        /// 矩形選択の塗りつぶし色（半透明）
        /// </summary>
        public OxyColor SelectionRectangleFill { get; set; } = OxyColor.FromArgb(50, 255, 165, 0);

        /// <summary>
        /// 現在選択されているポイントのインデックス
        /// </summary>
        private HashSet<int> _selectedPointIndices = new HashSet<int>();

        /// <summary>
        /// 現在の矩形選択範囲（スクリーン座標）
        /// </summary>
        private OxyRect? _currentSelectionRectangle;

        /// <summary>
        /// 永続的に表示する選択矩形のリスト（データ座標）
        /// </summary>
        private List<DataRect> _persistentSelectionRectangles = new List<DataRect>();

        /// <summary>
        /// 矩形選択が進行中かどうか
        /// </summary>
        private bool _isSelecting = false;

        /// <summary>
        /// 矩形選択の開始点（スクリーン座標）
        /// </summary>
        private ScreenPoint _selectionStartPoint;

        /// <summary>
        /// 矩形選択の開始点（データ座標）
        /// </summary>
        private DataPoint _selectionStartDataPoint;

        /// <summary>
        /// 永続的な矩形を表示するかどうか
        /// </summary>
        public bool ShowPersistentRectangles { get; set; } = true;

        /// <summary>
        /// 各ポイントの選択状態を管理する辞書
        /// </summary>
        private Dictionary<int, bool> _pointSelectionStates = new Dictionary<int, bool>();

        /// <summary>
        /// 新しい <see cref="RectangleSelectionScatterSeries"/> のインスタンスを初期化します
        /// </summary>
        public RectangleSelectionScatterSeries()
        {
            MarkerType = MarkerType.Circle;
            MarkerSize = UnselectedPointSize;
            MarkerFill = UnselectedPointColor;
            MarkerStroke = OxyColors.Black;
            MarkerStrokeThickness = 1.0;
        }

        /// <summary>
        /// ポイントを追加します
        /// </summary>
        /// <param name="point">追加するポイント</param>
        public void AddPoint(ScatterPoint point)
        {
            Points.Add(point);
            var index = Points.Count - 1;
            _pointSelectionStates[index] = false;
        }

        /// <summary>
        /// 初期化時にすべてのポイントの選択状態を設定します
        /// </summary>
        public void InitializePointSelectionStates()
        {
            _pointSelectionStates.Clear();
            for (int i = 0; i < Points.Count; i++)
            {
                _pointSelectionStates[i] = false;
            }
        }

        /// <summary>
        /// 矩形選択を開始します
        /// </summary>
        /// <param name="startPoint">選択開始点（スクリーン座標）</param>
        public void StartRectangleSelection(ScreenPoint startPoint)
        {
            if (!IsSelectionEnabled)
                return;

            // 新しい矩形選択を開始する前に、既存の矩形と選択をクリア
            _persistentSelectionRectangles.Clear();
            _selectedPointIndices.Clear();
            for (int i = 0; i < _pointSelectionStates.Count; i++)
            {
                _pointSelectionStates[i] = false;
            }

            _isSelecting = true;
            _selectionStartPoint = startPoint;
            
            // スクリーン座標をデータ座標に変換
            _selectionStartDataPoint = InverseTransform(startPoint);
            
            _currentSelectionRectangle = new OxyRect(startPoint.X, startPoint.Y, 0, 0);

            OnRectangleSelectionStarted(new RectangleSelectionEventArgs(startPoint));
            PlotModel?.InvalidatePlot(false);
        }

        /// <summary>
        /// 矩形選択を更新します
        /// </summary>
        /// <param name="currentPoint">現在のマウス位置（スクリーン座標）</param>
        public void UpdateRectangleSelection(ScreenPoint currentPoint)
        {
            if (!_isSelecting || !IsSelectionEnabled)
                return;

            var minX = Math.Min(_selectionStartPoint.X, currentPoint.X);
            var minY = Math.Min(_selectionStartPoint.Y, currentPoint.Y);
            var maxX = Math.Max(_selectionStartPoint.X, currentPoint.X);
            var maxY = Math.Max(_selectionStartPoint.Y, currentPoint.Y);

            var newRect = new OxyRect(minX, minY, maxX - minX, maxY - minY);
            
            // 矩形が実際に変更された場合のみ再描画
            if (!_currentSelectionRectangle.HasValue || 
                Math.Abs(_currentSelectionRectangle.Value.Left - newRect.Left) > 1 ||
                Math.Abs(_currentSelectionRectangle.Value.Top - newRect.Top) > 1 ||
                Math.Abs(_currentSelectionRectangle.Value.Width - newRect.Width) > 1 ||
                Math.Abs(_currentSelectionRectangle.Value.Height - newRect.Height) > 1)
            {
                _currentSelectionRectangle = newRect;
                PlotModel?.InvalidatePlot(false); // 強制再描画を避ける
            }
        }

        /// <summary>
        /// 矩形選択を終了し、範囲内のポイントを選択します
        /// </summary>
        /// <param name="endPoint">選択終了点（スクリーン座標）</param>
        public void EndRectangleSelection(ScreenPoint endPoint)
        {
            if (!_isSelecting || !IsSelectionEnabled)
                return;

            _isSelecting = false;

            if (_currentSelectionRectangle.HasValue)
            {
                // スクリーン座標をデータ座標に変換して矩形を作成
                var currentDataPoint = InverseTransform(new ScreenPoint(_currentSelectionRectangle.Value.Right, _currentSelectionRectangle.Value.Bottom));
                
                var dataRect = new DataRect(
                    Math.Min(_selectionStartDataPoint.X, currentDataPoint.X),
                    Math.Min(_selectionStartDataPoint.Y, currentDataPoint.Y),
                    Math.Abs(currentDataPoint.X - _selectionStartDataPoint.X),
                    Math.Abs(currentDataPoint.Y - _selectionStartDataPoint.Y)
                );

                // 永続的な矩形リストをクリアして、新しい矩形のみを追加
                _persistentSelectionRectangles.Clear();
                _persistentSelectionRectangles.Add(dataRect);
                
                // 範囲内のポイントを選択
                SelectPointsInDataRectangle(dataRect);
            }

            _currentSelectionRectangle = null;

            OnRectangleSelectionEnded(new RectangleSelectionEventArgs(endPoint));
            PlotModel?.InvalidatePlot(false);
        }

        /// <summary>
        /// 指定された矩形内のポイントを選択します
        /// </summary>
        /// <param name="rectangle">選択矩形（スクリーン座標）</param>
        private void SelectPointsInRectangle(OxyRect rectangle)
        {
            var newSelectedIndices = new HashSet<int>();

            for (int i = 0; i < Points.Count; i++)
            {
                var point = Points[i];
                var screenPoint = this.Transform(point.X, point.Y);

                if (rectangle.Contains(screenPoint.X, screenPoint.Y))
                {
                    newSelectedIndices.Add(i);
                    _pointSelectionStates[i] = true;
                }
                else
                {
                    _pointSelectionStates[i] = false;
                }
            }

            _selectedPointIndices = newSelectedIndices;
            OnSelectionChanged(new SelectionChangedEventArgs(_selectedPointIndices.ToList()));
        }

        /// <summary>
        /// 指定されたデータ座標矩形内のポイントを選択します
        /// </summary>
        /// <param name="dataRect">選択矩形（データ座標）</param>
        private void SelectPointsInDataRectangle(DataRect dataRect)
        {
            var newSelectedIndices = new HashSet<int>();

            for (int i = 0; i < Points.Count; i++)
            {
                var point = Points[i];

                if (dataRect.Contains(point.X, point.Y))
                {
                    newSelectedIndices.Add(i);
                    _pointSelectionStates[i] = true;
                }
                else
                {
                    _pointSelectionStates[i] = false;
                }
            }

            _selectedPointIndices = newSelectedIndices;
            OnSelectionChanged(new SelectionChangedEventArgs(_selectedPointIndices.ToList()));
        }

        /// <summary>
        /// 指定されたインデックスのポイントを選択状態にします
        /// </summary>
        /// <param name="index">ポイントのインデックス</param>
        public void SelectPoint(int index)
        {
            if (index >= 0 && index < Points.Count)
            {
                _selectedPointIndices.Add(index);
                _pointSelectionStates[index] = true;
                            OnSelectionChanged(new SelectionChangedEventArgs(_selectedPointIndices.ToList()));
            PlotModel?.InvalidatePlot(false);
            }
        }

        /// <summary>
        /// 指定されたインデックスのポイントの選択を解除します
        /// </summary>
        /// <param name="index">ポイントのインデックス</param>
        public void DeselectPoint(int index)
        {
            if (index >= 0 && index < Points.Count)
            {
                _selectedPointIndices.Remove(index);
                _pointSelectionStates[index] = false;
                            OnSelectionChanged(new SelectionChangedEventArgs(_selectedPointIndices.ToList()));
            PlotModel?.InvalidatePlot(false);
            }
        }

        /// <summary>
        /// すべてのポイントの選択を解除します
        /// </summary>
        public new void ClearSelection()
        {
            _selectedPointIndices.Clear();
            for (int i = 0; i < _pointSelectionStates.Count; i++)
            {
                _pointSelectionStates[i] = false;
            }
            OnSelectionChanged(new SelectionChangedEventArgs(new List<int>()));
            PlotModel?.InvalidatePlot(false);
        }

        /// <summary>
        /// 永続的な矩形をクリアします
        /// </summary>
        public void ClearPersistentRectangles()
        {
            _persistentSelectionRectangles.Clear();
            PlotModel?.InvalidatePlot(false);
        }

        /// <summary>
        /// 永続的な矩形の表示/非表示を切り替えます
        /// </summary>
        public void TogglePersistentRectangles()
        {
            ShowPersistentRectangles = !ShowPersistentRectangles;
            PlotModel?.InvalidatePlot(false);
        }

        /// <summary>
        /// 現在選択されているポイントのインデックスを取得します
        /// </summary>
        /// <returns>選択されているポイントのインデックスのリスト</returns>
        public List<int> GetSelectedPointIndices()
        {
            return _selectedPointIndices.ToList();
        }

        /// <summary>
        /// 現在選択されているポイントを取得します
        /// </summary>
        /// <returns>選択されているポイントのリスト</returns>
        public List<ScatterPoint> GetSelectedPoints()
        {
            return _selectedPointIndices.Select(i => Points[i]).ToList();
        }

        /// <summary>
        /// シリーズを描画します（選択状態に応じてポイントの色とサイズを変更）
        /// </summary>
        /// <param name="rc">描画コンテキスト</param>
        public override void Render(IRenderContext rc)
        {
            if (PlotModel == null)
                return;

            var actualPoints = ActualPointsList;
            if (actualPoints == null || actualPoints.Count == 0)
                return;

            // 各ポイントを描画
            for (int i = 0; i < actualPoints.Count; i++)
            {
                var point = actualPoints[i];
                if (point == null || double.IsNaN(point.X) || double.IsNaN(point.Y))
                    continue;

                var screenPoint = this.Transform(point.X, point.Y);
                
                // ポイントの選択状態に応じて色とサイズを決定
                var isSelected = _pointSelectionStates.ContainsKey(i) && _pointSelectionStates[i];
                var pointColor = isSelected ? SelectedPointColor : UnselectedPointColor;
                var pointSize = isSelected ? SelectedPointSize : UnselectedPointSize;

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

            // 永続的な矩形を描画
            if (ShowPersistentRectangles)
            {
                foreach (var dataRect in _persistentSelectionRectangles)
                {
                    // データ座標をスクリーン座標に変換
                    var topLeft = this.Transform(dataRect.Left, dataRect.Top);
                    var bottomRight = this.Transform(dataRect.Right, dataRect.Bottom);
                    
                    var screenRect = new OxyRect(
                        Math.Min(topLeft.X, bottomRight.X),
                        Math.Min(topLeft.Y, bottomRight.Y),
                        Math.Abs(bottomRight.X - topLeft.X),
                        Math.Abs(bottomRight.Y - topLeft.Y)
                    );

                    // 矩形の塗りつぶし
                    rc.DrawRectangle(screenRect, SelectionRectangleFill, OxyColors.Transparent, 0, EdgeRenderingMode);
                    
                    // 矩形の境界線
                    rc.DrawRectangle(screenRect, OxyColors.Transparent, SelectionRectangleColor, SelectionRectangleThickness, EdgeRenderingMode);
                }
            }

            // 現在の矩形選択範囲を描画
            if (_currentSelectionRectangle.HasValue)
            {
                var rect = _currentSelectionRectangle.Value;
                
                // 矩形の塗りつぶし
                rc.DrawRectangle(rect, SelectionRectangleFill, OxyColors.Transparent, 0, EdgeRenderingMode);
                
                // 矩形の境界線
                rc.DrawRectangle(rect, OxyColors.Transparent, SelectionRectangleColor, SelectionRectangleThickness, EdgeRenderingMode);
            }
        }

        /// <summary>
        /// SelectionChangedイベントを発生させます
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// RectangleSelectionStartedイベントを発生させます
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected virtual void OnRectangleSelectionStarted(RectangleSelectionEventArgs e)
        {
            RectangleSelectionStarted?.Invoke(this, e);
        }

        /// <summary>
        /// RectangleSelectionEndedイベントを発生させます
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected virtual void OnRectangleSelectionEnded(RectangleSelectionEventArgs e)
        {
            RectangleSelectionEnded?.Invoke(this, e);
        }
    }

    /// <summary>
    /// 選択が変更されたときのイベント引数
    /// </summary>
    public class SelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 選択されているポイントのインデックスのリスト
        /// </summary>
        public List<int> SelectedIndices { get; }

        /// <summary>
        /// 新しい <see cref="SelectionChangedEventArgs"/> のインスタンスを初期化します
        /// </summary>
        /// <param name="selectedIndices">選択されているポイントのインデックスのリスト</param>
        public SelectionChangedEventArgs(List<int> selectedIndices)
        {
            SelectedIndices = selectedIndices;
        }
    }

    /// <summary>
    /// 矩形選択のイベント引数
    /// </summary>
    public class RectangleSelectionEventArgs : EventArgs
    {
        /// <summary>
        /// スクリーン座標のポイント
        /// </summary>
        public ScreenPoint ScreenPoint { get; }

        /// <summary>
        /// 新しい <see cref="RectangleSelectionEventArgs"/> のインスタンスを初期化します
        /// </summary>
        /// <param name="screenPoint">スクリーン座標のポイント</param>
        public RectangleSelectionEventArgs(ScreenPoint screenPoint)
        {
            ScreenPoint = screenPoint;
        }
    }

    /// <summary>
    /// データ座標での矩形を表すクラス
    /// </summary>
    public class DataRect
    {
        /// <summary>
        /// 矩形の左端のX座標
        /// </summary>
        public double Left { get; set; }

        /// <summary>
        /// 矩形の上端のY座標
        /// </summary>
        public double Top { get; set; }

        /// <summary>
        /// 矩形の幅
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 矩形の高さ
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 矩形の右端のX座標
        /// </summary>
        public double Right => Left + Width;

        /// <summary>
        /// 矩形の下端のY座標
        /// </summary>
        public double Bottom => Top + Height;

        /// <summary>
        /// 新しい <see cref="DataRect"/> のインスタンスを初期化します
        /// </summary>
        /// <param name="left">左端のX座標</param>
        /// <param name="top">上端のY座標</param>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        public DataRect(double left, double top, double width, double height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// 指定された点が矩形内にあるかどうかを判定します
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <returns>矩形内にある場合はtrue</returns>
        public bool Contains(double x, double y)
        {
            return x >= Left && x <= Right && y >= Top && y <= Bottom;
        }
    }
}
