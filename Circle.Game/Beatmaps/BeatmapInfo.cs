#nullable disable

using System;
using System.IO;

namespace Circle.Game.Beatmaps
{
    /// <summary>
    /// 비트맵 파일에 대한 기본적인 정보를 제공합니다.
    /// 리소스에 대한 경로 정보도 포함하고 있습니다.
    /// </summary>
    public class BeatmapInfo : IEquatable<BeatmapInfo>
    {
        /// <summary>
        /// 비트맵.
        /// </summary>
        public Beatmap Beatmap { get; }

        /// <summary>
        /// 부모 디렉터리 이름입니다.
        /// </summary>
        public string Directory { get; }

        /// <summary>
        /// 모든 디렉터리 경로입니다. (파일 제외)
        /// </summary>
        public string DirectoryName { get; }

        /// <summary>
        /// 파일 이름입니다.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 파일 확장자 입니다.
        /// </summary>
        public string Extension { get; }

        /// <summary>
        /// 비트맵 파일 존재 여부.
        /// </summary>
        public bool Exists { get; }

        /// <summary>
        /// 비트맵 파일에 대한 절대 경로입니다.
        /// </summary>
        public string BeatmapPath { get; }

        /// <summary>
        /// 트랙 파일에 대한 절대 경로입니다.
        /// </summary>
        public string SongPath { get; } = string.Empty;

        /// <summary>
        /// 텍스처 파일에 대한 절대 경로입니다.
        /// </summary>
        public string BackgroundPath { get; } = string.Empty;

        /// <summary>
        /// 비디오 파일에 대한 절대 경로입니다.
        /// </summary>
        public string VideoPath { get; } = string.Empty;

        /// <summary>
        /// 비트맵 파일에 대한 상대 경로입니다. (ex: folder\beatmap.circle)
        /// </summary>
        public string RelativeBeatmapPath { get; }

        /// <summary>
        /// 트랙 파일에 대한 상대 경로입니다. (ex: folder\track.mp3)
        /// </summary>
        public string RelativeSongPath { get; } = string.Empty;

        /// <summary>
        /// 텍스처 파일에 대한 상대 경로입니다. (ex: folder\texture.png)
        /// </summary>
        public string RelativeBackgroundPath { get; } = string.Empty;

        /// <summary>
        /// 비디오 파일에 대한 상대 경로입니다. (ex: folder\video.mp4)
        /// </summary>
        public string RelativeVideoPath { get; } = string.Empty;

        private readonly FileInfo fileInfo;

        public BeatmapInfo(Beatmap beatmap, FileInfo info)
        {
            Beatmap = beatmap;
            fileInfo = info;

            if (info == null)
                return;

            Directory = fileInfo.Directory?.Name ?? string.Empty;
            DirectoryName = fileInfo.DirectoryName ?? string.Empty;
            Name = fileInfo.Name;
            Extension = fileInfo.Extension;
            Exists = fileInfo.Exists;
            BeatmapPath = fileInfo.FullName;

            if (!string.IsNullOrEmpty(Directory))
                RelativeBeatmapPath = Path.Combine(Directory, Name);

            if (!string.IsNullOrEmpty(Beatmap.Settings.SongFileName))
            {
                SongPath = Path.Combine(DirectoryName, Beatmap.Settings.SongFileName);
                RelativeSongPath = Path.Combine(Directory, Beatmap.Settings.SongFileName);
            }

            if (!string.IsNullOrEmpty(Beatmap.Settings.BgImage))
            {
                BackgroundPath = Path.Combine(DirectoryName, Beatmap.Settings.BgImage);
                RelativeBackgroundPath = Path.Combine(Directory, Beatmap.Settings.BgImage);
            }

            if (!string.IsNullOrEmpty(Beatmap.Settings.BgVideo))
            {
                VideoPath = Path.Combine(DirectoryName, Beatmap.Settings.BgVideo);
                RelativeVideoPath = Path.Combine(Directory, Beatmap.Settings.BgVideo);
            }
        }

        public bool Equals(BeatmapInfo info)
        {
            if (info == null)
                return false;

            return Beatmap.Equals(info.Beatmap) &&
                   Directory == info.Directory &&
                   DirectoryName == info.DirectoryName &&
                   Name == info.Name &&
                   Extension == info.Extension &&
                   Exists == info.Exists &&
                   BeatmapPath == info.BeatmapPath &&
                   SongPath == info.SongPath &&
                   BackgroundPath == info.BackgroundPath &&
                   VideoPath == info.VideoPath &&
                   RelativeBeatmapPath == info.RelativeBeatmapPath &&
                   RelativeSongPath == info.RelativeSongPath &&
                   RelativeBackgroundPath == info.RelativeBackgroundPath &&
                   RelativeVideoPath == info.RelativeVideoPath;
        }

        /// <summary>
        /// 비트맵 파일을 삭제합니다.
        /// </summary>
        public void Delete() => fileInfo.Delete();

        public override string ToString() => $"[{Beatmap.Settings.Author}] {Beatmap.Settings.Artist} - {Beatmap.Settings.Song}";
    }
}
