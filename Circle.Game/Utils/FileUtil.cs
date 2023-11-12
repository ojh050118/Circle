using System.IO;

namespace Circle.Game.Utils
{
    public static class FileUtil
    {
        private static readonly char[] unused_char = { '\\', '/', ':', '*', '?', '\"', '<', '>', '|' };

        /// <summary>
        /// 복사를 시도해봅니다.
        /// </summary>
        /// <param name="sourceFileName">파일경로가 포함된 원본 파일이름.</param>
        /// <param name="destFileName">파일경로가 포함된 복사할 파일이름.</param>
        /// <returns>복사 성공 여부.</returns>
        public static bool TryCopy(string sourceFileName, string destFileName)
        {
            try
            {
                if (File.Exists(sourceFileName))
                    File.Copy(sourceFileName, destFileName);
                else
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// OS에서 허용하지않는 모든 문자를 <paramref name="replaceChar"/>로 대체합니다.
        /// </summary>
        /// <param name="text">파일 이름.</param>
        /// <param name="replaceChar">대체할 문자.</param>
        /// <returns><paramref name="replaceChar"/>로 대체된 파일 이름.</returns>
        public static string ReplaceSafeChar(string text, char replaceChar = '_')
        {
            string result = text;

            foreach (char c in unused_char)
            {
                if (result.Contains(c))
                    result = result.Replace(c, replaceChar);
            }

            return result;
        }
    }
}
