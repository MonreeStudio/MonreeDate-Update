using System;
using MaterialLibs.Controls;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace 倒计时.Controls
{
    // Attach to the animated template surface, not the dismiss layer.
    public static class FloatingPlanCard
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(FloatingPlanCard),
                new PropertyMetadata(false, OnEnabledChanged));
        private static readonly DependencyProperty StateProperty =
            DependencyProperty.RegisterAttached("State", typeof(object), typeof(FloatingPlanCard), new PropertyMetadata(null));
        public static bool GetIsEnabled(DependencyObject element) => (bool)element.GetValue(IsEnabledProperty);
        public static void SetIsEnabled(DependencyObject element, bool value) => element.SetValue(IsEnabledProperty, value);

        private static void OnEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (!(sender is CardView card)) return;
            if ((bool)args.NewValue)
            {
                card.Loaded += OnLoaded;
                card.Unloaded += OnUnloaded;
                if (card.IsLoaded) OnLoaded(card, null);
            }
            else
            {
                card.Loaded -= OnLoaded;
                card.Unloaded -= OnUnloaded;
                OnUnloaded(card, null);
            }
        }
        private static void OnLoaded(object sender, RoutedEventArgs args)
        {
            var card = (CardView)sender;
            if (card.GetValue(StateProperty) == null) card.SetValue(StateProperty, new State(card));
        }
        private static void OnUnloaded(object sender, RoutedEventArgs args)
        {
            var card = (CardView)sender;
            (card.GetValue(StateProperty) as State)?.Dispose();
            card.ClearValue(StateProperty);
        }
        private static FrameworkElement Find(DependencyObject parent, string name)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement element && element.Name == name) return element;
                var match = Find(child, name);
                if (match != null) return match;
            }
            return null;
        }
        private sealed class State : System.IDisposable
        {
            private readonly CardView card;
            private readonly AccessibilitySettings accessibility = new AccessibilitySettings();
            private Grid host;
            private FrameworkElement surface;
            private OuterCardShadow shadow;
            private bool disposed;
            public State(CardView card)
            {
                this.card = card;
                card.ApplyTemplate();
                surface = Find(card, "BorderBackground");
                if (Find(card, "ContentGrid") is Grid grid && surface != null)
                {
                    host = new Grid { IsHitTestVisible = false };
                    Grid.SetRowSpan(host, 3);
                    grid.Children.Insert(0, host);
                }
                accessibility.HighContrastChanged += OnContrastChanged;
                Update();
            }
            private void Update()
            {
                shadow?.Dispose();
                shadow = null;
                if (!disposed && host != null && !accessibility.HighContrast)
                    shadow = new OuterCardShadow(surface, host, subtle: true);
            }
            private async void OnContrastChanged(AccessibilitySettings sender, object args)
            {
                await card.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { if (!disposed) Update(); });
            }
            public void Dispose()
            {
                disposed = true;
                accessibility.HighContrastChanged -= OnContrastChanged;
                shadow?.Dispose();
                if (host?.Parent is Panel parent) parent.Children.Remove(host);
            }
        }
    }
}
