using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using ConsoleApp3.Agents;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System;
using System.Configuration;

namespace Library5_Qwen_and_Ollama.Agents;

/// <summary>
/// Uses 'Microsoft.Agents.AI.Anthropic'
/// and 'Anthropic.SDK'
/// </summary>
internal class AnthropicClaudeAgent : IAgentManager
{
    private readonly string _system_prompt;
    private readonly bool _useMock;
    public AnthropicClaudeAgent(string system_prompt, bool useMock)
    {
        _useMock = useMock;
        _system_prompt = system_prompt;
    }

    public async Task<string> RunAsync(string user_input)
    {
        if (_useMock)
        {
            return "{\"type\":\"error\",\"error\":{\"type\":\"invalid_request_error\",\"message\":\"Your credit balance is too low to access the Anthropic API. Please go to Plans & Billing to upgrade or purchase credits.\"},\"request_id\":\"req_011CbDYir5HshSWkZeoZL2yy\"}";
        }


        var client = new AnthropicClient(ConfigurationManager.AppSettings["Claude_apiKey"]);

        var messages = new List<Message>
        {
            new Message(RoleType.User, user_input)
        };

        var parameters = new MessageParameters()
        {
            Messages = messages,
            MaxTokens = 1024,
            Model = "claude-opus-4-7", //AnthropicModels.Claude46Opus,
            Stream = false,
            Temperature = 0.7m,
            System = new List<SystemMessage> { new SystemMessage(_system_prompt) }
        };

        string AI_message = "";

        try
        {
            var response = await client.Messages.GetClaudeMessageAsync(parameters);
            // : '{"type":"error","error":{"type":"invalid_request_error","message":"Your credit balance is too low to access the Anthropic API. Please go to Plans & Billing to upgrade or purchase credits."},"request_id":"req_011CbA5VpNLK1vbaMy8CNpS1"}'

            Console.WriteLine(response.Message);
            //TODO - return

        }
        catch (Exception ex)
        {
            //TODO - log exception

            AI_message = ex.Message;
        }

        return AI_message;
    }
}
