using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using OxyPlot;
using OxyPlot.Series;

namespace OxyPlotCustomProject.RectangleSelectionScatterSeries
{
    /// <summary>
    /// 矩形範囲選択機能付きの散布図のViewModel
    /// </summary>
    public class RectangleSelectionViewModel : INotifyPropertyChanged, IDisposable
    {
        private PlotModel? _plotModel;
        private RectangleSelectionScatterSeries? _scatterSeries;
        private string _statusText = "Shift + マウスドラッグで矩形選択してください（1つの矩形のみ）";
        private int _selectedPointCount = 0;
        private bool _isSelectionEnabled = true;

        /// <summary>
        /// プロットモデル
        /// </summary>
        public PlotModel? PlotModel
        {
            get => _plotModel;
            set
            {
                _plotModel = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 散布図シリーズ
        /// </summary>
        public RectangleSelectionScatterSeries? ScatterSeries
        {
            get => _scatterSeries;
            set
            {
                _scatterSeries = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// ステータステキスト
        /// </summary>
        public string StatusText
        {
            get => _statusText;
            set
            {
                _statusText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 選択されたポイント数
        /// </summary>
        public int SelectedPointCount
        {
            get => _selectedPointCount;
            set
            {
                _selectedPointCount = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 選択機能が有効かどうか
        /// </summary>
        public bool IsSelectionEnabled
        {
            get => _isSelectionEnabled;
            set
            {
                _isSelectionEnabled = value;
                if (_scatterSeries != null)
                {
                    _scatterSeries.IsSelectionEnabled = value;
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 選択されたポイントの詳細情報
        /// </summary>
        public ObservableCollection<PointInfo> SelectedPoints { get; } = [];

        /// <summary>
        /// 選択をクリアするコマンド
        /// </summary>
        public ICommand? ClearSelectionCommand { get; private set; }

        /// <summary>
        /// サンプルデータを生成するコマンド
        /// </summary>
        public ICommand? GenerateSampleDataCommand { get; private set; }

        /// <summary>
        /// 選択機能の有効/無効を切り替えるコマンド
        /// </summary>
        public ICommand? ToggleSelectionCommand { get; private set; }

        /// <summary>
        /// 永続的な矩形をクリアするコマンド
        /// </summary>
        public ICommand? ClearRectanglesCommand { get; private set; }

        /// <summary>
        /// 永続的な矩形の表示/非表示を切り替えるコマンド
        /// </summary>
        public ICommand? ToggleRectanglesCommand { get; private set; }

        /// <summary>
        /// 新しい <see cref="RectangleSelectionViewModel"/> のインスタンスを初期化します
        /// </summary>
        public RectangleSelectionViewModel()
        {
            InitializePlotModel();
            InitializeCommands();
            GenerateSampleData();
        }

        /// <summary>
        /// プロットモデルを初期化します
        /// </summary>
        private void InitializePlotModel()
        {
            _plotModel = new PlotModel
            {
                Title = "矩形範囲選択機能付き散布図",
                Subtitle = "Shift + マウスドラッグで矩形を描画してポイントを選択してください（1つの矩形のみ）"
            };

            // 軸の設定
            _plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Title = "X軸",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            });

            _plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = "Y軸",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            });

            // 散布図シリーズを作成
            _scatterSeries = new RectangleSelectionScatterSeries
            {
                Title = "データポイント",
                MarkerType = MarkerType.Circle,
                MarkerSize = 6.0,
                MarkerFill = OxyColors.Blue,
                MarkerStroke = OxyColors.Black,
                MarkerStrokeThickness = 1.0,
                SelectedPointColor = OxyColors.Red,
                SelectedPointSize = 10.0,
                UnselectedPointColor = OxyColors.Blue,
                UnselectedPointSize = 6.0,
                SelectionRectangleColor = OxyColors.Orange,
                SelectionRectangleThickness = 2.0,
                SelectionRectangleFill = OxyColor.FromArgb(50, 255, 165, 0)
            };

            // イベントハンドラーを設定
            _scatterSeries.SelectionChanged += OnSelectionChanged;
            _scatterSeries.RectangleSelectionStarted += OnRectangleSelectionStarted;
            _scatterSeries.RectangleSelectionEnded += OnRectangleSelectionEnded;

            _plotModel.Series.Add(_scatterSeries);
        }

        /// <summary>
        /// コマンドを初期化します
        /// </summary>
        private void InitializeCommands()
        {
            ClearSelectionCommand = new RelayCommand(
                () => _scatterSeries?.ClearSelection(),
                () => _scatterSeries != null && _scatterSeries.GetSelectedPointIndices().Count > 0);

            GenerateSampleDataCommand = new RelayCommand(
                () => GenerateSampleData(),
                () => _scatterSeries != null);

            ToggleSelectionCommand = new RelayCommand(
                () => IsSelectionEnabled = !IsSelectionEnabled,
                () => _scatterSeries != null);

            ClearRectanglesCommand = new RelayCommand(
                () => _scatterSeries?.ClearPersistentRectangles(),
                () => _scatterSeries != null);

            ToggleRectanglesCommand = new RelayCommand(
                () => _scatterSeries?.TogglePersistentRectangles(),
                () => _scatterSeries != null);
        }

        /// <summary>
        /// サンプルデータを生成します
        /// </summary>
        private void GenerateSampleData()
        {
            if (_scatterSeries == null) return;

            _scatterSeries.Points.Clear();
            _scatterSeries.ClearSelection();

            var random = new Random(42); // 固定シードで再現可能なデータを生成

            // ランダムなデータポイントを生成
            for (int i = 0; i < 50; i++)
            {
                var x = random.NextDouble() * 100;
                var y = random.NextDouble() * 100;
                var size = 5.0 + random.NextDouble() * 5.0; // 5-10の範囲でサイズをランダム化
                _scatterSeries.AddPointWithSelectionState(new ScatterPoint(x, y, size));
            }

            // プロットを更新
            _plotModel?.InvalidatePlot(true);
            StatusText = "サンプルデータを生成しました。Shift + マウスドラッグで矩形選択してください（1つの矩形のみ）。";
        }

        /// <summary>
        /// 選択が変更されたときのイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信者</param>
        /// <param name="e">イベント引数</param>
        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            SelectedPointCount = e.SelectedIndices.Count;
            
            // 選択されたポイントの詳細情報を更新
            SelectedPoints.Clear();
            foreach (var index in e.SelectedIndices)
            {
                if (_scatterSeries != null && index < _scatterSeries.Points.Count)
                {
                    var point = _scatterSeries.Points[index];
                    SelectedPoints.Add(new PointInfo
                    {
                        Index = index,
                        X = point.X,
                        Y = point.Y,
                        Size = point.Size
                    });
                }
            }

            StatusText = $"{SelectedPointCount}個のポイントが選択されています。";
        }

        /// <summary>
        /// 矩形選択が開始されたときのイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信者</param>
        /// <param name="e">イベント引数</param>
        private void OnRectangleSelectionStarted(object? sender, RectangleSelectionEventArgs e)
        {
            StatusText = "矩形選択中...";
        }

        /// <summary>
        /// 矩形選択が終了したときのイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信者</param>
        /// <param name="e">イベント引数</param>
        private void OnRectangleSelectionEnded(object? sender, RectangleSelectionEventArgs e)
        {
            var selectedCount = _scatterSeries?.GetSelectedPointIndices().Count ?? 0;
            StatusText = $"{selectedCount}個のポイントが選択されました。";
        }

        /// <summary>
        /// プロパティ変更イベント
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// プロパティ変更イベントを発生させます
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// リソースを解放します
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// リソースを解放します
        /// </summary>
        /// <param name="disposing">マネージドリソースを解放するかどうか</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // イベントハンドラーを解除してメモリリークを防ぐ
                if (_scatterSeries != null)
                {
                    _scatterSeries.SelectionChanged -= OnSelectionChanged;
                    _scatterSeries.RectangleSelectionStarted -= OnRectangleSelectionStarted;
                    _scatterSeries.RectangleSelectionEnded -= OnRectangleSelectionEnded;
                }
            }
        }
    }

    /// <summary>
    /// ポイントの詳細情報
    /// </summary>
    public class PointInfo
    {
        /// <summary>
        /// ポイントのインデックス
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// X座標
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y座標
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// ポイントのサイズ
        /// </summary>
        public double Size { get; set; }

        /// <summary>
        /// 表示用の文字列
        /// </summary>
        public string DisplayText => $"ポイント{Index}: ({X:F2}, {Y:F2}) サイズ:{Size:F1}";
    }
}
