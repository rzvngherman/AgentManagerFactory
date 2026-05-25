using ConsoleApp3.Agents;
using System.Configuration;

namespace Library5_Qwen_and_Ollama.Agents
{
    public class GithubModelsAgent : IAgentManager
    {
        private readonly string _system_prompt;
        private readonly bool _useMock;

        private readonly string _git_token;
        private const string _api_endpoint = "https://models.github.ai/inference";

        //TODO
        // github models
        // nuget 'Azure.AI.Inference'
        // https://github.com/settings/personal-access-tokens
        // uri: 'https://models.github.ai/inference'
        public GithubModelsAgent(string system_prompt, bool useMock)
        {
            _useMock = useMock;
            _system_prompt = system_prompt;
            _git_token = ConfigurationManager.AppSettings["GithubModels_token"];
        }

        public Task<string> RunAsync(string user_input)
        {
            throw new NotImplementedException();
        }
    }
}
