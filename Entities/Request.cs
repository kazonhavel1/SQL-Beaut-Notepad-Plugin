using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;

internal class Request
{
    private static readonly HttpClient httpClient = new HttpClient(); // Aqui começamos a instacia da classe HTTP

    public static async Task<string> enviaRequisicao(string txt)
    {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var request = new HttpRequestMessage(HttpMethod.Post, "https://sqlformat.org/api/v1/format"); //Link da RQ

            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("sql", $"{txt}"));
            collection.Add(new("reindent", "1"));
            collection.Add(new("identifier_case", "upper")); 
            collection.Add(new("keyword_case", "upper"));   //Criação do Objeto para envio

            var content = new FormUrlEncodedContent(collection); //Decodificação para FormURL
            request.Content = content;

            HttpResponseMessage resposta = await httpClient.SendAsync(request); // envio da RQ

            string respostaString = await resposta.Content.ReadAsStringAsync();
            if (resposta.StatusCode != System.Net.HttpStatusCode.OK)
            {

            return $"Erro {resposta.StatusCode.GetHashCode()} {resposta.StatusCode} ao processar a requisição: {respostaString}";

            }
            else { return respostaString; }
    }
}