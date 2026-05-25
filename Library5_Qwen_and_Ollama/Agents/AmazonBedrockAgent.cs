using Amazon.BedrockRuntime;
using ConsoleApp3.Agents;
using Microsoft.Agents.AI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library5_Qwen_and_Ollama.Agents;

internal class AmazonBedrockAgent : IAgentManager
{
    private readonly string _system_prompt;
    private readonly bool _useMock;
    public AmazonBedrockAgent(string system_prompt, bool useMock)
    {
        _useMock = useMock;
        _system_prompt = system_prompt;
    }

    /// <summary>
    /// https://aws.amazon.com/bedrock
    /// </summary>
    /// <param name="user_input"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<string> RunAsync(string user_input)
    {
        if(_useMock)
        {
            return "Currently, Tokyo, Japan, is UTC+9. Local time is 20:36.";
        }

        string AI_message = "";
        try
        {
            string api_key = ConfigurationManager.AppSettings["AmazonBedrock_apiKey"];

            string modelName = "eu.amazon.nova-lite-v1:0"; // "amazon.nova-lite-v1:0"; //from Amazon
            Amazon.RegionEndpoint region = Amazon.RegionEndpoint.EUWest1; //ireland

            Environment.SetEnvironmentVariable("AWS_BEARER_TOKEN_BEDROCK", api_key);

            Amazon.BedrockRuntime.AmazonBedrockRuntimeClient amazonBedrockRuntimeClient = new(region);
            ChatClientAgent agent = new(amazonBedrockRuntimeClient.AsIChatClient(modelName));
            AgentResponse response = await agent.RunAsync(messages: new[]
            {
                new Microsoft.Extensions.AI.ChatMessage(
                    Microsoft.Extensions.AI.ChatRole.System,
                    _system_prompt),

                new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.User,
                    user_input)
            });

            AI_message = response.Text;
        }
        catch (Exception ex)
        {
            // Amazon.BedrockRuntime.Model.ValidationException: Invocation of model ID amazon.nova-lite-v1:0 with on-demand throughput isn’t supported
            // . Retry your request with the ID or ARN of an inference profile that contains this model.

            //TODO - log exception

            AI_message = ex.Message;
        }

        return AI_message;
    }
}
