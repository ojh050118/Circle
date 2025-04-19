using System.Configuration;
using System.Diagnostics;
using Circle.Desktop.Deploy.Builders;
using Circle.Desktop.Deploy.Uploaders;
using osu.Framework;
using osu.Framework.IO.Network;

namespace Circle.Desktop.Deploy
{
    public static partial class Program
    {
        public const string STAGING_FOLDER = "staging";
        public const string TEMPLATES_FOLDER = "templates";
        public const string RELEASES_FOLDER = "releases";

        public static string StagingPath => Path.Combine(Environment.CurrentDirectory, STAGING_FOLDER);
        public static string TemplatesPath => Path.Combine(Environment.CurrentDirectory, TEMPLATES_FOLDER);
        public static string ReleasesPath => Path.Combine(Environment.CurrentDirectory, RELEASES_FOLDER);

        public static string SolutionName => GetConfiguration("SolutionName");
        public static string ProjectName => GetConfiguration("ProjectName");
        public static string PackageName => GetConfiguration("PackageName");
        public static string IconName => GetConfiguration("IconName");

        public static bool GitHubUpload => bool.Parse(ConfigurationManager.AppSettings["GitHubUpload"] ?? "false");
        public static string? GitHubUsername => ConfigurationManager.AppSettings["GitHubUsername"];
        public static string? GitHubRepoName => ConfigurationManager.AppSettings["GitHubRepoName"];
        public static string? GitHubAccessToken => ConfigurationManager.AppSettings["GitHubAccessToken"];
        public static string GitHubApiEndpoint => $"https://api.github.com/repos/{GitHubUsername}/{GitHubRepoName}/releases";
        public static string GitHubRepoUrl => $"https://github.com/{GitHubUsername}/{GitHubRepoName}";
        public static bool CanGitHub => !string.IsNullOrEmpty(GitHubAccessToken);

        public static string? WindowsCodeSigningCertPath => ConfigurationManager.AppSettings["WindowsCodeSigningCertPath"];
        public static string? AndroidCodeSigningCertPath => ConfigurationManager.AppSettings["AndroidCodeSigningCertPath"];
        public static string? AppleCodeSignCertName => ConfigurationManager.AppSettings["AppleCodeSignCertName"];
        public static string? AppleInstallSignCertName => ConfigurationManager.AppSettings["AppleInstallSignCertName"];
        public static string? AppleNotaryProfileName => ConfigurationManager.AppSettings["AppleNotaryProfileName"];
        public static string? AppleKeyChainPath => ConfigurationManager.AppSettings["AppleKeyChainPath"];

        public static bool IncrementVersion => bool.Parse(ConfigurationManager.AppSettings["IncrementVersion"] ?? "true");

        public static string SolutionPath { get; private set; } = null!;

        private static bool interactive;

        /// <summary>
        /// args[0]: code signing passphrase
        /// args[1]: version
        /// args[2]: platform
        /// args[3]: arch
        /// </summary>
        public static void Main(string[] args)
        {
            interactive = args.Length == 0;
            displayHeader();

            findSolutionPath();

            if (!Directory.Exists(RELEASES_FOLDER))
            {
                Logger.Write("경고: 릴리즈 디렉토리를 찾을 수 없습니다. 이것을 원하는지 확인하십시오!", ConsoleColor.Yellow);
                Directory.CreateDirectory(RELEASES_FOLDER);
            }

            GitHubRelease? lastRelease = null;

            if (CanGitHub)
            {
                Logger.Write("GitHub 릴리즈 확인 중...");
                lastRelease = GetLastGithubRelease();

                Logger.Write(lastRelease == null
                    ? "이번이 첫 GitHub 릴리즈입니다."
                    : $"마지막 GitHub 릴리즈는 {lastRelease.Name}입니다.");
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
            Console.WriteLine($"서명 인증서:   {WindowsCodeSigningCertPath}");
            Console.WriteLine($"GitHub에 업로드:      {GitHubUpload}");
            Console.WriteLine();
            Console.Write($"{targetPlatform} 플랫폼에 {version} 버전을 배포할 준비가 되었습니다!");

            PauseIfInteractive();

            RunCommand("dotnet", "tool restore", useSolutionPath: false);

            Logger.Write("vpk 명령줄 도구 설치 중...");
            RunCommand("dotnet", "tool install -g vpk");

            Builder builder;

            switch (targetPlatform)
            {
                case RuntimeInfo.Platform.Windows:
                    builder = new WindowsBuilder(version, getArg(0));
                    break;

                case RuntimeInfo.Platform.Linux:
                    builder = new LinuxBuilder(version);
                    break;

                case RuntimeInfo.Platform.macOS:
                    builder = new MacOSBuilder(version, getArg(3));
                    break;

                case RuntimeInfo.Platform.iOS:
                    builder = new IOSBuilder(version);
                    break;

                case RuntimeInfo.Platform.Android:
                    builder = new AndroidBuilder(version, getArg(0));
                    break;

                default:
                    throw new PlatformNotSupportedException(targetPlatform.ToString());
            }

            Uploader uploader = builder.CreateUploader();

            Logger.Write("이전 빌드 복원 중...");
            uploader.RestoreBuild();

            Logger.Write("빌드 실행 중...");
            builder.Build();

            Logger.Write("릴리즈 생성 중...");
            uploader.PublishBuild(version);

            if (CanGitHub && GitHubUpload)
                openGitHubReleasePage();

            Logger.Write("완료!");
            PauseIfInteractive();
        }

        private static void displayHeader()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("  이 프로젝트의 빌드를 공개적으로 사용하여 배포하지 마십시오.");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void openGitHubReleasePage()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = GitHubApiEndpoint,
                UseShellExecute = true // https://github.com/dotnet/corefx/issues/10361 참조
            });
        }

        public static GitHubRelease? GetLastGithubRelease(bool includeDrafts = false)
        {
            var req = new JsonWebRequest<List<GitHubRelease>>($"{GitHubApiEndpoint}");
            req.AuthenticatedBlockingPerform();

            return req.ResponseObject.FirstOrDefault(r => includeDrafts || !r.Draft);
        }

        /// <summary>
        /// 활성 솔루션의 기본 경로 찾기(git checkout 위치)
        /// </summary>
        private static void findSolutionPath()
        {
            string? path = Path.GetDirectoryName(Environment.CommandLine.Replace("\"", "").Trim());

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

            SolutionPath = path + Path.DirectorySeparatorChar;
        }

        public static bool RunCommand(string command, string args, bool useSolutionPath = true, Dictionary<string, string>? environmentVariables = null, bool throwIfNonZero = true,
                                      bool exitOnFail = true)
        {
            string workingDirectory = useSolutionPath ? SolutionPath : Environment.CurrentDirectory;

            Logger.Write($"작업 디렉터리 {workingDirectory} 사용중...");
            Logger.Write($"{command} {args} 실행 중...");

            var psi = new ProcessStartInfo(command, args)
            {
                WorkingDirectory = workingDirectory,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            if (environmentVariables != null)
            {
                foreach (var pair in environmentVariables)
                    psi.EnvironmentVariables.Add(pair.Key, pair.Value);
            }

            try
            {
                Process? p = Process.Start(psi);
                if (p == null) return false;

                string output = p.StandardOutput.ReadToEnd();
                output += p.StandardError.ReadToEnd();

                p.WaitForExit();

                if (p.ExitCode == 0) return true;

                Logger.Write(output);
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
            }

            if (!throwIfNonZero) return false;

            if (exitOnFail)
                Logger.Error($"명령 {command} {args} 실패!");
            else
                Logger.Write($"명령 {command} {args} 실패!");
            return false;
        }

        public static string? ReadLineMasked(string prompt)
        {
            Console.WriteLine(prompt);

            var fg = Console.ForegroundColor;
            Console.ForegroundColor = Console.BackgroundColor;

            string? ret = Console.ReadLine();
            Console.ForegroundColor = fg;

            return ret;
        }

        public static void PauseIfInteractive()
        {
            if (interactive)
                Console.ReadLine();
            else
                Console.WriteLine();
        }

        public static string GetConfiguration(string key) => ConfigurationManager.AppSettings[key] ?? throw new KeyNotFoundException($"구성 키 '{key}'를 찾지 못했습니다.");

        public static void AuthenticatedBlockingPerform(this WebRequest r)
        {
            r.AddHeader("Authorization", $"token {GitHubAccessToken}");
            r.Perform();
        }

        private static string? getArg(int index)
        {
            string[] args = Environment.GetCommandLineArgs();
            return args.Length > ++index ? args[index] : null;
        }
    }
}
