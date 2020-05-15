using System;
using System.IO;

namespace COD_Teamspeak.Helpers
{
    public class FileMonitor
    {
        DateTime _lastChange;
        public FileSystemEventHandler FileChanged { get; set; }

        public FileMonitor(string file)
        {
            _lastChange = DateTime.MinValue;
            if(!File.Exists(file)) return;
            var watcher = new FileSystemWatcher(Path.GetDirectoryName(file) ?? throw new InvalidOperationException(), Path.GetFileName(file))
            {
                EnableRaisingEvents = true
            };
            //_watcher.Changed += watcher_Changed;
            watcher.Changed += WatcherChanged;
        }

        private void WatcherChanged(object sender, FileSystemEventArgs e)
        {
            if((DateTime.Now - _lastChange).Seconds <= 5 ) return;
            System.Threading.Thread.Sleep(500);
            FileChanged?.Invoke(sender, e);
            _lastChange = DateTime.Now;
        }

        private static bool IsFileLocked(string filePath)
        {
            FileStream stream = null;
            try
            {
                stream = File.OpenRead(filePath);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                stream?.Close();
            }
            return false;
        }
    }
}
