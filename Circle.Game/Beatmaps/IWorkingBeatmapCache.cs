namespace Circle.Game.Beatmaps
{
    public interface IWorkingBeatmapCache
    {
        WorkingBeatmap GetWorkingBeatmap(BeatmapInfo beatmapInfo);

        void Invalidate(BeatmapSetInfo beatmapSetInfo);

        void Invalidate(BeatmapInfo beatmapInfo);
    }
}
