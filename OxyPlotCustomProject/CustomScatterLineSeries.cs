// Custom series that connects ScatterErrorSeries points with lines while keeping error bars and markers.
using System;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Series;

namespace OxyPlotCustomProject
{
    /// <summary>
    /// A custom series that inherits from ScatterErrorSeries and draws connecting lines between points.
    /// It renders a polyline (as segments) between consecutive valid points, then defers to base for markers and error bars.
    /// </summary>
    public class CustomScatterLineSeries : ScatterErrorSeries
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomScatterLineSeries"/> class.
        /// </summary>
        public CustomScatterLineSeries()
        {
            this.LineColor = OxyColors.Automatic;
            this.LineThickness = 1.0;
            this.LineJoin = LineJoin.Round;
        }

        /// <summary>
        /// Gets or sets the line color used to connect the points.
        /// </summary>
        public OxyColor LineColor { get; set; }

        /// <summary>
        /// Gets or sets the thickness of the connecting line.
        /// </summary>
        public double LineThickness { get; set; }

        /// <summary>
        /// Gets or sets the line join style when drawing segments.
        /// </summary>
        public LineJoin LineJoin { get; set; }

        /// <summary>
        /// Renders the series: first draws connecting lines between consecutive points, then calls base to draw error bars and markers.
        /// </summary>
        /// <param name="rc">The rendering context.</param>
        public override void Render(IRenderContext rc)
        {
            // Build segments connecting consecutive valid points using the ActualPointsList (preserves data order)
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

                    // Skip invalid coordinates
                    if (double.IsNaN(dp.X) || double.IsNaN(dp.Y))
                    {
                        prev = null;
                        continue;
                    }

                    var sp = this.Transform(dp.X, dp.Y);

                    if (prev.HasValue)
                    {
                        // Only add segment if points are distinct
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

            // Let the base class draw error bars and markers on top
            base.Render(rc);
        }
    }
}
