using Azure.AI.Extensions.OpenAI;
using Azure.AI.Projects;
using Azure.AI.Projects.Agents;
using Azure.Identity;
using ConsoleApp3.Agents;
using Microsoft.Agents.AI;
using OpenAI.Responses;
using static System.Net.Mime.MediaTypeNames;

#pragma warning disable OPENAI001

namespace Library5_Qwen_and_Ollama.Agents
{
    public class MicrosoftFoundryAgent : IAgentManager
    {
        const string endpoint = ConfigurationManager.AppSettings["MicrosoftFoundry_endpoint"];
        const string modelDeploymentName = "grok-4.3";
        const string agentName = "agnt-raz";

        private readonly string _system_prompt;
        private readonly bool _useMock;

        /// <summary>
        /// Microsoft Foundry, AIProjectClient, namespace 'Microsoft.Agents.AI.AzureAI
        /// </summary>
        public MicrosoftFoundryAgent(string system_prompt, bool useMock)
        {
            _useMock = useMock;
            _system_prompt = system_prompt;
        }

        public async Task<string> RunAsync(string user_input)
        {
            if (_useMock)
            {
                return "Tokyo is UTC+9 (JST). Current local time equals UTC now +9 hours.";
            }

            string AI_message = "";
            try
            {
                // Connect to your project using the endpoint from your project page
                // The AzureCliCredential will use your logged-in Azure CLI identity, make sure to run `az login` first
                AIProjectClient projectClient = new(endpoint: new Uri(endpoint), tokenProvider: new DefaultAzureCredential());

                // Create your agent
                PromptAgentDefinition agentDefinition = new(model: modelDeploymentName)
                {
                    Instructions = _system_prompt
                };

                // Creates an agent or bumps the existing agent version if parameters have changed
                AgentVersion agentVersion = projectClient.Agents.CreateAgentVersion(
                    agentName: agentName,
                    options: new(agentDefinition));
                //Console.WriteLine($"Agent created (id: {agentVersion.Id}, name: {agentVersion.Name}, version: {agentVersion.Version})");

                // To automatically store history, we can optionally create a conversation to use with the agent:
                ProjectConversation conversation = projectClient.OpenAI.Conversations.CreateProjectConversation();
                ProjectResponsesClient responseClient
                    = projectClient.OpenAI.GetProjectResponsesClientForAgent(new(name: agentVersion.Name, version: agentVersion.Version), conversation.Id);
                // Use the agent to generate a response
                ResponseResult response = responseClient.CreateResponse(user_input);
                AI_message = response.GetOutputText();
            }
            catch (Exception ex)
            {
                //TODO - log exception

                AI_message = ex.Message;
            }

            return AI_message;

        }
    }
}
