using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace 倒计时.Controls
{
    /// <summary>Size a page's content to its scroll viewport before centering it.</summary>
    public static class CenteredScrollContent
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool),
                typeof(CenteredScrollContent), new PropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject element) =>
            (bool)element.GetValue(IsEnabledProperty);

        public static void SetIsEnabled(DependencyObject element, bool value) =>
            element.SetValue(IsEnabledProperty, value);

        private static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (!(sender is ScrollViewer viewer))
                return;

            if ((bool)args.NewValue)
            {
                viewer.HorizontalContentAlignment = HorizontalAlignment.Center;
                viewer.HorizontalScrollMode = ScrollMode.Disabled;
                viewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                viewer.Loaded += OnLoaded;
                viewer.SizeChanged += OnSizeChanged;
                UpdateWidth(viewer);
            }
            else
            {
                viewer.Loaded -= OnLoaded;
                viewer.SizeChanged -= OnSizeChanged;
            }
        }

        private static void OnLoaded(object sender, RoutedEventArgs args) => UpdateWidth((ScrollViewer)sender);

        private static void OnSizeChanged(object sender, SizeChangedEventArgs args) => UpdateWidth((ScrollViewer)sender);

        private static void UpdateWidth(ScrollViewer viewer)
        {
            if (viewer.ActualWidth <= 0 || !(viewer.Content is FrameworkElement content))
                return;

            // Use the page's actual available width, not window width: opening
            // or collapsing NavigationView changes this viewport independently.
            double availableWidth = Math.Max(0, viewer.ActualWidth
                - viewer.Padding.Left - viewer.Padding.Right
                - viewer.BorderThickness.Left - viewer.BorderThickness.Right
                - content.Margin.Left - content.Margin.Right);
            content.HorizontalAlignment = HorizontalAlignment.Center;
            content.Width = Math.Min(content.MaxWidth, availableWidth);
        }
    }
}
