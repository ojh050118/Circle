#nullable disable

using System.IO;
using System.Text;
using Circle.Game.Converting.Adofai;
using Newtonsoft.Json;

namespace Circle.Game.IO
{
    public class AdofaiFileReader
    {
        public AdofaiBeatmap AdofaiFile { get; private set; }

        public AdofaiBeatmap Get(string file)
        {
            using (StreamReader sr = new StreamReader(file, Encoding.UTF8))
            {
                string text = sr.ReadToEnd();
                filterTrailingComma(ref text);
                addComma(ref text);

                AdofaiFile = JsonConvert.DeserializeObject<AdofaiBeatmap>(text);
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
    }
}
