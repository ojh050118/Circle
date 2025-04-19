namespace Circle.Desktop.Deploy.Uploaders
{
    public abstract class Uploader
    {
        public abstract void RestoreBuild();

        public abstract void PublishBuild(string version);
    }
}
