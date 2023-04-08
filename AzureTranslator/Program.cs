using AzureTranslator;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>();

var config = builder.Build();

while (true)
{
    Console.Write("Введите текст для перевода:");
    object[] body = new object[] { new { Text = Console.ReadLine() } };
    var requestBody = JsonConvert.SerializeObject(body);
    var responseModel = JsonConvert.DeserializeObject<List<TranslateModelResponse>>(await PostHttpResponseMessage(requestBody));
    string resultsJoin = string.Join(", ", responseModel!.SelectMany(x => x.Translations.Select(s => s.Text)));
    Console.WriteLine($"Перевод: {resultsJoin} ");
}


async Task<string> PostHttpResponseMessage(string word)
{
    var result = string.Empty;
    using (var client = new HttpClient())
    using (var request = new HttpRequestMessage())
    {
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri(config["translator:endpoint"]+config["translator:route"]);
        request.Content = new StringContent(word, Encoding.UTF8, "application/json");
        request.Headers.Add("Ocp-Apim-Subscription-Key", config["translator:key"]);
        request.Headers.Add("Ocp-Apim-Subscription-Region", config["translator:location"]);
        var response = await client.SendAsync(request).ConfigureAwait(false);
        result = await response.Content.ReadAsStringAsync();
    }

    return result;
}