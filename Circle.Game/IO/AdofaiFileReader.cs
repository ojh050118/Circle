#nullable disable

using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Circle.Game.Converting.Adofai;
using Circle.Game.Converting.Json;

namespace Circle.Game.IO
{
    public class AdofaiFileReader
    {
        public AdofaiBeatmap AdofaiFile { get; private set; }

        private readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReadCommentHandling = JsonCommentHandling.Skip,
            UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter(), new ToggleToBoolConverter() }
        };

        public AdofaiBeatmap Get(string file)
        {
            using (StreamReader sr = new StreamReader(file, Encoding.UTF8))
            {
                string text = sr.ReadToEnd();
                filterTrailingComma(ref text);
                addComma(ref text);
                ensureJsonString(ref text);
                text = text.Replace("\\\\\"", string.Empty);

                AdofaiFile = JsonSerializer.Deserialize<AdofaiBeatmap>(text, options);
            }

            return AdofaiFile;
        }

        private string filterTrailingComma(ref string result)
        {
            return result = result.Replace(",,", ",");
        }

        private string addComma(ref string result)
        {
            return result = result.Replace("}\n", "},\n")
                                  .Replace("]\n", "],\n")
                                  .TrimEnd('\n').TrimEnd(',');
        }

        private string ensureJsonString(ref string result)
        {
            return result = Regex.Replace(result, @"""([^""\\]*(?:\\.[^""\\]*)*)""", match =>
            {
                // 따옴표 제거
                string content = match.Groups.Count > 2 ? match.Groups[1].Value : match.Value.Substring(1, match.Value.Length - 2);

                // 실제 이스케이프 문자를 Json String에 맞도록 변환합니다.
                content = content.Replace("\r", "\\r")
                                 .Replace("\n", "\\n")
                                 .Replace("\t", "\\t")
                                 .Replace("\b", "\\b")
                                 .Replace("\f", "\\f")
                                 .Replace("\"", "\\\"");

                return $"\"{content}\"";
            });
        }
    }
}
