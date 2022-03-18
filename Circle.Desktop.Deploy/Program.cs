using System.Configuration;
using System.Diagnostics;
using System.Management.Automation;
using Newtonsoft.Json;
using osu.Framework;
using osu.Framework.Extensions;
using osu.Framework.IO.Network;
using FileWebRequest = osu.Framework.IO.Network.FileWebRequest;
using WebRequest = osu.Framework.IO.Network.WebRequest;

namespace Circle.Desktop.Deploy
{
    public static class Program
    {
        private static string packages => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");
        private static string nugetPath => Path.Combine(packages, @"nuget.commandline\6.0.0\Tools\NuGet.exe");
        private static string squirrelPath => Path.Combine(packages, @"clowd.squirrel\2.8.15-pre\Tools\Squirrel.exe");

        private const string staging_folder = "staging";
        private const string templates_folder = "templates";
        private const string releases_folder = "releases";

        /// <summary>
        /// 게시할 때 유지하려는 이전 빌드 델타 수입니다.
        /// </summary>
        private const int keep_delta_count = 4;

#pragma warning disable CS8601 // 가능한 null 참조 할당입니다.
        public static string GitHubAccessToken = ConfigurationManager.AppSettings["GitHubAccessToken"];
        public static bool GitHubUpload = bool.Parse(ConfigurationManager.AppSettings["GitHubUpload"] ?? "false");
        public static string GitHubUsername = ConfigurationManager.AppSettings["GitHubUsername"];
        public static string GitHubRepoName = ConfigurationManager.AppSettings["GitHubRepoName"];
        public static string SolutionName = ConfigurationManager.AppSettings["SolutionName"];
        public static string ProjectName = ConfigurationManager.AppSettings["ProjectName"];
        public static string NuSpecName = ConfigurationManager.AppSettings["NuSpecName"];
        public static bool IncrementVersion = bool.Parse(ConfigurationManager.AppSettings["IncrementVersion"] ?? "true");
        public static string PackageName = ConfigurationManager.AppSettings["PackageName"];
        public static string IconName = ConfigurationManager.AppSettings["IconName"];
        public static string CodeSigningCertificate = ConfigurationManager.AppSettings["CodeSigningCertificate"];

        public static string GitHubApiEndpoint => $"https://api.github.com/repos/{GitHubUsername}/{GitHubRepoName}/releases";

#pragma warning disable CS8618 // 생성자를 종료할 때 null을 허용하지 않는 필드에 null이 아닌 값을 포함해야 합니다. null 허용으로 선언해 보세요.
        private static string solutionPath;

        private static string stagingPath => Path.Combine(Environment.CurrentDirectory, staging_folder);
        private static string templatesPath => Path.Combine(Environment.CurrentDirectory, templates_folder);
        private static string releasesPath => Path.Combine(Environment.CurrentDirectory, releases_folder);
        private static string iconPath => Path.Combine(solutionPath, ProjectName, IconName);
        private static string splashImagePath => Path.Combine(solutionPath, "assets\\of-nuget.png");

        private static readonly Stopwatch stopwatch = new Stopwatch();

        private static bool interactive;

        public static void Main(string[] args)
        {
            interactive = args.Length == 0;
            displayHeader();

            findSolutionPath();

            if (!Directory.Exists(releases_folder))
            {
                write("경고: 릴리스 디렉토리를 찾을 수 없습니다. 이것을 원하는지 확인하십시오!", ConsoleColor.Yellow);
                Directory.CreateDirectory(releases_folder);
            }

#pragma warning disable CS8600 // null 리터럴 또는 가능한 null 값을 null을 허용하지 않는 형식으로 변환하는 중입니다.
            GitHubRelease lastRelease = null;

            if (canGitHub)
            {
                write("GitHub 릴리스 확인 중...");
                lastRelease = getLastGithubRelease();

                write($"마지막 GitHub 릴리즈는 {lastRelease.Name}입니다.");
            }

            //고유한 빌드 번호를 가질 때까지 빌드 번호를 증가시킵니다.
            string verBase = DateTime.Now.ToString("yyyy.Mdd.");
            int increment = 0;

            if (lastRelease?.TagName.StartsWith(verBase, StringComparison.InvariantCulture) ?? false)
                increment = int.Parse(lastRelease.TagName.Split('.')[2]) + (IncrementVersion ? 1 : 0);

            string version = $"{verBase}{increment}";

            var targetPlatform = RuntimeInfo.OS;

            if (args.Length > 1 && !string.IsNullOrEmpty(args[1]))
                version = args[1];
            if (args.Length > 2 && !string.IsNullOrEmpty(args[2]))
                Enum.TryParse(args[2], true, out targetPlatform);

            Console.ResetColor();
            Console.WriteLine($"증분 버전:     {IncrementVersion}");
            Console.WriteLine($"서명 인증서:   {CodeSigningCertificate}");
            Console.WriteLine($"GitHub에 업로드:      {GitHubUpload}");
            Console.WriteLine();
            Console.Write($"{targetPlatform} 플랫폼에 {version} 버전을 배포할 준비가 되었습니다!");

            pauseIfInteractive();

            stopwatch.Start();

            refreshDirectory(staging_folder);
            updateAppveyorVersion(version);

            write("빌드 프로세스 실행 중...");

            switch (targetPlatform)
            {
                case RuntimeInfo.Platform.Windows:
#pragma warning disable CS8604 // 가능한 null 참조 인수입니다.
                    getAssetsFromRelease(lastRelease);

                    runCommand("dotnet", $"publish -f net6.0 -r win-x64 {ProjectName} -o {stagingPath} --configuration Release /p:Version={version}");

                    // dotnet 스텁의 하위 시스템을 Windows로 변경(기본값은 콘솔, 아직 변경할 방법 없음 https://github.com/dotnet/core-setup/issues/196)
                    runCommand("tools/editbin.exe", $"/SUBSYSTEM:WINDOWS {stagingPath}\\Circle.exe");

                    // dotnet 스텁에 아이콘 추가
                    runCommand("tools/rcedit-x64.exe", $"\"{stagingPath}\\Circle.exe\" --set-icon \"{iconPath}\"");

                    write("NuGet 배포 패키지 생성 중...");
                    runCommand(nugetPath, $"pack {NuSpecName} -Version {version} -Properties Configuration=Deploy -OutputDirectory {stagingPath} -BasePath {stagingPath}");

                    // 이 빌드에 필요하지 않은 파일에 대한 오류를 방지할 수 있도록 파일을 확인하기 전에 한 번 정리합니다.
                    pruneReleases();

                    checkReleaseFiles();

                    write("squirrel 빌드 실행중...");

                    string codeSigningPassword = string.Empty;

                    if (!string.IsNullOrEmpty(CodeSigningCertificate))
                    {
                        if (args.Length > 0)
                        {
                            codeSigningPassword = args[0];
                        }
                        else
                        {
                            Console.Write("코드 서명 암호를 입력하십시오: ");
                            codeSigningPassword = readLineMasked();
                        }
                    }

                    string codeSigningCertPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), CodeSigningCertificate);
                    string codeSigningCmd = string.IsNullOrEmpty(codeSigningPassword)
                        ? ""
                        : $"--signParams=\"/td sha256 /fd sha256 /f {codeSigningCertPath} /p {codeSigningPassword} /tr http://timestamp.comodoca.com\"";

                    string nupkgFileName = $"{PackageName}.{version}.nupkg";

                    runCommand(squirrelPath, $"releasify --package={stagingPath}\\{nupkgFileName} --releaseDir={releasesPath} --icon={iconPath} --appIcon={iconPath} --splashImage={splashImagePath} {codeSigningCmd} --allowUnaware");

                    // 업로드 전에 정리를 위해 다시 정리합니다.
                    pruneReleases();

                    // 설치할 setup의 이름을 바꿉니다.
                    File.Copy(Path.Combine(releases_folder, "CircleSetup.exe"), Path.Combine(releases_folder, "install.exe"), true);
                    File.Delete(Path.Combine(releases_folder, "CircleSetup.exe"));
                    break;

                case RuntimeInfo.Platform.Linux:
                    const string app_dir = "Circle.AppDir";

                    string stagingTarget = Path.Combine(stagingPath, app_dir);

                    if (Directory.Exists(stagingTarget))
                        Directory.Delete(stagingTarget, true);

                    Directory.CreateDirectory(stagingTarget);

                    foreach (var file in Directory.GetFiles(Path.Combine(templatesPath, app_dir)))
                        new FileInfo(file).CopyTo(Path.Combine(stagingTarget, Path.GetFileName(file)));

                    // zip에는 실행 가능한 정보가 포함되어 있지 않으므로 AppRun을 실행 가능한 것으로 표시합니다.
                    runCommand("chmod", $"+x {stagingTarget}/AppRun");

                    runCommand("dotnet", $"publish -f net6.0 -r linux-x64 {ProjectName} -o {stagingTarget}/usr/bin/ --configuration Release /p:Version={version} --self-contained");

                    // 출력을 실행 가능한 것으로 표시
                    runCommand("chmod", $"+x {stagingTarget}/usr/bin/Circle");

                    // png 아이콘 복사(데스크탑 파일용)
                    File.Copy(Path.Combine(solutionPath, "assets/of.png"), $"{stagingTarget}/Circle.png");

                    // appimagetool 다운로드
                    string appImageToolPath = $"{stagingPath}/appimagetool.AppImage";

                    using (var client = new HttpClient())
                    {
                        using (var stream = client.GetStreamAsync("https://github.com/AppImage/AppImageKit/releases/download/continuous/appimagetool-x86_64.AppImage").GetResultSafely())
                        using (var fileStream = new FileStream(appImageToolPath, FileMode.CreateNew))
                        {
                            stream.CopyToAsync(fileStream).WaitSafely();
                        }
                    }

                    // appimagetool을 실행 가능으로 표시
                    runCommand("chmod", $"a+x {appImageToolPath}");

                    // AppImage 자체 생성
                    // gh-releases-zsync는 GitHub Releases ZSync를 나타내며 업데이트를 확인하는 방법입니다.
                    // ojh050118|Circle|latest는 https://github.com/ojh050118/Circle 를 나타내며 최신 릴리스를 얻습니다.
                    // Circle.AppImage.zsync는 AppImage 업데이트 정보 파일로, 도구에 의해 생성됩니다.
                    // 자세한 내용은 https://docs.appimage.org/packaging-guide/optional/updates.html?highlight=update#using-appimagetool 에 있습니다.
                    runCommand(appImageToolPath, $"\"{stagingTarget}\" -u \"gh-releases-zsync|Junho|Circle|latest|Circle.AppImage.zsync\" \"{Path.Combine(Environment.CurrentDirectory, "releases")}/Circle.AppImage\" --sign", false);

                    // 마지막으로 Circle을 표시합니다! AppImage를 실행 파일로 -> 압축하지 마십시오.
                    runCommand("chmod", $"+x \"{Path.Combine(Environment.CurrentDirectory, "releases")}/Circle.AppImage\"");

                    // 업데이트 정보 복사
                    File.Move(Path.Combine(Environment.CurrentDirectory, "Circle.AppImage.zsync"), $"{releases_folder}/Circle.AppImage.zsync", true);

                    break;
            }

            if (GitHubUpload)
                uploadBuild(version);

            write("완료!");
            pauseIfInteractive();
        }

        private static void displayHeader()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("  이 프로젝트의 빌드를 공개적으로 사용하여 배포하지 마십시오.");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// 릴리스 디렉토리에 있을 것으로 예상되는 모든 파일이 있는지 확인합니다.
        /// 이것은 이전 단계에서 설명되어야 하며 단지 확인 단계로 사용됩니다.
        /// </summary>
        private static void checkReleaseFiles()
        {
            if (!canGitHub) return;

            var releaseLines = getReleaseLines();

            // 필요한 모든 파일이 있는지 확인
            foreach (var l in releaseLines)
            {
                if (!File.Exists(Path.Combine(releases_folder, l.Filename)))
                    error($"로컬 파일 {l.Filename} 누락");
            }
        }

        private static IEnumerable<ReleaseLine> getReleaseLines()
        {
            return File.ReadAllLines(Path.Combine(releases_folder, "RELEASES")).Select(l => new ReleaseLine(l));
        }

        private static void pruneReleases()
        {
            if (!canGitHub) return;

            write("릴리즈 정리...");

            var releaseLines = getReleaseLines().ToList();

            var fulls = releaseLines.Where(l => l.Filename.Contains("-full")).Reverse().Skip(1);

            // 모든 전체 릴리스를 제거합니다(최신 버전 제외).
            foreach (var l in fulls)
            {
                write($"- 이전 릴리즈 {l.Filename} 제거", ConsoleColor.Yellow);
                File.Delete(Path.Combine(releases_folder, l.Filename));
                releaseLines.Remove(l);
            }

            // 초과 델타 제거
            var deltas = releaseLines.Where(l => l.Filename.Contains("-delta")).ToArray();

            if (deltas.Length > keep_delta_count)
            {
                foreach (var l in deltas.Take(deltas.Length - keep_delta_count))
                {
                    write($"- 오래된 델타 {l.Filename} 제거", ConsoleColor.Yellow);
                    File.Delete(Path.Combine(releases_folder, l.Filename));
                    releaseLines.Remove(l);
                }
            }

            var lines = new List<string>();
            releaseLines.ForEach(l => lines.Add(l.ToString()));
            File.WriteAllLines(Path.Combine(releases_folder, "RELEASES"), lines);
        }

        private static void uploadBuild(string version)
        {
            if (!canGitHub || string.IsNullOrEmpty(CodeSigningCertificate))
                return;

            write("GitHub에 개시 중...");

            var req = new JsonWebRequest<GitHubRelease>($"{GitHubApiEndpoint}")
            {
                Method = HttpMethod.Post,
            };

            GitHubRelease targetRelease = getLastGithubRelease(true);

            if (targetRelease.TagName != version)
            {
                write($"- 릴리즈 {version} 생성 중...", ConsoleColor.Yellow);
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
                write($"- 기존 릴리즈에 {version} 추가 중...", ConsoleColor.Yellow);
            }

            var assetUploadUrl = targetRelease.UploadUrl.Replace("{?name,label}", "?name={0}");

            foreach (var a in Directory.GetFiles(releases_folder).Reverse()) // RELEASES를 먼저 업로드하려면 역순입니다.
            {
                if (Path.GetFileName(a).StartsWith('.'))
                    continue;

                write($"- 자산 {a} 추가 중...", ConsoleColor.Yellow);
                var upload = new WebRequest(assetUploadUrl, Path.GetFileName(a))
                {
                    Method = HttpMethod.Post,
                    Timeout = 240000,
                    ContentType = "application/octet-stream",
                };

                upload.AddRaw(File.ReadAllBytes(a));
                upload.AuthenticatedBlockingPerform();
            }

            openGitHubReleasePage();
        }

        private static void openGitHubReleasePage()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = $"https://github.com/{GitHubUsername}/{GitHubRepoName}/releases",
                UseShellExecute = true // https://github.com/dotnet/corefx/issues/10361 참조
            });
        }

        private static bool canGitHub => !string.IsNullOrEmpty(GitHubAccessToken);

        private static GitHubRelease getLastGithubRelease(bool includeDrafts = false)
        {
            var req = new JsonWebRequest<List<GitHubRelease>>($"{GitHubApiEndpoint}");
            req.AuthenticatedBlockingPerform();
#pragma warning disable CS8603 // 가능한 null 참조 반환입니다.
            return req.ResponseObject.FirstOrDefault(r => includeDrafts || !r.Draft);
        }

        /// <summary>
        /// 이전 릴리스의 자산을 릴리스 폴더로 다운로드합니다.
        /// </summary>
        /// <param name="release"></param>
        private static void getAssetsFromRelease(GitHubRelease release)
        {
            if (!canGitHub) return;

            // 이 프로젝트에 대한 이전 릴리스가 있습니다.
            var assetReq = new JsonWebRequest<List<GitHubObject>>($"{GitHubApiEndpoint}/{release.Id}/assets");
            assetReq.AuthenticatedBlockingPerform();
            var assets = assetReq.ResponseObject;

            // RELEASES 파일이 서버의 마지막 빌드와 동일한지 확인합니다.
            var releaseAsset = assets.FirstOrDefault(a => a.Name == "RELEASES");

            // RELEASES 자산이 없는 경우 이전 릴리스는 Squirrel이 아니었을 것입니다.
            if (releaseAsset == null) return;

            bool requireDownload = false;

            if (!File.Exists(Path.Combine(releases_folder, $"{PackageName}-{release.Name}-full.nupkg")))
            {
                write("마지막 버전의 패키지를 로컬에서 찾을 수 없습니다.", ConsoleColor.Red);
                requireDownload = true;
            }
            else
            {
                var lastReleases = new RawFileWebRequest($"{GitHubApiEndpoint}/assets/{releaseAsset.Id}");
                lastReleases.AuthenticatedBlockingPerform();

                if (File.ReadAllText(Path.Combine(releases_folder, "RELEASES")) != lastReleases.GetResponseString())
                {
                    write("서버의 릴리즈가 우리와 다릅니다.", ConsoleColor.Red);
                    requireDownload = true;
                }
            }

            if (!requireDownload) return;

            write("로컬 릴리즈 디렉토리 새로 고치는 중...");
            refreshDirectory(releases_folder);

            foreach (var a in assets)
            {
                if (a.Name != "RELEASES" && !a.Name.EndsWith(".nupkg", StringComparison.InvariantCulture)) continue;

                write($"- Downloading {a.Name}...", ConsoleColor.Yellow);
                new FileWebRequest(Path.Combine(releases_folder, a.Name), $"{GitHubApiEndpoint}/assets/{a.Id}").AuthenticatedBlockingPerform();
            }
        }

        private static void refreshDirectory(string directory)
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
            Directory.CreateDirectory(directory);
        }

        /// <summary>
        /// 활성 솔루션의 기본 경로 찾기(git checkout 위치)
        /// </summary>
        private static void findSolutionPath()
        {
            string path = Path.GetDirectoryName(Environment.CommandLine.Replace("\"", "").Trim());

            if (string.IsNullOrEmpty(path))
                path = Environment.CurrentDirectory;

            while (true)
            {
                if (File.Exists(Path.Combine(path, $"{SolutionName}.sln")))
                    break;

                if (Directory.Exists(Path.Combine(path, "Circle")) && File.Exists(Path.Combine(path, "Circle", $"{SolutionName}.sln")))
                {
                    path = Path.Combine(path, "Circle");
                    break;
                }

                path = path.Remove(path.LastIndexOf(Path.DirectorySeparatorChar));
            }

            path += Path.DirectorySeparatorChar;

            solutionPath = path;
        }

        private static bool runCommand(string command, string args, bool useSolutionPath = true)
        {
            write($"{command} {args} 실행 중...");

            var psi = new ProcessStartInfo(command, args)
            {
                WorkingDirectory = useSolutionPath ? solutionPath : Environment.CurrentDirectory,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process p = Process.Start(psi);
            if (p == null) return false;

            string output = p.StandardOutput.ReadToEnd();
            output += p.StandardError.ReadToEnd();

            p.WaitForExit();

            if (p.ExitCode == 0) return true;

            write(output);
            error($"명령 {command} {args} 실패!");
            return false;
        }

        private static string readLineMasked()
        {
            var fg = Console.ForegroundColor;
            Console.ForegroundColor = Console.BackgroundColor;
            var ret = Console.ReadLine();
            Console.ForegroundColor = fg;

            return ret;
        }

        private static void error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"치명적 오류: {message}");

            pauseIfInteractive();
            Environment.Exit(-1);
        }

        private static void pauseIfInteractive()
        {
            if (interactive)
                Console.ReadLine();
            else
                Console.WriteLine();
        }

        private static bool updateAppveyorVersion(string version)
        {
            try
            {
                using (PowerShell ps = PowerShell.Create())
                {
                    ps.AddScript($"Update-AppveyorBuild -Version \"{version}\"");
                    ps.Invoke();
                }

                return true;
            }
            catch
            {
                // 우리는 appveyor가 없고 상관하지 않습니다.
            }

            return false;
        }

        private static void write(string message, ConsoleColor col = ConsoleColor.Gray)
        {
            if (stopwatch.ElapsedMilliseconds > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(stopwatch.ElapsedMilliseconds.ToString().PadRight(8));
            }

            Console.ForegroundColor = col;
            Console.WriteLine(message);
        }

        public static void AuthenticatedBlockingPerform(this WebRequest r)
        {
            r.AddHeader("Authorization", $"token {GitHubAccessToken}");
            r.Perform();
        }
    }

    internal class RawFileWebRequest : WebRequest
    {
        public RawFileWebRequest(string url)
            : base(url)
        {
        }

        protected override string Accept => "application/octet-stream";
    }

    internal class ReleaseLine
    {
        public string Hash;
        public string Filename;
        public int FileSize;

        public ReleaseLine(string line)
        {
            var split = line.Split(' ');
            Hash = split[0];
            Filename = split[1];
            FileSize = int.Parse(split[2]);
        }

        public override string ToString()
        {
            return $"{Hash} {Filename} {FileSize}";
        }
    }
}
