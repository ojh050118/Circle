using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Circle.Game.Converting.Adofai;
using Circle.Game.Converting.Circle;
using Circle.Game.Converting.Json;
using Circle.Game.IO;
using Circle.Game.IO.Archives;
using Circle.Game.Utils;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using SharpCompress.Archives;

namespace Circle.Game.Beatmaps
{
    public class BeatmapManager : IWorkingBeatmapCache
    {
        public ITrackStore BeatmapTrackStore { get; }

        private readonly Storage storage;

        private readonly WorkingBeatmapCache workingBeatmapCache;

        public IWorkingBeatmap DefaultBeatmap => workingBeatmapCache.DefaultBeatmap;

        private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IndentCharacter = '\t',
            IndentSize = 1,
            Converters = { new JsonStringEnumConverter(), new FloatToIntConverter() }
        };

        public BeatmapManager(Storage files, AudioManager audioManager, IResourceStore<byte[]> gameResources, GameHost? host = null, WorkingBeatmap? defaultBeatmap = null)
        {
            storage = files;

            var userResources = new StorageBackedResourceStore(files);
            BeatmapTrackStore = audioManager.GetTrackStore(userResources);

            workingBeatmapCache = CreateWorkingBeatmapCache(audioManager, gameResources, userResources, defaultBeatmap, host);
        }

        protected virtual WorkingBeatmapCache CreateWorkingBeatmapCache(AudioManager audioManager, IResourceStore<byte[]> resources, IResourceStore<byte[]> storage, WorkingBeatmap? defaultBeatmap,
                                                                        GameHost? host)
        {
            return new WorkingBeatmapCache(BeatmapTrackStore, audioManager, resources, storage, defaultBeatmap, host);
        }

        public WorkingBeatmap GetWorkingBeatmap(BeatmapInfo? beatmapInfo, bool refetch = false)
        {
            if (beatmapInfo != null)
            {
                if (refetch)
                    workingBeatmapCache.Invalidate(beatmapInfo);
            }

            return workingBeatmapCache.GetWorkingBeatmap(beatmapInfo);
        }

        WorkingBeatmap IWorkingBeatmapCache.GetWorkingBeatmap(BeatmapInfo beatmapInfo) => GetWorkingBeatmap(beatmapInfo);
        void IWorkingBeatmapCache.Invalidate(BeatmapSetInfo beatmapSetInfo) => workingBeatmapCache.Invalidate(beatmapSetInfo);
        void IWorkingBeatmapCache.Invalidate(BeatmapInfo beatmapInfo) => workingBeatmapCache.Invalidate(beatmapInfo);

        public IEnumerable<BeatmapInfo> GetAvailableBeatmaps()
        {
            List<BeatmapInfo> beatmaps = new List<BeatmapInfo>();

            foreach (string dir in storage.GetDirectories(string.Empty))
            {
                var di = new DirectoryInfo(storage.GetFullPath(dir));
                foreach (var file in di.GetFiles("*.circle"))
                    beatmaps.Add(new BeatmapInfo(file));
            }

            return beatmaps;
        }

        public void Save(BeatmapInfo beatmapInfo, Beatmap beatmap)
        {
            beatmap.BeatmapInfo = beatmapInfo;

            string json = JsonSerializer.Serialize(beatmap, serializerOptions);

            string fileName = $"[{beatmap.Metadata.Author}] {beatmap.Metadata.Artist} - {beatmap.Metadata.Song}.circle";
            string path = storage.GetStorageForDirectory(Path.GetFileNameWithoutExtension(fileName)).GetFullPath(string.Empty);

            using (StreamWriter sw = File.CreateText(Path.Combine(path, fileName)))
                sw.WriteLine(json);
        }

        public bool Delete(BeatmapInfo beatmap)
        {
            if (beatmap.File == null || !beatmap.File.Exists)
                return false;

            // TODO: 비트맵세트에 2개이상의 비트맵이 있을 때, 파일만 삭제하고, 1개만 남아있을 때 폴더삭제
            beatmap.File.Delete();
            return true;
        }

        public void Import(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path).Trim(' ');
            string beatmap = storage.GetStorageForDirectory(fileName).GetFullPath(string.Empty);

            if (Path.GetExtension(path).Equals(@".circle", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    File.Copy(path, Path.Combine(beatmap, Path.GetFileName(path)), true);
                    OnImported?.Invoke(Path.GetFileName(path));
                }
                catch (Exception e)
                {
                    Logger.Log($"Error during import {Path.GetFileName(path)}: {e.Message}");
                }

                return;
            }

            // .circlez
            using (var reader = new ZipArchiveReader(File.Open(path, FileMode.Open, FileAccess.Read), Path.GetFileName(path)))
            {
                reader.Archive.WriteToDirectory(beatmap);
                OnImported?.Invoke(Path.GetFileName(path));
            }
        }

        public void Import(Stream? stream, string name)
        {
            if (stream == null)
            {
                Logger.Log($"Error when importing local beatmap: {name}");
                return;
            }

            string beatmap = storage.GetStorageForDirectory(name).GetFullPath(string.Empty);

            using (var reader = new ZipArchiveReader(stream))
            {
                reader.Archive.WriteToDirectory(beatmap);

                OnImported?.Invoke(name);
            }
        }

        public void ConvertWithImport(DirectoryInfo[] target, Bindable<int>? progress = null)
        {
            var adofaiFileReader = new AdofaiFileReader();
            var converter = new CircleBeatmapConverter();

            progress ??= new Bindable<int>();

            foreach (var level in target)
            {
                foreach (var adofai in level.GetFiles("*.adofai"))
                {
                    if (adofai.Name == "backup.adofai")
                        continue;

                    AdofaiBeatmap adofaiBeatmap;

                    try
                    {
                        Logger.Log($"Started parsing {adofai.FullName} for convert...");
                        adofaiBeatmap = adofaiFileReader.Get(adofai.FullName);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, $"Failed parsing {adofai.FullName}.");
                        continue;
                    }

                    var circle = converter.Convert(adofaiBeatmap);
                    string title = $"[{(string.IsNullOrEmpty(circle.Metadata.Author) ? "Unknown author" : circle.Metadata.Author)}] "
                                   + $"{(string.IsNullOrEmpty(circle.Metadata.Artist) ? "Unknown artist" : circle.Metadata.Artist)} - "
                                   + $"{(string.IsNullOrEmpty(circle.Metadata.Song) ? circle.Metadata.SongFileName : circle.Metadata.Song)}";

                    if (title.Length >= 100)
                        title = title.Substring(0, 100);

                    string fileName = FileUtil.ReplaceSafeChar($"{title}.circle");

                    var beatmap = storage.GetStorageForDirectory(Path.GetFileNameWithoutExtension(fileName));

                    try
                    {
                        Logger.Log($"Writing to \"{fileName}\"...");

                        using (StreamWriter sw = File.CreateText(Path.Combine(beatmap.GetFullPath(string.Empty), fileName)))
                            sw.WriteLine(JsonSerializer.Serialize(circle, serializerOptions));
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, $"Error while writing {Path.GetFileNameWithoutExtension(fileName)}.");
                    }

                    Logger.Log("Copying beatmap resources...");
                    FileUtil.TryCopy(Path.Combine(adofai.DirectoryName ?? string.Empty, circle.Metadata.BgImage), Path.Combine(beatmap.GetFullPath(string.Empty), circle.Metadata.BgImage));
                    FileUtil.TryCopy(Path.Combine(adofai.DirectoryName ?? string.Empty, circle.Metadata.SongFileName), Path.Combine(beatmap.GetFullPath(string.Empty), circle.Metadata.SongFileName));
                    FileUtil.TryCopy(Path.Combine(adofai.DirectoryName ?? string.Empty, circle.Metadata.BgVideo), Path.Combine(beatmap.GetFullPath(string.Empty), circle.Metadata.BgVideo));

                    OnImported?.Invoke(title);
                }

                progress.Value++;
            }
        }

        public void Export(BeatmapInfo beatmapInfo, string path)
        {
            // TODO: 압축 후 내보내기 지원
        }

        public event Action<string>? OnImported;
    }
}
