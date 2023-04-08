using System.Text.Json.Serialization;

namespace AzureTranslator
{
    public class TranslateModelResponse
    {
        [JsonPropertyName("translations")]
        public List<Translation> Translations { get; set; }
    }

    public class Translation
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("to")]
        public string To { get; set; }
    }


}
