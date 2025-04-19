using System.Diagnostics;
using Newtonsoft.Json;
using osu.Framework.IO.Network;

namespace Circle.Desktop.Deploy.Uploaders
{
    public class GitHubUploader : Uploader
    {
        public override void RestoreBuild()
        {
        }

        public override void PublishBuild(string version)
        {
            if (!Program.CanGitHub || !Program.GitHubUpload)
                return;

            var req = new JsonWebRequest<GitHubRelease>($"{Program.GitHubApiEndpoint}")
            {
                Method = HttpMethod.Post,
            };

            GitHubRelease? targetRelease = Program.GetLastGithubRelease(true);

            if (targetRelease == null || targetRelease.TagName != version)
            {
                Logger.Write($"- 릴리즈 {version} 생성 중...", ConsoleColor.Yellow);
                req.AddRaw(JsonConvert.SerializeObject(new GitHubRelease
                {
                    Name = version,
                    Draft = true,
                }));
                req.AuthenticatedBlockingPerform();

                targetRelease = req.ResponseObject;
            }
            else
            {
                Logger.Write($"- 기존 릴리즈 {version}에 추가 중...", ConsoleColor.Yellow);
            }

            Debug.Assert(targetRelease.UploadUrl != null);

            string assetUploadUrl = targetRelease.UploadUrl.Replace("{?name,label}", "?name={0}");

            foreach (string a in Directory.GetFiles(Program.RELEASES_FOLDER).Reverse()) //reverse to upload RELEASES first.
            {
                if (Path.GetFileName(a).StartsWith('.'))
                    continue;

                Logger.Write($"- 에셋 {a} 추가 중...", ConsoleColor.Yellow);
                var upload = new WebRequest(assetUploadUrl, Path.GetFileName(a))
                {
                    Method = HttpMethod.Post,
                    Timeout = 240000,
                    ContentType = "application/octet-stream",
                };

                upload.AddRaw(File.ReadAllBytes(a));
                upload.AuthenticatedBlockingPerform();
            }
        }
    }
}
