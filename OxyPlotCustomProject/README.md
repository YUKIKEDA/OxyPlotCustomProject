# CustomScatterLineSeries

このプロジェクトには、誤差線付きの散布図点を線で繋ぐカスタムシリーズ `CustomScatterLineSeries` を追加しています。

主な特徴:
- `ScatterErrorSeries` を継承し、点同士を線で接続します。
- 誤差線（Error bars）とマーカーは親クラスの描画をそのまま使用します。
- 線の色・太さ・接合スタイルをプロパティで設定できます。

簡単な使用例 (WPF):

MainWindow.xaml の PlotView に直接シリーズを追加するか、コードビハインドで作成して `PlotModel.Series.Add(...)` してください。

例 (コードビハインド):

```csharp
using OxyPlot;
using OxyPlot.Wpf;

var model = new PlotModel { Title = "CustomScatterLineSeries Example" };

var series = new OxyPlotCustomProject.CustomScatterLineSeries
{
    LineColor = OxyColors.SteelBlue,
    LineThickness = 1.5,
    MarkerType = MarkerType.Circle,
    MarkerSize = 4
};

// 代入方法: ScatterErrorPoint(x, y, errorX, errorY, size, value, tag)
series.Points.Add(new ScatterErrorPoint(0, 0, 0.5, 0.2, 4, 0, null));
series.Points.Add(new ScatterErrorPoint(1, 1, 0.3, 0.3, 4, 0, null));
series.Points.Add(new ScatterErrorPoint(2, 0.5, 0.2, 0.4, 4, 0, null));

model.Series.Add(series);

var pv = new PlotView { Model = model };
// PlotView をウィンドウに配置してください
```
