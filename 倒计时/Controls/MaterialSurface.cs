using System;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace 倒计时.Controls
{
    /// <summary>Applies the selected material directly to each live card surface.</summary>
    public static class MaterialSurface
    {
        private static event EventHandler MaterialChanged;

        public static void Refresh() => MaterialChanged?.Invoke(null, EventArgs.Empty);

        // Floating editors cannot expose the page underneath as a Mica layer.
        public static readonly DependencyProperty IsOverlayProperty =
            DependencyProperty.RegisterAttached("IsOverlay", typeof(bool), typeof(MaterialSurface),
                new PropertyMetadata(false));
        public static bool GetIsOverlay(DependencyObject element) => (bool)element.GetValue(IsOverlayProperty);
        public static void SetIsOverlay(DependencyObject element, bool value) => element.SetValue(IsOverlayProperty, value);

        public static readonly DependencyProperty SubtleShadowProperty =
            DependencyProperty.RegisterAttached("SubtleShadow", typeof(bool), typeof(MaterialSurface),
                new PropertyMetadata(false));
        public static bool GetSubtleShadow(DependencyObject element) => (bool)element.GetValue(SubtleShadowProperty);
        public static void SetSubtleShadow(DependencyObject element, bool value) => element.SetValue(SubtleShadowProperty, value);

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(MaterialSurface),
                new PropertyMetadata(false, OnIsEnabledChanged));

        private static readonly DependencyProperty SubscriptionProperty =
            DependencyProperty.RegisterAttached("Subscription", typeof(object), typeof(MaterialSurface),
                new PropertyMetadata(null));

        public static bool GetIsEnabled(DependencyObject element) => (bool)element.GetValue(IsEnabledProperty);
        public static void SetIsEnabled(DependencyObject element, bool value) => element.SetValue(IsEnabledProperty, value);

        private static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (!(sender is FrameworkElement element)) return;
            if ((bool)args.NewValue)
            {
                element.Loaded += OnLoaded;
                element.Unloaded += OnUnloaded;
                if (element.IsLoaded) OnLoaded(element, null);
            }
            else
            {
                element.Loaded -= OnLoaded;
                element.Unloaded -= OnUnloaded;
                OnUnloaded(element, null);
            }
        }

        private static void OnLoaded(object sender, RoutedEventArgs args)
        {
            var element = (FrameworkElement)sender;
            if (element.GetValue(SubscriptionProperty) == null)
                element.SetValue(SubscriptionProperty, new Subscription(element));
            Apply(element);
        }

        private static void OnUnloaded(object sender, RoutedEventArgs args)
        {
            var element = (FrameworkElement)sender;
            (element.GetValue(SubscriptionProperty) as Subscription)?.Dispose();
            element.ClearValue(SubscriptionProperty);
        }

        private static void Apply(FrameworkElement element)
        {
            bool mica = ApplicationData.Current.LocalSettings.Values["MaterialStyle"]?.ToString() == "Mica";
            bool dark = element.ActualTheme == ElementTheme.Dark;
            Color tint = dark ? Color.FromArgb(255, 43, 43, 43) : Colors.White;
            var accessibility = new AccessibilitySettings();
            Brush background;
            if (accessibility.HighContrast)
                background = new SolidColorBrush(new UISettings().GetColorValue(UIColorType.Background));
            else if (!mica)
                background = new AcrylicBrush { BackgroundSource = AcrylicBackgroundSource.Backdrop,
                    TintColor = tint, TintOpacity = 0.85, FallbackColor = tint };
            else if (GetIsOverlay(element))
                background = new SolidColorBrush(tint);
            else if (SystemInformation.OperatingSystemVersion.Build >= 22000)
                background = new SolidColorBrush(dark ? Color.FromArgb(20, 255, 255, 255)
                    : Color.FromArgb(100, 255, 255, 255));
            else
                background = new SolidColorBrush(tint);

            if (element is Panel panel) panel.Background = background;
            else if (element is Border border) border.Background = background;
            else if (element is Control control) control.Background = background;

            // The material surface is the content of the outer card shadow.
            // Do not change shadows belonging to nested controls or other cards.
            DropShadowPanel outerShadowHost = null;
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            while (parent != null)
            {
                if (parent is DropShadowPanel shadow)
                {
                    if (ReferenceEquals(shadow.Content, element))
                    {
                        bool exteriorOnly = mica || GetSubtleShadow(element);
                        if (exteriorOnly && !accessibility.HighContrast) outerShadowHost = shadow;
                        shadow.ShadowOpacity = accessibility.HighContrast || exteriorOnly ? 0 : 0.16;
                        shadow.BlurRadius = 16;
                        shadow.OffsetY = 4;
                    }
                    break;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            (element.GetValue(SubscriptionProperty) as Subscription)?.SetOuterShadow(outerShadowHost);
        }

        private sealed class Subscription : IDisposable
        {
            private readonly FrameworkElement element;
            private readonly AccessibilitySettings accessibility = new AccessibilitySettings();
            private bool disposed;
            private OuterCardShadow outerShadow;

            public void SetOuterShadow(DropShadowPanel host)
            {
                if (host != null && outerShadow == null) outerShadow = new OuterCardShadow(element, host, GetSubtleShadow(element));
                else if (host == null && outerShadow != null)
                {
                    outerShadow.Dispose();
                    outerShadow = null;
                }
            }

            public Subscription(FrameworkElement element)
            {
                this.element = element;
                MaterialChanged += OnChanged;
                element.ActualThemeChanged += OnThemeChanged;
                accessibility.HighContrastChanged += OnHighContrastChanged;
            }

            private void OnChanged(object sender, EventArgs args) => Update();
            private void OnThemeChanged(FrameworkElement sender, object args) => Update();
            private void OnHighContrastChanged(AccessibilitySettings sender, object args) => Update();

            private async void Update()
            {
                if (disposed) return;
                if (element.Dispatcher.HasThreadAccess) Apply(element);
                else await element.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (!disposed) Apply(element);
                });
            }

            public void Dispose()
            {
                disposed = true;
                SetOuterShadow(null);
                MaterialChanged -= OnChanged;
                element.ActualThemeChanged -= OnThemeChanged;
                accessibility.HighContrastChanged -= OnHighContrastChanged;
            }
        }
    }
}
