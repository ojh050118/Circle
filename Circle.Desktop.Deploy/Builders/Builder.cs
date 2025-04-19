using Circle.Desktop.Deploy.Uploaders;

namespace Circle.Desktop.Deploy.Builders
{
    public abstract class Builder
    {
        protected abstract string TargetFramework { get; }
        protected abstract string RuntimeIdentifier { get; }

        protected string SplashImagePath => Path.Combine(Environment.CurrentDirectory, "Circle-velopack.jpg");
        protected string IconPath => Path.Combine(Program.SolutionPath, Program.ProjectName, Program.IconName);

        protected readonly string Version;

        protected Builder(string version)
        {
            Version = version;

            refreshDirectory(Program.STAGING_FOLDER);
        }

        public abstract Uploader CreateUploader();

        public abstract void Build();

        protected void RunDotnetPublish(string? extraArgs = null, string? outputDir = null)
        {
            extraArgs ??= string.Empty;
            outputDir ??= Program.StagingPath;

            Program.RunCommand("dotnet", $"publish"
                                         + $" -f {TargetFramework}"
                                         + $" -r {RuntimeIdentifier}"
                                         + $" -c Release"
                                         + $" -o \"{outputDir}\""
                                         + $" -p:Version={Version}"
                                         + $" --self-contained"
                                         + $" {extraArgs}"
                                         + $" {Program.ProjectName}");
        }

        private static void refreshDirectory(string directory)
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
            Directory.CreateDirectory(directory);
        }
    }
}
