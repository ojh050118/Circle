using System.Management.Automation;
using osu.Framework.IO.Network;

namespace Circle.Desktop.Deploy.Uploaders
{
    public class VelopackUploader : Uploader
    {
        private readonly string applicationName;
        private readonly string operatingSystemName;
        private readonly string runtimeIdentifier;
        private readonly string channel;
        private readonly string? extraArgs;
        private readonly string stagingPath;

        public VelopackUploader(string applicationName, string operatingSystemName, string runtimeIdentifier, string channel, string? extraArgs = null, string? stagingPath = null)
        {
            this.applicationName = applicationName;
            this.operatingSystemName = operatingSystemName;
            this.runtimeIdentifier = runtimeIdentifier;
            this.channel = channel;
            this.extraArgs = extraArgs;
            this.stagingPath = stagingPath ?? Program.StagingPath;
        }

        public override void RestoreBuild()
        {
            if (Program.CanGitHub)
            {
                Program.RunCommand("vpk", $"download github"
                                          + $" --repoUrl=\"{Program.GitHubRepoUrl}\""
                                          + $" --token=\"{Program.GitHubAccessToken}\""
                                          + $" --channel=\"{channel}\""
                                          + $" --outputDir=\"{Program.ReleasesPath}\"",
                    throwIfNonZero: false,
                    useSolutionPath: false);
            }
        }

        public override void PublishBuild(string version)
        {
            Program.RunCommand("vpk", $"[{operatingSystemName}] pack"
                                      + $" --packTitle=\"Circle\""
                                      + $" --packId=\"{Program.PackageName}\""
                                      + $" --packVersion=\"{version}\""
                                      + $" --runtime=\"{runtimeIdentifier}\""
                                      + $" --outputDir=\"{Program.ReleasesPath}\""
                                      + $" --mainExe=\"{applicationName}\""
                                      + $" --packDir=\"{stagingPath}\""
                                      + $" --channel=\"{channel}\""
                                      + $" {extraArgs}",
                useSolutionPath: false);

            if (Program.CanGitHub && Program.GitHubUpload)
            {
                Program.RunCommand("vpk", $"upload github"
                                          + $" --repoUrl=\"{Program.GitHubRepoUrl}\""
                                          + $" --token=\"{Program.GitHubAccessToken}\""
                                          + $" --outputDir=\"{Program.ReleasesPath}\""
                                          + $" --tag=\"{version}\""
                                          + $" --releaseName=\"{version}\""
                                          + $" --merge"
                                          + $" --channel=\"{channel}\"",
                    useSolutionPath: false);
            }
        }

        protected void RenameAsset(string fromName, string toName)
        {
            if (!Program.CanGitHub || !Program.GitHubUpload)
                return;

            Logger.Write($"자산 이름을 '{fromName}'에서 '{toName}'로 바꾸는 중...");

            GitHubRelease targetRelease = Program.GetLastGithubRelease(true)
                                          ?? throw new ItemNotFoundException("릴리즈를 찾을 수 없습니다.");

            GitHubAsset asset = targetRelease.Assets.SingleOrDefault(a => a.Name == fromName)
                                ?? throw new FileNotFoundException($"릴리즈에서 '{fromName}'에셋을 찾을 수 없습니다.");

            var req = new WebRequest(asset.Url)
            {
                Method = HttpMethod.Patch,
            };

            req.AddRaw(
                $$"""
                  { "name": "{{toName}}" }
                  """);

            req.AuthenticatedBlockingPerform();
        }
    }
}
