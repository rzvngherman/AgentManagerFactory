using GenerativeAI.Microsoft;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
// nuget Google_GenerativeAI.Microsoft

namespace ConsoleApp3.Agents;

internal class GeminiAgent : IAgentManager
{
    private readonly IChatClient _chatClient;
    private readonly Microsoft.Agents.AI.ChatClientAgent _agent;
    private readonly string _system_prompt;
    private readonly bool _useMock;

    /// <summary>
    /// Get key from here:
    /// https://aistudio.google.com/app/api-keys
    /// </summary>
    /// <param name="system_prompt"></param>
    /// <param name="useMock"></param>
    public GeminiAgent(string system_prompt, bool useMock)
    {
        _useMock = useMock;
        _system_prompt = system_prompt;

        string apiKey = ConfigurationManager.AppSettings["Gemini_apiKey"];
        _chatClient = new GenerativeAIChatClient(apiKey);
        
        _agent = new(_chatClient);
    }

    /// <summary>
    /// error 1:
    /// INVALID_ARGUMENT (Code: 400): API key not valid. Please pass a valid API key
    /// </summary>
    /// <param name="user_input"></param>
    /// <returns></returns>
    public async Task<string> RunAsync(string user_input)
    {
        string AI_message = "";
        if (_useMock)
        {
            //"What time is now in Tokyo, Japan?" -- maxim 10 words
            return "The current time in Tokyo, Japan is **1:42 PM on Friday, June 7, 2024 (JST)**.";

            //"What time is now in Tokyo, Japan, in UTC."
            /*
            The current time in UTC is:

**[Current UTC Time]**

(For example, if it were 10:30 AM on October 27, 2023 in UTC, I would say "10:30 AM UTC on Friday, October 27, 2023")

The time in UTC is universal and does not change based on your location (like Tokyo). Tokyo's local time (JST) is UTC+9.
            */
        }

        try
        {
            AgentResponse response1 = await _agent.RunAsync(user_input);
            AI_message = response1.Text;
        }
        catch (Exception ex)
        {
            //TODO - log exception

            AI_message = ex.Message;
        }

        return AI_message;
    }
}
