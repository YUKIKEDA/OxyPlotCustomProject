using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using OxyPlot;
using OxyPlot.Legends;
using OxyPlot.Series;

namespace OxyPlotCustomProject
{
    public class ClickableScatterViewModel : INotifyPropertyChanged
    {
        private ClickableScatterSeries _clickableScatterSeries;
        private string _statusMessage = "グラフ内をクリックして新しい点を追加してください";
        private int _initialPointCount = 4; // 初期点の数

        public PlotModel PlotModel { get; }
        public ObservableCollection<ScatterPoint> Points { get; }
        public ICommand ClearAddedPointsCommand { get; }
        public ICommand RemoveLastAddedPointCommand { get; }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public string PointCountText
        {
            get => $"点の数: {Points.Count}";
        }

        public ClickableScatterViewModel()
        {
            this.PlotModel = new PlotModel { Title = "クリック可能な散布図" };
            this.Points = new ObservableCollection<ScatterPoint>();

            // コマンドの初期化
            this.ClearAddedPointsCommand = new RelayCommand(ClearAddedPoints, () => Points.Count > _initialPointCount);
            this.RemoveLastAddedPointCommand = new RelayCommand(RemoveLastAddedPoint, () => Points.Count > _initialPointCount);

            // 凡例の設定
            var legend = new Legend
            {
                LegendTitle = "凡例",
                LegendPosition = LegendPosition.TopRight,
                LegendPlacement = LegendPlacement.Outside
            };
            this.PlotModel.Legends.Add(legend);

            // クリック可能な散布図シリーズの作成
            _clickableScatterSeries = new ClickableScatterSeries
            {
                Title = "クリック可能な散布図",
                MarkerType = MarkerType.Circle,
                MarkerSize = 8.0,
                MarkerFill = OxyColors.Blue,
                MarkerStroke = OxyColors.Black,
                MarkerStrokeThickness = 1.0,
                NewPointMarkerSize = 6.0,
                NewPointMarkerColor = OxyColors.Red,
                NewPointMarkerType = MarkerType.Circle
            };

            // イベントハンドラーの設定
            _clickableScatterSeries.PointAdded += OnPointAdded;
            _clickableScatterSeries.PointClicked += OnPointClicked;

            // 初期データの追加
            _clickableScatterSeries.AddPoint(new ScatterPoint(0, 0, 8.0));
            _clickableScatterSeries.AddPoint(new ScatterPoint(1, 1, 8.0));
            _clickableScatterSeries.AddPoint(new ScatterPoint(2, 0.5, 8.0));
            _clickableScatterSeries.AddPoint(new ScatterPoint(3, 1.2, 8.0));

            // 初期点の色を明示的に設定
            _clickableScatterSeries.InitializePointColors();

            this.PlotModel.Series.Add(_clickableScatterSeries);

            // 初期点をコレクションに追加
            foreach (var point in _clickableScatterSeries.Points)
            {
                this.Points.Add(point);
            }

            // プロパティ変更通知の設定
            this.Points.CollectionChanged += (s, e) => OnPropertyChanged(nameof(PointCountText));
        }

        private void OnPointAdded(object? sender, PointAddedEventArgs e)
        {
            this.Points.Add(e.Point);
            StatusMessage = $"新しい点が追加されました: ({e.Point.X:F2}, {e.Point.Y:F2})";
            
            // コマンドの実行可能性を更新
            ((RelayCommand)ClearAddedPointsCommand).RaiseCanExecuteChanged();
            ((RelayCommand)RemoveLastAddedPointCommand).RaiseCanExecuteChanged();
        }

        private void OnPointClicked(object? sender, PointClickedEventArgs e)
        {
            StatusMessage = $"点がクリックされました: ({e.Point.X:F2}, {e.Point.Y:F2})";
        }

        private void ClearAddedPoints()
        {
            // 追加した点のみを削除（初期点は残す）
            while (this.Points.Count > _initialPointCount)
            {
                var lastIndex = this.Points.Count - 1;
                _clickableScatterSeries.RemovePointAt(lastIndex);
                this.Points.RemoveAt(lastIndex);
            }
            
            StatusMessage = "追加した点がクリアされました";
            
            // コマンドの実行可能性を更新
            ((RelayCommand)ClearAddedPointsCommand).RaiseCanExecuteChanged();
            ((RelayCommand)RemoveLastAddedPointCommand).RaiseCanExecuteChanged();
        }

        private void RemoveLastAddedPoint()
        {
            if (this.Points.Count > _initialPointCount)
            {
                var lastIndex = this.Points.Count - 1;
                _clickableScatterSeries.RemovePointAt(lastIndex);
                this.Points.RemoveAt(lastIndex);
                StatusMessage = "最後に追加した点が削除されました";
                
                // コマンドの実行可能性を更新
                ((RelayCommand)ClearAddedPointsCommand).RaiseCanExecuteChanged();
                ((RelayCommand)RemoveLastAddedPointCommand).RaiseCanExecuteChanged();
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
