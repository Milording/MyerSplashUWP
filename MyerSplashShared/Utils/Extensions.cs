﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MyerSplashShared.Utils
{
    public static class Extensions
    {
        public static IList<T> Append<T>(this IList<T> list, T t)
        {
            list.Add(t);
            return list;
        }

        public static async Task WaitForNonZeroSizeAsync(this FrameworkElement frameworkElement)
        {
            if (frameworkElement == null)
            {
                throw new ArgumentNullException(nameof(frameworkElement));
            }

            while (frameworkElement.ActualWidth == 0 && frameworkElement.ActualHeight == 0)
            {
                var tcs = new TaskCompletionSource<object>();

                SizeChangedEventHandler handler = null;

                handler = (sender, e) =>
                {
                    frameworkElement.SizeChanged -= handler;
                    tcs.SetResult(null);
                };

                frameworkElement.SizeChanged += handler;

                await tcs.Task;
            }
        }

    }
}
