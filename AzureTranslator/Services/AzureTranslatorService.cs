using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AzureTranslator.Models;

namespace AzureTranslator.Services
{
    public class AzureTranslatorService
    {
        private readonly IHttpClientFactory _factory;
        public AzureTranslatorService(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<string> TranslateAsync(string text)
        {
            var client = _factory.CreateClient("translator");
            var body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);
            var request = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress)
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };
            var response = await client.SendAsync(request).ConfigureAwait(false);
            string content = await response.Content.ReadAsStringAsync();
            var translationModel = JsonConvert.DeserializeObject<List<TranslateModelResponse>>(content)!;
            var translationTexts = translationModel.SelectMany(t => t.Translations.Select(t => t.Text));
            var result = string.Join(", ", translationTexts);
            return result;
        }
    }
}
