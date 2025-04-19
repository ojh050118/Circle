using System.ComponentModel;

namespace Circle.Desktop.Deploy.Uploaders
{
    public class MacOSVelopackUploader : VelopackUploader
    {
        private readonly string channel;

        public MacOSVelopackUploader(string applicationName, string operatingSystemName, string runtimeIdentifier, string channel, string? extraArgs = null, string? stagingPath = null)
            : base(applicationName, operatingSystemName, runtimeIdentifier, channel, extraArgs, stagingPath)
        {
            this.channel = channel;
        }

        public override void PublishBuild(string version)
        {
            base.PublishBuild(version);

            string suffix;

            switch (channel)
            {
                case "osx-arm64":
                    suffix = "Apple.Silicon";
                    break;

                case "osx-x64":
                    suffix = "Intel";
                    break;

                default:
                    throw new InvalidEnumArgumentException($"인식할 수 없는 채널: {channel}");
            }

            RenameAsset($"{Program.PackageName}-{channel}-Portable.zip", $"Circle.app.{suffix}.zip");
        }
    }
}
