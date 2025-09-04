using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using OxyPlot;
using OxyPlot.Series;

namespace OxyPlotCustomProject
{
    /// <summary>
    /// クリック可能な散布図シリーズ専用のマウスビヘイビア
    /// 既存のPlotViewMouseBehaviorを変更せずに、新しい機能を提供します
    /// </summary>
    public class ClickableScatterMouseBehavior : Behavior<OxyPlot.Wpf.PlotView>
    {
        public static readonly DependencyProperty MouseMoveCommandProperty =
            DependencyProperty.Register(nameof(MouseMoveCommand), typeof(ICommand), typeof(ClickableScatterMouseBehavior));

        public static readonly DependencyProperty MouseDownCommandProperty =
            DependencyProperty.Register(nameof(MouseDownCommand), typeof(ICommand), typeof(ClickableScatterMouseBehavior));

        public static readonly DependencyProperty EnableClickableSeriesProperty =
            DependencyProperty.Register(nameof(EnableClickableSeries), typeof(bool), typeof(ClickableScatterMouseBehavior), new PropertyMetadata(true));

        public ICommand MouseMoveCommand
        {
            get { return (ICommand)GetValue(MouseMoveCommandProperty); }
            set { SetValue(MouseMoveCommandProperty, value); }
        }

        public ICommand MouseDownCommand
        {
            get { return (ICommand)GetValue(MouseDownCommandProperty); }
            set { SetValue(MouseDownCommandProperty, value); }
        }

        /// <summary>
        /// クリック可能なシリーズの機能を有効にするかどうか
        /// </summary>
        public bool EnableClickableSeries
        {
            get { return (bool)GetValue(EnableClickableSeriesProperty); }
            set { SetValue(EnableClickableSeriesProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (MouseMoveCommand != null && MouseMoveCommand.CanExecute(null))
            {
                var position = e.GetPosition(AssociatedObject);
                var screenPoint = new ScreenPoint(position.X, position.Y);
                MouseMoveCommand.Execute(screenPoint);
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(AssociatedObject);
            var screenPoint = new ScreenPoint(position.X, position.Y);

            // クリック可能なシリーズの処理を優先
            if (EnableClickableSeries && HandleClickableSeries(screenPoint))
            {
                e.Handled = true;
                return;
            }

            // 通常のコマンド処理
            if (MouseDownCommand != null && MouseDownCommand.CanExecute(null))
            {
                MouseDownCommand.Execute(screenPoint);
                e.Handled = true;
            }
        }

        /// <summary>
        /// クリック可能なシリーズのクリックイベントを処理します
        /// </summary>
        /// <param name="screenPoint">クリックされたスクリーン座標</param>
        /// <returns>クリックが処理された場合はtrue</returns>
        private bool HandleClickableSeries(ScreenPoint screenPoint)
        {
            if (AssociatedObject?.Model?.Series == null)
                return false;

            // すべてのクリック可能なシリーズをチェック
            foreach (var series in AssociatedObject.Model.Series)
            {
                if (series is ClickableScatterSeries clickableSeries)
                {
                    if (clickableSeries.HandleClick(screenPoint))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
