using ConsoleApp3.Agents;
using Library5_Qwen_and_Ollama.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3.FactoryPattern;

public class AgentManagerFactory
{
    private readonly string _system_prompt;
    private readonly bool _useMockData;
    public AgentManagerFactory(string system_prompt, bool useMockData = true)
    {
        _system_prompt = system_prompt;
        _useMockData = useMockData;
    }

    /// <summary>
    /// https://www.youtube.com/watch?v=Lt4P8yU7tEA
    /// "How to connect to 15 different model providers - AI in C# (Microsoft Agent Framework)"
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public IAgentManager GetAgentManager(AgentType type)
    {
        switch (type)
        {
            case AgentType.Qwen:
                return new QwenAgent(_system_prompt, _useMockData);

            case AgentType.Ollama:
                return new OllamaAgent(_system_prompt, _useMockData);

            case AgentType.Gemini:
                return new GeminiAgent(_system_prompt, _useMockData);

            case AgentType.Claude:
                return new AnthropicClaudeAgent(_system_prompt, _useMockData);

            case AgentType.Mistral:
                return new MistralAgent(_system_prompt, _useMockData);

            case AgentType.AmazonBedrock:
                return new AmazonBedrockAgent(_system_prompt, _useMockData);

            case AgentType.X_AI_Grok:
                return new GrokAgent(_system_prompt, _useMockData);

            case AgentType.MicrosoftFoundry:
                return new MicrosoftFoundryAgent(_system_prompt, _useMockData);

            case AgentType.MicrosoftFoundryLOCAL:
                return new MicrosoftFoundryLOCALAgent(_system_prompt, _useMockData);

            case AgentType.TogetherAI:
                return new TogetherAIAgent(_system_prompt, _useMockData);

            case AgentType.GithubModels:
                return new GithubModelsAgent(_system_prompt, _useMockData);

            default:
                throw new NotSupportedException();
        }
    }
}

public enum AgentType
{
    Ollama,
    Qwen,
    Gemini,
    Claude,
    Mistral,
    AmazonBedrock,
    X_AI_Grok,
    MicrosoftFoundry,
    MicrosoftFoundryLOCAL,
    TogetherAI,
    GithubModels
}
