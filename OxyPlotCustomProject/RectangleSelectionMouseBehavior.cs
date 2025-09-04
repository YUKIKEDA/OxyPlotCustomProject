using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using OxyPlot;
using OxyPlot.Series;

namespace OxyPlotCustomProject
{
    /// <summary>
    /// 矩形範囲選択機能付きの散布図シリーズ専用のマウスビヘイビア
    /// マウスドラッグで矩形選択を実現します
    /// </summary>
    public class RectangleSelectionMouseBehavior : Behavior<OxyPlot.Wpf.PlotView>
    {
        public static readonly DependencyProperty MouseMoveCommandProperty =
            DependencyProperty.Register(nameof(MouseMoveCommand), typeof(ICommand), typeof(RectangleSelectionMouseBehavior));

        public static readonly DependencyProperty MouseDownCommandProperty =
            DependencyProperty.Register(nameof(MouseDownCommand), typeof(ICommand), typeof(RectangleSelectionMouseBehavior));

        public static readonly DependencyProperty MouseUpCommandProperty =
            DependencyProperty.Register(nameof(MouseUpCommand), typeof(ICommand), typeof(RectangleSelectionMouseBehavior));

        public static readonly DependencyProperty EnableRectangleSelectionProperty =
            DependencyProperty.Register(nameof(EnableRectangleSelection), typeof(bool), typeof(RectangleSelectionMouseBehavior), new PropertyMetadata(true));

        /// <summary>
        /// マウス移動時のコマンド
        /// </summary>
        public ICommand MouseMoveCommand
        {
            get { return (ICommand)GetValue(MouseMoveCommandProperty); }
            set { SetValue(MouseMoveCommandProperty, value); }
        }

        /// <summary>
        /// マウスダウン時のコマンド
        /// </summary>
        public ICommand MouseDownCommand
        {
            get { return (ICommand)GetValue(MouseDownCommandProperty); }
            set { SetValue(MouseDownCommandProperty, value); }
        }

        /// <summary>
        /// マウスアップ時のコマンド
        /// </summary>
        public ICommand MouseUpCommand
        {
            get { return (ICommand)GetValue(MouseUpCommandProperty); }
            set { SetValue(MouseUpCommandProperty, value); }
        }

        /// <summary>
        /// 矩形選択機能を有効にするかどうか
        /// </summary>
        public bool EnableRectangleSelection
        {
            get { return (bool)GetValue(EnableRectangleSelectionProperty); }
            set { SetValue(EnableRectangleSelectionProperty, value); }
        }

        /// <summary>
        /// ドラッグが進行中かどうか
        /// </summary>
        private bool _isDragging = false;

        /// <summary>
        /// ドラッグ開始点
        /// </summary>
        private ScreenPoint _dragStartPoint;

        /// <summary>
        /// 最後に更新した時刻（メモリリーク防止のため更新頻度を制限）
        /// </summary>
        private DateTime _lastUpdateTime = DateTime.MinValue;

        /// <summary>
        /// 更新間隔（ミリ秒）
        /// </summary>
        private const int UpdateIntervalMs = 16; // 約60FPS

        protected override void OnAttached()
        {
            base.OnAttached();
            // PreviewMouseイベントを使用して、PlotViewの内部処理より先に処理する
            AssociatedObject.PreviewMouseMove += OnMouseMove;
            AssociatedObject.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
            AssociatedObject.MouseLeave += OnMouseLeave; // PreviewMouseLeaveは存在しないため、通常のMouseLeaveを使用
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseMove -= OnMouseMove;
            AssociatedObject.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.PreviewMouseLeftButtonUp -= OnMouseLeftButtonUp;
            AssociatedObject.MouseLeave -= OnMouseLeave;
            
            // マウスキャプチャを解放（念のため）
            AssociatedObject.ReleaseMouseCapture();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(AssociatedObject);
            var screenPoint = new ScreenPoint(position.X, position.Y);

            // シフトキーが押されていて、ドラッグ中の場合は矩形選択を更新
            if (EnableRectangleSelection && _isDragging && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                // 更新頻度を制限してメモリリークを防ぐ
                var now = DateTime.Now;
                if ((now - _lastUpdateTime).TotalMilliseconds >= UpdateIntervalMs)
                {
                    // マウスがプロットエリア外にある場合でも矩形選択を継続
                    // 座標をプロットエリア内に制限する
                    var clampedPoint = ClampToPlotArea(screenPoint);
                    HandleRectangleSelectionUpdate(clampedPoint);
                    _lastUpdateTime = now;
                }
                e.Handled = true;
                return;
            }

            // 通常のマウス移動コマンド
            if (MouseMoveCommand != null && MouseMoveCommand.CanExecute(null))
            {
                MouseMoveCommand.Execute(screenPoint);
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(AssociatedObject);
            var screenPoint = new ScreenPoint(position.X, position.Y);

            // シフトキーが押されている場合のみ矩形選択を開始
            if (EnableRectangleSelection && Keyboard.Modifiers == ModifierKeys.Shift && HandleRectangleSelectionStart(screenPoint))
            {
                _isDragging = true;
                _dragStartPoint = screenPoint;
                
                // マウスキャプチャを開始して、マウスがプロットエリア外に移動しても矩形選択を継続
                AssociatedObject.CaptureMouse();
                
                e.Handled = true;
                return;
            }

            // シフトキーが押されていない場合は、通常のマウスダウンコマンドを処理しない
            // これにより、PlotViewの標準的なマウス処理が動作する
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(AssociatedObject);
            var screenPoint = new ScreenPoint(position.X, position.Y);

            // シフトキーが押されていて、ドラッグ中の場合は矩形選択を終了
            if (_isDragging && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                // マウスがプロットエリア外にある場合でも矩形選択を終了
                // 座標をプロットエリア内に制限する
                var clampedPoint = ClampToPlotArea(screenPoint);
                HandleRectangleSelectionEnd(clampedPoint);
                
                // マウスキャプチャを解放
                AssociatedObject.ReleaseMouseCapture();
                
                _isDragging = false;
                e.Handled = true;
                return;
            }

            // ドラッグが終了した場合は、フラグをリセット
            if (_isDragging)
            {
                _isDragging = false;
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            // マウスがプロットエリアを離れても矩形選択は継続する
            // マウスアップイベントでのみ矩形選択を終了する
            // これにより、マウスがプロットエリア外に移動しても矩形選択が継続される
        }

        /// <summary>
        /// 座標をプロットエリア内に制限します
        /// </summary>
        /// <param name="screenPoint">制限する座標</param>
        /// <returns>制限された座標</returns>
        private ScreenPoint ClampToPlotArea(ScreenPoint screenPoint)
        {
            if (AssociatedObject?.Model == null)
                return screenPoint;

            var plotArea = AssociatedObject.Model.PlotArea;
            
            var clampedX = Math.Max(plotArea.Left, Math.Min(plotArea.Right, screenPoint.X));
            var clampedY = Math.Max(plotArea.Top, Math.Min(plotArea.Bottom, screenPoint.Y));
            
            return new ScreenPoint(clampedX, clampedY);
        }

        /// <summary>
        /// 矩形選択を開始します
        /// </summary>
        /// <param name="screenPoint">開始点（スクリーン座標）</param>
        /// <returns>矩形選択が開始された場合はtrue</returns>
        private bool HandleRectangleSelectionStart(ScreenPoint screenPoint)
        {
            if (AssociatedObject?.Model?.Series == null)
                return false;

            // 矩形選択可能なシリーズを検索
            foreach (var series in AssociatedObject.Model.Series)
            {
                if (series is RectangleSelectionScatterSeries selectionSeries)
                {
                    if (selectionSeries.IsSelectionEnabled)
                    {
                        selectionSeries.StartRectangleSelection(screenPoint);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 矩形選択を更新します
        /// </summary>
        /// <param name="screenPoint">現在のマウス位置（スクリーン座標）</param>
        private void HandleRectangleSelectionUpdate(ScreenPoint screenPoint)
        {
            if (AssociatedObject?.Model?.Series == null)
                return;

            // 矩形選択可能なシリーズを検索
            foreach (var series in AssociatedObject.Model.Series)
            {
                if (series is RectangleSelectionScatterSeries selectionSeries)
                {
                    if (selectionSeries.IsSelectionEnabled)
                    {
                        selectionSeries.UpdateRectangleSelection(screenPoint);
                        break; // 最初に見つかったシリーズのみ処理
                    }
                }
            }
        }

        /// <summary>
        /// 矩形選択を終了します
        /// </summary>
        /// <param name="screenPoint">終了点（スクリーン座標）</param>
        private void HandleRectangleSelectionEnd(ScreenPoint screenPoint)
        {
            if (AssociatedObject?.Model?.Series == null)
                return;

            // 矩形選択可能なシリーズを検索
            foreach (var series in AssociatedObject.Model.Series)
            {
                if (series is RectangleSelectionScatterSeries selectionSeries)
                {
                    if (selectionSeries.IsSelectionEnabled)
                    {
                        selectionSeries.EndRectangleSelection(screenPoint);
                        break; // 最初に見つかったシリーズのみ処理
                    }
                }
            }
        }
    }
}
