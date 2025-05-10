namespace Circle.Game.Converting.Adofai.Elements
{
    public enum EventType
    {
        MoveCamera,
        CustomBackground,
        SetFilter,
        RecolorTrack,
        ColorTrack,
        PositionTrack,
        MoveTrack,
        Bloom,
        SetSpeed,
        Twirl,
        ShakeScreen,
        Flash,
        SetHitsound,
        AnimateTrack,
        SetPlanetRotation,
        RepeatEvents,
        HallOfMirrors,
        AddDecoration,
        MoveDecorations,
        Checkpoint,
        SetConditionalEvents,
        AddText,
        ScreenScroll,
        SetText,
        ScreenTile,
        EditorComment,
        Bookmark,
        PlaySound,
        ChangeTrack,

        // TODO: 구현이 필요할 듯. 게임플레이에 큰 영향을 끼치는 이벤트일 듯함.
        Pause,
        ScaleMargin
    }
}
