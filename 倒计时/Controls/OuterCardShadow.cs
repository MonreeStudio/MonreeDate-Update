using System;
using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Composition;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace 倒计时.Controls
{
    /// <summary>A rounded shadow with the entire card interior cut out.</summary>
    internal sealed class OuterCardShadow : IDisposable
    {
        private const float Inset = 24;
        private readonly FrameworkElement element;
        private readonly FrameworkElement host;
        private readonly Compositor compositor;
        private readonly CompositionGraphicsDevice graphics;
        private readonly SpriteVisual visual;
        private readonly DropShadow shadow;
        private CompositionDrawingSurface surface;
        private CompositionSurfaceBrush mask;
        private CompositionPathGeometry clipGeometry;
        private CompositionGeometricClip clip;

        public OuterCardShadow(FrameworkElement element, FrameworkElement host, bool subtle = false)
        {
            this.element = element;
            this.host = host;
            compositor = ElementCompositionPreview.GetElementVisual(element).Compositor;
            graphics = CanvasComposition.CreateCompositionGraphicsDevice(compositor, CanvasDevice.GetSharedDevice());
            graphics.RenderingDeviceReplaced += OnDeviceReplaced;
            visual = compositor.CreateSpriteVisual();
            visual.Offset = new Vector3(-Inset, -Inset, 0);
            shadow = compositor.CreateDropShadow();
            shadow.Color = Colors.Black;
            shadow.BlurRadius = subtle ? 10 : 12;
            shadow.Opacity = subtle ? 0.08f : 0.12f;
            shadow.Offset = new Vector3(0, 2, 0);
            visual.Shadow = shadow;
            // Attach to the wrapper so the card's rounded child clip cannot
            // cut away the shadow that extends beyond its own bounds.
            ElementCompositionPreview.SetElementChildVisual(host, visual);
            element.SizeChanged += OnSizeChanged;
            Update();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs args) => Update();
        private void OnDeviceReplaced(CompositionGraphicsDevice sender, RenderingDeviceReplacedEventArgs args) => Update();

        private void Update()
        {
            float width = (float)element.ActualWidth, height = (float)element.ActualHeight;
            if (width <= 0 || height <= 0) return;
            Point origin = element.TransformToVisual(host).TransformPoint(new Point(0, 0));
            visual.Offset = new Vector3((float)origin.X - Inset, (float)origin.Y - Inset, 0);
            float radius = 8;
            if (element is Grid grid) radius = (float)grid.CornerRadius.TopLeft;
            else if (element is StackPanel stack) radius = (float)stack.CornerRadius.TopLeft;
            else if (element is Border border) radius = (float)border.CornerRadius.TopLeft;
            else if (element is Windows.UI.Xaml.Shapes.Rectangle rectangle) radius = (float)rectangle.RadiusX;
            radius = Math.Min(radius, Math.Min(width, height) / 2);
            var size = new Size(Math.Ceiling(width + Inset * 2), Math.Ceiling(height + Inset * 2));
            visual.Size = new Vector2((float)size.Width, (float)size.Height);

            if (surface == null)
            {
                surface = graphics.CreateDrawingSurface(size, DirectXPixelFormat.B8G8R8A8UIntNormalized,
                    DirectXAlphaMode.Premultiplied);
                mask = compositor.CreateSurfaceBrush(surface);
                shadow.Mask = mask;
            }
            else surface.Resize(new Windows.Graphics.SizeInt32 { Width = (int)size.Width, Height = (int)size.Height });
            using (var drawing = CanvasComposition.CreateDrawingSession(surface))
            {
                drawing.Clear(Colors.Transparent);
                drawing.FillRoundedRectangle(Inset, Inset, width, height, radius, radius, Colors.White);
            }

            // Clip after the shadow is blurred: no black pixels remain under the
            // translucent card face, even when the shadow is offset downward.
            using (var outer = CanvasGeometry.CreateRectangle(null, 0, 0, (float)size.Width, (float)size.Height))
            using (var inner = CanvasGeometry.CreateRoundedRectangle(null, Inset, Inset, width, height, radius, radius))
            using (var ring = outer.CombineWith(inner, Matrix3x2.Identity, CanvasGeometryCombine.Exclude))
            {
                if (clipGeometry == null)
                {
                    clipGeometry = compositor.CreatePathGeometry();
                    clip = compositor.CreateGeometricClip(clipGeometry);
                    visual.Clip = clip;
                }
                clipGeometry.Path = new CompositionPath(ring);
            }
        }

        public void Dispose()
        {
            element.SizeChanged -= OnSizeChanged;
            graphics.RenderingDeviceReplaced -= OnDeviceReplaced;
            ElementCompositionPreview.SetElementChildVisual(host, null);
            visual.Dispose();
            shadow.Dispose();
            clip?.Dispose();
            clipGeometry?.Dispose();
            mask?.Dispose();
            surface?.Dispose();
            graphics.Dispose();
        }
    }
}
