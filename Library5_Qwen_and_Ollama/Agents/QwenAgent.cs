
using ConsoleApp3.Agents.QwenModels;
using System;
using System.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ConsoleApp3.Agents;

public class QwenAgent : IAgentManager
{
    private readonly QwenHttpClient _customHttpClient;
    private readonly bool _useMock;

    public QwenAgent(string system_prompt, bool useMock)
    {
        _customHttpClient = new QwenHttpClient(system_prompt);
        _useMock = useMock;
    }

    public async Task<string> RunAsync(string user_input)
    {
        string responseString = "";
        if (_useMock)
        {
            //"What time is now in Tokyo, Japan, in UTC." -- maxim 20 words
            //-> "Current Tokyo time: 12:45 PM (Japan Standard Time)"

            responseString = "Tokyo, Japan is currently UTC+9. When it's 12:00 PM UTC, it's 9:00 PM in Tokyo.";
        }
        else
        {
            responseString = await _customHttpClient.PostAsync(user_input);
        }

        return responseString;
    }
}


public class QwenHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly string _uri;
    private readonly string _modelName;
    private readonly string _system_prompt;

    /// <summary>
    /// Using LEVIN
    /// </summary>
    /// <param name="system_prompt"></param>
    public QwenHttpClient(string system_prompt)
    {
        _system_prompt = system_prompt;

        string apiKey = ConfigurationManager.AppSettings["Qwen_apiKey"]; // Environment.GetEnvironmentVariable("Qwen_apiKey")
        _uri = ConfigurationManager.AppSettings["Qwen_url"];
        _modelName = ConfigurationManager.AppSettings["Qwen_model"];

        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.CacheControl = CacheControlHeaderValue.Parse("no-cache");
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
    }

    /// <summary>
    /// "{ \"statusCode\": 404, \"message\": \"Resource not found\" }"
    /// </summary>
    /// <param name="user_text"></param>
    /// <returns></returns>
    public async Task<string> PostAsync(string user_text)
    {
        var respMessagesAI = new List<MessageCustomRequest>
                    {
                        new MessageCustomRequest{ role = "system", content = _system_prompt },
                        new MessageCustomRequest{ role = "user", content = user_text }
                    };

        string requestBody = BuildRequest2(respMessagesAI);
        using (var strContent = new StringContent(requestBody, Encoding.UTF8, "application/json"))
        {
            string responseString = "";
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(_uri, strContent);

                responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //parse response:
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    ChatCustomResponse chatCustomResponse = JsonSerializer.Deserialize<ChatCustomResponse>(responseString, options);
                    var AI_message = chatCustomResponse?.choices?[0]?.message?.content;
                    return AI_message;
                }
            }
            catch (Exception ex)
            {
                //TODO - log data

                responseString = ex.Message;
            }

            return responseString;
        }
    }

    private string BuildRequest2(List<MessageCustomRequest> message_param, bool useTools = false)
    {
        // '/v1/chat/completions'

        //
        //If you send multiple consecutive "user" messages like this:

        //[
        //  { "role": "user", "content": "A"},
        //  { "role": "user", "content": "B"}
        //]

        //Most Qwen/ OpenAI - compatible servers treat it as:
        //"User said A, then user said B"
        //So the model usually responds to both in one combined answer, as shown above.





        var req = new ChatRequestCustom
        {
            model = _modelName,
            messages = message_param.ToArray(),
        };

        if (useTools)
        {
            //req.tools = GetTools();

            //req.tool_choice = new
            //{
            //    type = "function",
            //    function = new { name = "get_weather" }
            //};
        }

        var reqStr = JsonSerializer.Serialize(req);
        return reqStr;
    }
}