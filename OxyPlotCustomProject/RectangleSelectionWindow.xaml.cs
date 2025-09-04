using System.Windows;

namespace OxyPlotCustomProject
{
    /// <summary>
    /// RectangleSelectionWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class RectangleSelectionWindow : Window
    {
        /// <summary>
        /// 新しい <see cref="RectangleSelectionWindow"/> のインスタンスを初期化します
        /// </summary>
        public RectangleSelectionWindow()
        {
            InitializeComponent();
            this.DataContext = new RectangleSelectionViewModel();
        }

        /// <summary>
        /// ウィンドウが閉じられる時にリソースを解放します
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected override void OnClosed(EventArgs e)
        {
            // ViewModelを適切に解放してメモリリークを防ぐ
            if (this.DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }
            base.OnClosed(e);
        }
    }
}
