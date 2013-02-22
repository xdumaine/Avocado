﻿using WinRTXamlToolkit.Controls.Extensions;
using Windows.UI.Xaml;
using Rectangle = Windows.UI.Xaml.Shapes.Rectangle;

namespace WinRTXamlToolkit.Composition.Renderers
{
    public static class RectangleRenderer
    {
        internal static void Render(CompositionEngine compositionEngine, SharpDX.Direct2D1.RenderTarget renderTarget, FrameworkElement rootElement, Rectangle rectangle)
        {
            var rect = rectangle.GetBoundingRect(rootElement).ToSharpDX();
            var fill = rectangle.Fill.ToSharpDX(renderTarget, rect);
            var stroke = rectangle.Stroke.ToSharpDX(renderTarget, rect);

            //var layer = new Layer(renderTarget);
            //var layerParameters = new LayerParameters();
            //layerParameters.ContentBounds = rect;
            //renderTarget.PushLayer(ref layerParameters, layer);

            if (rectangle.RadiusX > 0 &&
                rectangle.RadiusY > 0)
            {
                var roundedRect = new SharpDX.Direct2D1.RoundedRect();
                roundedRect.Rect = rect;
                roundedRect.RadiusX = (float)rectangle.RadiusX;
                roundedRect.RadiusY = (float)rectangle.RadiusY;

                if (rectangle.StrokeThickness > 0 &&
                    stroke != null)
                {
                    var halfThickness = (float)(rectangle.StrokeThickness * 0.5);
                    roundedRect.Rect = rect.Eroded(halfThickness);

                    if (fill != null)
                    {
                        renderTarget.FillRoundedRectangle(roundedRect, fill);
                    }

                    renderTarget.DrawRoundedRectangle(
                        roundedRect,
                        stroke,
                        (float)rectangle.StrokeThickness,
                        rectangle.GetStrokeStyle(compositionEngine.D2DFactory));
                }
                else
                {
                    renderTarget.FillRoundedRectangle(roundedRect, fill);
                }
            }
            else
            {
                if (rectangle.StrokeThickness > 0 &&
                    stroke != null)
                {
                    var halfThickness = (float)(rectangle.StrokeThickness * 0.5);

                    if (fill != null)
                    {
                        renderTarget.FillRectangle(rect.Eroded(halfThickness), fill);
                    }

                    var strokeRect = rect.Eroded(halfThickness);
                    renderTarget.DrawRectangle(
                        strokeRect,
                        stroke,
                        (float)rectangle.StrokeThickness,
                        rectangle.GetStrokeStyle(compositionEngine.D2DFactory));
                }
                else
                {
                    renderTarget.FillRectangle(rect, fill);
                }
            }
            //renderTarget.PopLayer();
        }
    }
}
