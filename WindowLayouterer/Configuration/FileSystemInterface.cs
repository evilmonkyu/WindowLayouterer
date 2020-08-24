using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WindowLayouterer.Configuration
{
    public class FileInterface
    {
        public virtual string ReadAllText(string filename)
        {
            return File.ReadAllText(filename);
        }

        public virtual bool Exists(string filename)
        {
            return File.Exists(filename);
        }
    }

    public class FileSystemWatcherInterface : IDisposable
    {
        public FileSystemWatcherInterface(string directory, string filter)
        { 
        }

        public virtual event FileSystemEventHandler Changed;

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
