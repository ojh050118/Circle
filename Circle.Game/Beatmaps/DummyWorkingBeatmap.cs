#nullable disable

using System;
using System.IO;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;

namespace Circle.Game.Beatmaps
{
    public class DummyWorkingBeatmap : WorkingBeatmap
    {
        private readonly TextureStore textures;

        public DummyWorkingBeatmap(AudioManager audioManager, TextureStore textures)
            : base(new BeatmapInfo
            {
                Metadata = new BeatmapMetadata
                {
                    Artist = "please load a beatmap!",
                    Song = "no beatmaps available!",
                }
            }, audioManager)
        {
            this.textures = textures;
            LoadTrack();
        }

        protected override Beatmap GetBeatmap() => throw new NotImplementedException();
        protected override Track GetBeatmapTrack() => GetVirtualTrack();
        public override Texture GetBackground() => textures?.Get("bg1");
        public override Stream GetVideo() => throw new NotImplementedException();

        public override byte[] Get(string name) => throw new NotImplementedException();

        public override Stream GetStream(string storagePath) => throw new NotImplementedException();
    }
}
