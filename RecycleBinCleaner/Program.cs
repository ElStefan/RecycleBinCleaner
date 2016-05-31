using Shell32;
using System;
using System.IO;

namespace RecycleBinCleaner
{
    internal static class Program
    {
        [STAThread]
        public static void Main()
        {
            var shell = new Shell();

            var recycler = shell.NameSpace(10);
            var items = recycler.Items();
            var exceptionOccured = false;
            for (int i = 0; i < items.Count; i++)
            {
                var path = "";
                var isFolder = false;
                try
                {
                    var folderItem = items.Item(i);
                    var fileName = recycler.GetDetailsOf(folderItem, 0);
                    var recycleDateString = recycler.GetDetailsOf(folderItem, 2);
                    var recycleSplitDate = recycleDateString.Split(new char[] { '?', '.', ' ', '\x200e', '\x200f' }, StringSplitOptions.RemoveEmptyEntries);

                    path = folderItem.Path;
                    isFolder = folderItem.IsFolder;

                    if (recycleSplitDate.Length == 4)
                    {
                        var year = Convert.ToInt32(recycleSplitDate[2]);
                        var month = Convert.ToInt32(recycleSplitDate[1]);
                        var day = Convert.ToInt32(recycleSplitDate[0]);
                        var recycleDate = new DateTime(year, month, day);

                        if (recycleDate.AddDays(7) < DateTime.Now)
                        {
                            Console.WriteLine("Item - Name: {0}\nPath: {1}\nIsFolder:{2}\n\n", fileName, path, isFolder);
                            if (isFolder)
                            {
                                if (path.EndsWith(".zip"))
                                {
                                    File.Delete(path);
                                    continue;
                                }
                                var tempPath = path[0] + @":\RecycleBinCleaner";
                                if (Directory.Exists(tempPath))
                                {
                                    Directory.Delete(tempPath, true);
                                }
                                Directory.Move(path, tempPath);
                                Directory.Delete(tempPath, true);
                            }
                            else
                            {
                                File.Delete(path);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Failed to delete: {0}, IsFolder: {1}", path, isFolder);
                    Console.WriteLine(exception);
                    Console.WriteLine();
                    exceptionOccured = true;
                }
            }
            if (exceptionOccured)
            {
                Console.ReadLine();
            }
        }
    }
}