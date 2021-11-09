using System;
using System.Collections.Generic;
using System.IO;
using osu.Framework.Platform;

namespace Circle.Game.IO
{
    public class WrappedStorage : Storage
    {
        protected Storage UnderlyingStorage { get; private set; }

        private readonly string subPath;

        public WrappedStorage(Storage storage, string subPath = null)
            : base(string.Empty)
        {
            UnderlyingStorage = storage;
            this.subPath = subPath;
        }

        protected virtual string MutatePath(string path)
        {
            if (path == null)
                return null;

            return !string.IsNullOrEmpty(subPath) ? Path.Combine(subPath, path) : path;
        }

        public IEnumerable<string> ToLocalRelative(IEnumerable<string> paths)
        {
            string localRoot = GetFullPath(string.Empty);

            foreach (var path in paths)
                yield return Path.GetRelativePath(localRoot, UnderlyingStorage.GetFullPath(path));
        }

        public override string GetFullPath(string path, bool createIfNotExisting = false)
            => UnderlyingStorage.GetFullPath(MutatePath(path), createIfNotExisting);

        public override bool Exists(string path)
            => UnderlyingStorage.Exists(MutatePath(path));

        public override bool ExistsDirectory(string path)
            => UnderlyingStorage.ExistsDirectory(MutatePath(path));

        public override void DeleteDirectory(string path)
            => UnderlyingStorage.DeleteDirectory(MutatePath(path));

        public override void Delete(string path)
            => UnderlyingStorage.Delete(MutatePath(path));

        public override IEnumerable<string> GetDirectories(string path)
            => ToLocalRelative(UnderlyingStorage.GetDirectories(MutatePath(path)));

        public override IEnumerable<string> GetFiles(string path, string pattern = "*")
            => ToLocalRelative(UnderlyingStorage.GetFiles(MutatePath(path), pattern));

        public override Stream GetStream(string path, FileAccess access = FileAccess.Read, FileMode mode = FileMode.OpenOrCreate)
            => UnderlyingStorage.GetStream(MutatePath(path), access, mode);

        public override void OpenFileExternally(string filename)
            => UnderlyingStorage.OpenFileExternally(filename);

        public override void PresentFileExternally(string filename)
            => UnderlyingStorage.PresentFileExternally(filename);

        public override Storage GetStorageForDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Must be non-null and not empty string", nameof(path));

            if (!path.EndsWith(Path.DirectorySeparatorChar))
                path += Path.DirectorySeparatorChar;

            // create non-existing path.
            GetFullPath(path, true);

            return new WrappedStorage(this, path);
        }
    }
}
