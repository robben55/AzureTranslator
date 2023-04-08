using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using AzureTranslator.Services;

namespace AzureTranslator
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var translator = host.Services.GetRequiredService<AzureTranslatorService>();
            while (true)
            {
                Console.Write("Введите текст для перевода:");
                string inputText = Console.ReadLine();

                string translationText = await translator.TranslateAsync(inputText);
                Console.WriteLine($"Перевод: {translationText}");
            }
        }
        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args).ConfigureServices((context, collection) =>
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<Program>();

            var config = builder.Build();
            collection.AddHttpClient("translator", client =>
            {
                client.BaseAddress = new Uri("https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&from=ru&to=en");
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", config["translator:key"]);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", config["translator:location"]);
            });
            collection.AddSingleton<AzureTranslatorService>();
        });
    }
}