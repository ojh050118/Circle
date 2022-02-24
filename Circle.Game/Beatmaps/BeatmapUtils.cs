namespace Circle.Game.Beatmaps
{
    public static class BeatmapUtils
    {
        public static bool Compare(Beatmap b1, Beatmap b2)
        {
            if (b1 == null)
            {
                if (b2 == null)
                    return true;
                else
                    return false;
            }
            else
                return b1.Equals(b2);
        }

        public static bool Compare(BeatmapInfo b1, BeatmapInfo b2)
        {
            if (b1 == null)
            {
                if (b2 == null)
                    return true;
                else
                    return false;
            }
            else
                return b1.Equals(b2);
        }
    }
}
