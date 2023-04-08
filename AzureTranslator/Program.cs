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
    string inputText = Console.ReadLine();
    object[] body = new object[] { new { Text = inputText } };
    var requestBody = JsonConvert.SerializeObject(body);

    var endpoint = config["translator:endpoint"];
    var route = config["translator:route"];

    using (var client = new HttpClient())
    using (var request = new HttpRequestMessage())
    {
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri(endpoint + route);
        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        request.Headers.Add("Ocp-Apim-Subscription-Key", config["translator:key"]);
        request.Headers.Add("Ocp-Apim-Subscription-Region", config["translator:location"]);
        var response = await client.SendAsync(request).ConfigureAwait(false);
        var result = await response.Content.ReadAsStringAsync();
        var responseModel = JsonConvert.DeserializeObject<List<TranslateModelResponse>>(result);

        string resultsJoin = string.Join(", ", responseModel!.SelectMany(x => x.Translations.Select(x => x.Text)));
        Console.WriteLine($"Перевод: {resultsJoin} ");
    }
}