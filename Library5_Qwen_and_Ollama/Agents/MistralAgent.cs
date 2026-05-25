using ConsoleApp3.Agents;
using Mistral.SDK;
using Mistral.SDK.DTOs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library5_Qwen_and_Ollama.Agents;

internal class MistralAgent : IAgentManager
{
    private readonly string _system_prompt;
    private readonly bool _useMock;

    public MistralAgent(string system_prompt, bool useMock)
    {
        _useMock = useMock;
        _system_prompt = system_prompt;
    }

    public async Task<string> RunAsync(string user_input)
    {
        if (_useMock)
        {
            //"What time is now in Tokyo, Japan?" -- maxim 10 words
            return "UTC+9. Check current time for exact hours and minutes.";
        }



        string apiKey = ConfigurationManager.AppSettings["Mistral_apiKey"];

        // Create client
        var client = new MistralClient(apiKey);

        // Create request
        var request = new ChatCompletionRequest
        {
            Model = "mistral-small-2506", //"mistral-large-latest",
            Messages =
            [
                new ChatMessage
                {
                    Role = ChatMessage.RoleEnum.System,
                    Content = _system_prompt
                },
                new ChatMessage
                {
                    Role = ChatMessage.RoleEnum.User,
                    Content = user_input
                }
            ],
            //Temperature = 0.7
        };

        // Send request
        var response = await client.Completions.GetCompletionAsync(request);

        // Print response
        Console.WriteLine(response.Choices[0].Message.Content);

        return response.Choices[0].Message.Content;
    }
}
