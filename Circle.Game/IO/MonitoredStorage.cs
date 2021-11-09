using System;
using System.IO;
using osu.Framework.Bindables;
using osu.Framework.Platform;

namespace Circle.Game.IO
{
    public class MonitoredStorage : WrappedStorage, IDisposable
    {
        public readonly BindableBool Enabled = new BindableBool(true);

        public readonly BindableList<string> Files = new BindableList<string>();

        public event Action<string> FileCreated;

        public event Action<string> FileDeleted;

        public event Action<string> FileUpdated;

        public event Action<string, string> FileRenamed;

        public bool IncludeSubPaths
        {
            get => watcher.IncludeSubdirectories;
            set => watcher.IncludeSubdirectories = value;
        }

        private readonly FileSystemWatcher watcher;

        public MonitoredStorage(Storage underlyingStorage)
            : base(underlyingStorage)
        {
            watcher = new FileSystemWatcher
            {
                Path = GetFullPath(string.Empty, true),
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size,
            };

            watcher.Created += (_, e) => OnFileCreated(e.Name);
            watcher.Deleted += (_, e) => OnFileDeleted(e.Name);
            watcher.Changed += (_, e) => OnFileUpdated(e.Name);
            watcher.Renamed += (_, e) => OnFileRenamed(e.OldName, e.Name);

            Enabled.BindValueChanged(e => watcher.EnableRaisingEvents = e.NewValue, true);
        }

        protected void OnFileCreated(string name, bool invoke = true)
        {
            if (Files.Contains(name))
                return;

            Files.Add(name);

            if (invoke)
                FileCreated?.Invoke(name);
        }

        protected void OnFileDeleted(string name, bool invoke = true)
        {
            if (!Files.Contains(name))
                return;

            Files.Remove(name);

            if (invoke)
                FileDeleted?.Invoke(name);
        }

        protected void OnFileUpdated(string name)
        {
            OnFileDeleted(name, false);
            OnFileCreated(name, false);
            FileUpdated?.Invoke(name);
        }

        protected void OnFileRenamed(string oldName, string newName)
        {
            OnFileDeleted(oldName, false);
            OnFileCreated(newName, false);
            FileRenamed?.Invoke(oldName, newName);
        }

        private bool isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                    watcher.Dispose();

                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
