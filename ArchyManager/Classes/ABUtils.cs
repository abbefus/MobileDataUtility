using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ABUtils
{
    public static class FileUtils
    {
        public static string Increment(string path, int pad=2)
        {
            int count = 1;
            string ext = Path.GetExtension(path);
            string padding = string.Format("D{0}", pad);

            int pathlen = path.Length - ext.Length;
            while (File.Exists(path))
            {
                path = string.Format("{0}_{1}{2}", path.Substring(0, pathlen), count.ToString(padding), ext);
                count++;
            }

            return path;
        }
    }
    public static class Utils
    {
        public static T GetVisualChild<T>(DependencyObject parent) where T : Visual
        {
            T child = default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                if (VisualTreeHelper.GetChild(parent, i) is T)
                {
                    child = VisualTreeHelper.GetChild(parent, i) as T;
                    break;
                }
            }
            return child;
        }

        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        public static bool IsAllNumbers(string s)
        {
            return Regex.IsMatch(s, @"^\d+$");
        }
        public static void DelayAction(int millisecond, Action action)
        {
            var timer = new DispatcherTimer();
            timer.Tick += delegate

            {
                action.Invoke();
                timer.Stop();
            };

            timer.Interval = TimeSpan.FromMilliseconds(millisecond);
            timer.Start();
        }
    }
}
