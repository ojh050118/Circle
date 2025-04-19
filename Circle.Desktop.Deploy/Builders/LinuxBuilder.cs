using Circle.Desktop.Deploy.Uploaders;

namespace Circle.Desktop.Deploy.Builders
{
    public class LinuxBuilder : Builder
    {
        private const string app_dir = "Circle.AppDir";
        private const string app_name = "Circle";
        private const string os_name = "linux";

        private readonly string stagingTarget;
        private readonly string publishTarget;

        public LinuxBuilder(string version)
            : base(version)
        {
            stagingTarget = Path.Combine(Program.StagingPath, app_dir);
            publishTarget = Path.Combine(stagingTarget, "usr", "bin");
        }

        protected override string TargetFramework => "net8.0";
        protected override string RuntimeIdentifier => $"{os_name}-x64";

        public override Uploader CreateUploader() => new LinuxVelopackUploader(app_name, os_name, RuntimeIdentifier, RuntimeIdentifier, stagingPath: stagingTarget);

        public override void Build()
        {
            if (Directory.Exists(stagingTarget))
                Directory.Delete(stagingTarget, true);
            Directory.CreateDirectory(stagingTarget);

            foreach (string file in Directory.GetFiles(Path.Combine(Program.TemplatesPath, app_dir)))
                new FileInfo(file).CopyTo(Path.Combine(stagingTarget, Path.GetFileName(file)));

            Program.RunCommand("chmod", $"+x {stagingTarget}/AppRun");

            RunDotnetPublish(outputDir: publishTarget);
        }
    }
}
