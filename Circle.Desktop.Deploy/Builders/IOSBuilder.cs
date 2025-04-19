using Circle.Desktop.Deploy.Uploaders;

namespace Circle.Desktop.Deploy.Builders
{
    public class IOSBuilder : Builder
    {
        public IOSBuilder(string version)
            : base(version)
        {
        }

        protected override string TargetFramework => "net8.0-ios";
        protected override string RuntimeIdentifier => "ios-arm64";

        public override Uploader CreateUploader() => new GitHubUploader();

        public override void Build()
        {
            RunDotnetPublish("-p:ApplicationDisplayVersion=1.0");

            File.Move(Path.Combine(Program.StagingPath, "Circle.iOS.app"), Path.Combine(Program.ReleasesPath, "Circle.iOS.app"), true);
        }
    }
}
