using Amazon.BedrockRuntime;
using ConsoleApp3.Agents;
using Microsoft.Agents.AI;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Library5_Qwen_and_Ollama.Agents
{
    internal class GrokAgent : IAgentManager
    {
        private readonly string _system_prompt;
        private readonly bool _useMock;

        /// <summary>
        /// X AI (Grok)
        /// </summary>
        public GrokAgent(string system_prompt, bool useMock)
        {
            _useMock = useMock;
            _system_prompt = system_prompt;
        }

        public async Task<string> RunAsync(string user_input)
        {
            if (_useMock)
            {
                return "{\"code\":\"The caller does not have permission to execute the specified operation\"" +
                    ",\"error\":\"Your newly created team doesn't have any credits or licenses yet. " +
                    "You can purchase those on https://console.x.ai/team/bd87f2b0-b5cb-4363-9bcb-6586220f5b77.\"}";
            }

            string api_key = ConfigurationManager.AppSettings["Grok_apiKey"];
            string modelName = "grok-4.20-0309-non-reasoning";

            string AI_message = "";
            try
            {

                using var client2 = new HttpClient();
                client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", api_key);

                var requestBody = new
                {
                    model = modelName,
                    instructions = _system_prompt,
                    max_output_tokens = 1000000,
                    stream = true,
                    input = new object[]
                    {
                        new
                        {
                            role = "user",
                            content = user_input
                        }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);

                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var response2 = await client2.PostAsync(
                    "https://api.x.ai/v1/responses",
                    content);

                var responseBody = await response2.Content.ReadAsStringAsync();
                //response2.EnsureSuccessStatusCode();

                AI_message = responseBody;
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
}
