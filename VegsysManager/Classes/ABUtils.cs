﻿using System.IO;

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
}
