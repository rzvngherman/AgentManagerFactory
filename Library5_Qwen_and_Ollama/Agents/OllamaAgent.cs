
using Microsoft.Extensions.AI;
using OllamaSharp;
using System.Configuration;

namespace ConsoleApp3.Agents;

public class OllamaAgent : IAgentManager
{
    private readonly IChatClient _chatClient;
    private readonly Microsoft.Agents.AI.ChatClientAgent _agent;
    private readonly string _system_prompt;
    private readonly bool _useMock;

    public OllamaAgent(string system_prompt, bool useMock)
    {
        _useMock = useMock;
        _system_prompt = system_prompt;
        string _model = ConfigurationManager.AppSettings["Ollama_model"];
        string _uri_str = ConfigurationManager.AppSettings["Ollama_url"];

        _chatClient = new OllamaSharp.OllamaApiClient(_uri_str, _model);

        _agent = _chatClient
             .AsBuilder()
             .UseFunctionInvocation()
             //.UseDistributedCache(new MemoryDistributedCache(
             //    Options.Create(new MemoryDistributedCacheOptions())))
             .Build()
             .AsAIAgent(
                instructions: system_prompt
                //, tools: listoftools
                );
    }
    public async Task<string> RunAsync(string user_input)
    {
        string AI_message = "";
        if (_useMock)
        {
            //"What time is now in Tokyo, Japan?" -- maxim 10 words
            AI_message = "Tokyo is currently at UTC+9, approximately 16:00 hours.";
        }
        else
        {
            try
            {
                Microsoft.Agents.AI.AgentResponse response1 = await _agent.RunAsync(user_input);
                AI_message = response1.ToString();
            }
            catch (Exception ex)
            {
                //TODO - log exception

                AI_message = ex.Message;
            }

        }
        
        return AI_message;
    }
}
