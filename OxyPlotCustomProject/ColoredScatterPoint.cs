using OxyPlot;
using OxyPlot.Series;

namespace OxyPlotCustomProject
{
    /// <summary>
    /// 色情報を持つカスタムScatterPoint
    /// </summary>
    public class ColoredScatterPoint : ScatterPoint
    {
        /// <summary>
        /// 点の色
        /// </summary>
        public OxyColor Color { get; set; }

        /// <summary>
        /// 点がクリックされているかどうか
        /// </summary>
        public bool IsClicked { get; set; }

        /// <summary>
        /// 新しい <see cref="ColoredScatterPoint"/> のインスタンスを初期化します
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="size">サイズ</param>
        /// <param name="color">色</param>
        public ColoredScatterPoint(double x, double y, double size, OxyColor color) 
            : base(x, y, size)
        {
            this.Color = color;
            this.IsClicked = false;
        }

        /// <summary>
        /// 新しい <see cref="ColoredScatterPoint"/> のインスタンスを初期化します
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="size">サイズ</param>
        public ColoredScatterPoint(double x, double y, double size) 
            : this(x, y, size, OxyColors.Blue)
        {
        }
    }
}
