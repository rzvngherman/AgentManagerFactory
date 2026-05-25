using ConsoleApp3.FactoryPattern;
using Microsoft.Agents.AI;
using System.Configuration;

namespace ConsoleApp3
{
    internal class Program
    {
        private readonly AgentManagerFactory _agentManagerFactory;
        private readonly bool _useMockResponse;

        private string _prompt = "You are a time expert. Give a short response, MAXIM 20 words.";
        private string _userText = "What time is now in Tokyo, Japan, in UTC.";

        public Program()
        {
            string use_real_data = ConfigurationManager.AppSettings["use_real_data"];

            _useMockResponse = (!string.IsNullOrEmpty(use_real_data) && (use_real_data == "1" || use_real_data.ToLower() == "true"))
                ? false
                : true;
            _agentManagerFactory = new AgentManagerFactory(_prompt, _useMockResponse);
        }

        private async Task MainNotStatic(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"\nUse mock data: (set 'use_real_data' in config file)");
            Console.ResetColor();
            Console.WriteLine(_useMockResponse);

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\nPrompt:");
            Console.ResetColor();
            Console.WriteLine($"{_prompt}");

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\nUser text:");
            Console.ResetColor();
            Console.WriteLine($"{_userText}");

            await GetAgentResponse(AgentType.Qwen);
            await GetAgentResponse(AgentType.Ollama);
            await GetAgentResponse(AgentType.Gemini);

            await GetAgentResponse(AgentType.Claude); //no free tier !!!
            await GetAgentResponse(AgentType.Mistral);
            await GetAgentResponse(AgentType.AmazonBedrock);

            await GetAgentResponse(AgentType.X_AI_Grok);

            await GetAgentResponse(AgentType.MicrosoftFoundry);
            await GetAgentResponse(AgentType.MicrosoftFoundryLOCAL);

            // OpenRouter, namespace 'Microsoft.Agents.AI.OpenAI'
            // https://openrouter.ai/settings/keys
            //--> need to buy credits for create API keys


            await GetAgentResponse(AgentType.TogetherAI);
            await GetAgentResponse(AgentType.GithubModels);
                       


            //TODO
            // cohere, namespace 'Microsoft.Agents.AI.OpenAI'

            //TODO
            // AzureOpenAIClient 'namespace Azure.AI.OpenAI



            //TODO
            // hugging face, namespace 'Microsoft.Agents.AI.OpenAI'



            //TODO
            // open AI



        }

        private async Task GetAgentResponse(AgentType agentType)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"\n'{agentType}' response:");
            Console.ResetColor();

            var agent = _agentManagerFactory.GetAgentManager(agentType);
            string agentResponse = await agent.RunAsync(_userText);

            Console.WriteLine(agentResponse);
            Console.WriteLine();
        }

        static async Task Main(string[] args)
        {
            await new Program().MainNotStatic(args);
        }
    }
}
