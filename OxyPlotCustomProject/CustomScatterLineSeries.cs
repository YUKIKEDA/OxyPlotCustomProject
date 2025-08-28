// エラーバーとマーカーを保持しつつ、ScatterErrorSeries の点を線で接続するカスタムシリーズ。
using OxyPlot;
using OxyPlot.Series;

namespace OxyPlotCustomProject
{
    /// <summary>
    /// ScatterErrorSeries を継承し、点を線で接続して描画するカスタムシリーズです。
    /// 有効な連続点間にセグメントとしてポリラインを描画し、マーカーとエラーバーの描画は基底クラスに委譲します。
    /// </summary>
    public class CustomScatterLineSeries : ScatterErrorSeries
    {
        /// <summary>
        /// 新しい <see cref="CustomScatterLineSeries"/> のインスタンスを初期化します。
        /// </summary>
        public CustomScatterLineSeries()
        {
            this.LineColor = OxyColors.Automatic;
            this.LineThickness = 1.0;
            this.LineJoin = LineJoin.Round;
        }

        /// <summary>
        /// 点を接続する線の色を取得または設定します。
        /// </summary>
        public OxyColor LineColor { get; set; }

        /// <summary>
        /// 接続線の太さを取得または設定します。
        /// </summary>
        public double LineThickness { get; set; }

        /// <summary>
        /// セグメント描画時の線の結合スタイルを取得または設定します。
        /// </summary>
        public LineJoin LineJoin { get; set; }

        /// <summary>
        /// シリーズを描画します: まず連続する点を接続する線を描き、その後基底クラスを呼んでエラーバーとマーカーを描画します。
        /// </summary>
        /// <param name="rc">描画コンテキスト。</param>
        public override void Render(IRenderContext rc)
        {
            // ActualPointsList を使って連続する有効な点を接続するセグメントを構築する（データ順序を保持）
            var actualPoints = this.ActualPointsList;
            if (actualPoints != null && actualPoints.Count > 0)
            {
                var segments = new List<ScreenPoint>();
                ScreenPoint? prev = null;

                foreach (var dp in actualPoints)
                {
                    if (dp == null)
                    {
                        prev = null;
                        continue;
                    }

                    // 無効な座標をスキップ
                    if (double.IsNaN(dp.X) || double.IsNaN(dp.Y))
                    {
                        prev = null;
                        continue;
                    }

                    var sp = this.Transform(dp.X, dp.Y);

                    if (prev.HasValue)
                    {
                        // 点が異なる場合のみセグメントを追加
                        if (!sp.Equals(prev.Value))
                        {
                            segments.Add(prev.Value);
                            segments.Add(sp);
                        }
                    }

                    prev = sp;
                }

                if (segments.Count > 0)
                {
                    rc.DrawLineSegments(
                        segments,
                        this.GetSelectableColor(this.LineColor),
                        this.LineThickness,
                        this.EdgeRenderingMode,
                        null,
                        this.LineJoin);
                }
            }

            // 基底クラスにエラーバーとマーカーの描画を任せる（上に重ねる）
            base.Render(rc);
        }
    }
}
