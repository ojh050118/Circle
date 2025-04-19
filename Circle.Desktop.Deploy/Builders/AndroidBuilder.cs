using Circle.Desktop.Deploy.Uploaders;

namespace Circle.Desktop.Deploy.Builders
{
    public class AndroidBuilder : Builder
    {
        private readonly string? codeSigningPassword;

        public AndroidBuilder(string version, string? codeSigningPassword)
            : base(version)
        {
            if (!string.IsNullOrEmpty(Program.AndroidCodeSigningCertPath))
                this.codeSigningPassword = codeSigningPassword ?? Program.ReadLineMasked("코드 서명 암호를 입력하십시오: ");
        }

        protected override string TargetFramework => "net8.0-android";
        protected override string RuntimeIdentifier => "android-arm64";

        public override Uploader CreateUploader() => new GitHubUploader();

        public override void Build()
        {
            string codeSigningArguments = string.Empty;

            if (!string.IsNullOrEmpty(Program.AndroidCodeSigningCertPath))
            {
                codeSigningArguments +=
                    $" -p:AndroidKeyStore=true"
                    + $" -p:AndroidSigningKeyStore={Program.AndroidCodeSigningCertPath}"
                    + $" -p:AndroidSigningKeyAlias={Path.GetFileNameWithoutExtension(Program.AndroidCodeSigningCertPath)}"
                    + $" -p:AndroidSigningKeyPass={codeSigningPassword}"
                    + $" -p:AndroidSigningKeyStorePass={codeSigningPassword}";
            }

            string[] versionParts = Version.Split('.');
            string versionCode = versionParts[0].PadLeft(4, '0') + versionParts[1].PadLeft(4, '0') + versionParts[2].PadLeft(1, '0');

            RunDotnetPublish($"-p:ApplicationVersion={versionCode} {codeSigningArguments}");

            File.Move(Path.Combine(Program.StagingPath, "junho.Circle-Signed.apk"), Path.Combine(Program.ReleasesPath, "junho.Circle.apk"), true);
        }
    }
}
