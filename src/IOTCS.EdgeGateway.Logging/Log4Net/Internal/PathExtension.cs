using System;
using System.Runtime.InteropServices;

namespace System.IO
{
    public static class PathExtension
    {
        public static String BasePath { get; set; }

        #region 静态构造
        static PathExtension()
        {
            var dir = "";
            // 命令参数
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].EqualIgnoreCase("-BasePath", "--BasePath") && i + 1 < args.Length)
                {
                    dir = args[i + 1];
                    break;
                }
            }

            // 环境变量
            if (string.IsNullOrEmpty(dir)) dir = Environment.GetEnvironmentVariable("BasePath");

            // 最终取应用程序域。Linux下编译为单文件时，应用程序释放到临时目录，应用程序域基路径不对，当前目录也不一定正确，唯有进程路径正确
            if (string.IsNullOrEmpty(dir)) dir = AppDomain.CurrentDomain.BaseDirectory;

            BasePath = GetPath(dir, 1);
        }
        #endregion

        #region 路径操作辅助
        private static String GetPath(String path, Int32 mode)
        {
            // 处理路径分隔符，兼容Windows和Linux
            var sep = Path.DirectorySeparatorChar;
            var sep2 = sep == '/' ? '\\' : '/';
            path = path.Replace(sep2, sep);

            var dir = "";
            switch (mode)
            {
                case 1:
                    dir = AppDomain.CurrentDomain.BaseDirectory;
                    break;
                case 2:
                    dir = BasePath;
                    break;
                case 3:
                    dir = Environment.CurrentDirectory;
                    break;
                default:
                    break;
            }
            if (string.IsNullOrEmpty(dir)) return Path.GetFullPath(path);

            // 处理网络路径
            if (path.StartsWith(@"\\", StringComparison.Ordinal)) return Path.GetFullPath(path);

            // 考虑兼容Linux
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {                
                //!!! 注意：不能直接依赖于Path.IsPathRooted判断，/和\开头的路径虽然是绝对路径，但是它们不是驱动器级别的绝对路径
                if (/*path[0] == sep ||*/ path[0] == sep2 || !Path.IsPathRooted(path))
                {
                    path = path.TrimStart('~');

                    path = path.TrimStart(sep);
                    path = Path.Combine(dir, path);
                }
            }
            else
            {
                if (!path.StartsWith(dir))
                {
                    // path目录存在，不用再次拼接
                    if (!Directory.Exists(path))
                    {
                        path = path.TrimStart(sep);
                        path = Path.Combine(dir, path);
                    }
                }
            }

            return Path.GetFullPath(path);
        }
       
        public static String GetFullPath(this String path)
        {
            if (String.IsNullOrEmpty(path)) return path;

            return GetPath(path, 1);
        }
        
        public static String GetBasePath(this String path)
        {
            if (String.IsNullOrEmpty(path)) return path;

            return GetPath(path, 2);
        }
       
        public static String GetCurrentPath(this String path)
        {
            if (String.IsNullOrEmpty(path)) return path;

            return GetPath(path, 3);
        }
        
        public static String EnsureDirectory(this String path, Boolean isfile = true)
        {
            if (String.IsNullOrEmpty(path)) return path;

            path = path.GetFullPath();
            if (File.Exists(path) || Directory.Exists(path)) return path;

            var dir = path;
            // 斜杠结尾的路径一定是目录，无视第二参数
            if (dir[dir.Length - 1] == Path.DirectorySeparatorChar)
                dir = Path.GetDirectoryName(path);
            else if (isfile)
                dir = Path.GetDirectoryName(path);

            if (!String.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            return path;
        }

        /// <summary>合并多段路径</summary>
        /// <param name="path"></param>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static String CombinePath(this String path, params String[] ps)
        {
            if (ps == null || ps.Length < 1) return path;
            if (path == null) path = String.Empty;

            //return Path.Combine(path, path2);
            foreach (var item in ps)
            {
                if (!string.IsNullOrEmpty(item)) path = Path.Combine(path, item);
            }
            return path;
        }
        #endregion

        /// <summary>忽略大小写的字符串相等比较，判断是否以任意一个待比较字符串相等</summary>
        /// <param name="value">字符串</param>
        /// <param name="strs">待比较字符串数组</param>
        /// <returns></returns>
        public static Boolean EqualIgnoreCase(this String value, params String[] strs)
        {
            foreach (var item in strs)
            {
                if (String.Equals(value, item, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }
    }
}
