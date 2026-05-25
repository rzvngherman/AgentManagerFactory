using ConsoleApp3.Agents;
using Microsoft.Agents.AI;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Configuration;

namespace Library5_Qwen_and_Ollama.Agents;

public class TogetherAIAgent: IAgentManager
{
    private readonly string _system_prompt;
    private readonly bool _useMock;
    private readonly string _api_key;

    private const string _api_endpoint = "https://api.together.xyz/v1";
    private const string _model_name = "LiquidAI/LFM2-24B-A2B";

    public TogetherAIAgent(string system_prompt, bool useMock)
    {
        _useMock = useMock;
        _system_prompt = system_prompt;
        _api_key = ConfigurationManager.AppSettings["TogetherAI_apiKey"];
    }

    public async Task<string> RunAsync(string user_input)
    {
        if (_useMock)
        {
            return "Tokyo, Japan is currently 14 hours ahead of Coordinated Universal Time (UTC+14)" +
                ", but since Daylight Saving Time (DST) was abolished in Japan in 2011, the standard time offset for Japan is UTC+9" +
                ". \r\n\r\nAssuming the current time, it would be 9 hours ahead of UTC. However, please note that without the current date" +
                ", I can only provide the general UTC offset for Tokyo, which is UTC+9. To know the exact current UTC time" +
                ", you would need to consider the current time in Tokyo and subtract 9 hours.";

        }

        OpenAIClient client = new(new ApiKeyCredential(_api_key), new OpenAIClientOptions
        {
            Endpoint = new Uri(_api_endpoint)
        });
        ChatClientAgent agent = client.GetChatClient(_model_name).AsAIAgent();
        AgentResponse response = await agent.RunAsync(user_input);

        Console.WriteLine(response);

        return response.Text;
    }
}
