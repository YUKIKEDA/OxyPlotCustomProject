using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using OxyPlot;

namespace OxyPlotCustomProject.ParallelCoordinatesSeries
{
    /// <summary>
    /// PlotViewのマウスイベントをViewModelのコマンドにバインドするビヘイビア
    /// </summary>
    public class PlotViewMouseBehavior : Behavior<OxyPlot.Wpf.PlotView>
    {
        public static readonly DependencyProperty MouseMoveCommandProperty =
            DependencyProperty.Register(nameof(MouseMoveCommand), typeof(ICommand), typeof(PlotViewMouseBehavior));

        public static readonly DependencyProperty MouseDownCommandProperty =
            DependencyProperty.Register(nameof(MouseDownCommand), typeof(ICommand), typeof(PlotViewMouseBehavior));

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
            if (MouseDownCommand != null && MouseDownCommand.CanExecute(null))
            {
                var position = e.GetPosition(AssociatedObject);
                var screenPoint = new ScreenPoint(position.X, position.Y);
                MouseDownCommand.Execute(screenPoint);
                e.Handled = true;
            }
        }
    }
}
