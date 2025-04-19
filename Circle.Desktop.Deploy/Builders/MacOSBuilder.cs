using Circle.Desktop.Deploy.Uploaders;

namespace Circle.Desktop.Deploy.Builders
{
    public class MacOSBuilder : Builder
    {
        private const string app_dir = "Circle.app";
        private const string app_name = "Circle";
        private const string os_name = "osx";

        private readonly string stagingTarget;
        private readonly string publishTarget;

        public MacOSBuilder(string version, string? arch)
            : base(version)
        {
            if (string.IsNullOrEmpty(arch))
            {
                Console.Write("어떤 아키텍쳐용 빌드입니까? [x64/arm64]: ");
                arch = Console.ReadLine() ?? string.Empty;
            }

            if (arch != "x64" && arch != "arm64")
                Logger.Error($"유효하지않는 아키텍쳐: {arch}");

            RuntimeIdentifier = $"{os_name}-{arch}";

            stagingTarget = Path.Combine(Program.StagingPath, app_dir);
            publishTarget = Path.Combine(stagingTarget, "Contents", "MacOS");
        }

        protected override string TargetFramework => "net8.0";
        protected override string RuntimeIdentifier { get; }

        public override Uploader CreateUploader()
        {
            string extraArgs = $" --signEntitlements=\"{Path.Combine(Environment.CurrentDirectory, "Circle.entitlements")}\""
                               + $" --noInst";

            if (!string.IsNullOrEmpty(Program.AppleCodeSignCertName))
                extraArgs += $" --signAppIdentity=\"{Program.AppleCodeSignCertName}\"";
            if (!string.IsNullOrEmpty(Program.AppleInstallSignCertName))
                extraArgs += $" --signInstallIdentity=\"{Program.AppleInstallSignCertName}\"";
            if (!string.IsNullOrEmpty(Program.AppleNotaryProfileName))
                extraArgs += $" --notaryProfile=\"{Program.AppleNotaryProfileName}\"";
            if (!string.IsNullOrEmpty(Program.AppleKeyChainPath))
                extraArgs += $" --keychain=\"{Program.AppleKeyChainPath}\"";

            return new MacOSVelopackUploader(app_name, os_name, RuntimeIdentifier, RuntimeIdentifier, extraArgs: extraArgs, stagingPath: stagingTarget);
        }

        public override void Build()
        {
            if (Directory.Exists(stagingTarget))
                Directory.Delete(stagingTarget, true);

            Program.RunCommand("cp", $"-r \"{Path.Combine(Program.TemplatesPath, app_dir)}\" \"{stagingTarget}\"");

            RunDotnetPublish(outputDir: publishTarget);

            // without touching the app bundle itself, changes to file associations / icons / etc. will be cached at a macOS level and not updated.
            Program.RunCommand("touch", $"\"{stagingTarget}\" {Program.StagingPath}", false);
        }
    }
}
