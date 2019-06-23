using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace DyTrailer {
    public class UtilClass {
        public static string CleanTrailerName (string trailerName) {
            Regex pattern = new Regex ("[\\~#%&*{}/:<>?|\"-]");
            trailerName = pattern.Replace (trailerName, "");

            trailerName = trailerName.Normalize (NormalizationForm.FormD);
            var trailerNameBuilder = new StringBuilder ();

            foreach (var c in trailerName) {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory (c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark) {
                    trailerNameBuilder.Append (c);
                }
            }

            return trailerNameBuilder.ToString ().Normalize (NormalizationForm.FormC);
        }

        public static dynamic GetDynamicJson (string url) {
            var client = new RestClient (url);
            var request = new RestRequest (Method.GET);

            string response = client.Execute (request).Content;
            return JObject.Parse (response);
        }
    }
}