using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace 倒计时.Controls
{
    public sealed class CalendarViewDayLunarItem : Control
    {
        public CalendarViewDayLunarItem()
        {
            this.DefaultStyleKey = typeof(CalendarViewDayLunarItem);

        }

        public string DateValue
        {
            get { return (string)GetValue(DateValueProperty); }
            set { SetValue(DateValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DateValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateValueProperty =
            DependencyProperty.Register("DateValue", typeof(string), typeof(CalendarViewDayLunarItem), new PropertyMetadata(string.Empty));

        public DateTimeOffset Date
        {
            get { return (DateTimeOffset)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(DateTimeOffset), typeof(CalendarViewDayLunarItem), new PropertyMetadata(DateTimeOffset.MinValue, DatePropertyChanged));

        private static void DatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CalendarViewDayLunarItem sender)
            {
                if (e.NewValue != e.OldValue)
                {
                    sender.DateValue = LunarCalendarConverter.GetDayOrTermString((DateTimeOffset)e.NewValue);
                }
            }
        }
    }
}
