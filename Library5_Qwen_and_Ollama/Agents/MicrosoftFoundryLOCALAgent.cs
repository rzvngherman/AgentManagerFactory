//using Betalgo.Ranul.OpenAI;
using ConsoleApp3.Agents;
using Microsoft.Agents.AI;
using Microsoft.AI.Foundry.Local;
using Microsoft.Extensions.Logging.Abstractions;
using OpenAI;
using System.ClientModel;
using System.Text;
using static Betalgo.Ranul.OpenAI.ObjectModels.RealtimeModels.RealtimeEventTypes;


namespace Library5_Qwen_and_Ollama.Agents;

public class MicrosoftFoundryLOCALAgent : IAgentManager
{
    private readonly string _system_prompt;
    private readonly bool _useMock;

    /// <summary>
    /// Microsoft Foundry local data (like ollama local)
    /// Fountry local, namespace 'Microsoft.AI.Foundry.Local' (local host from microsoft; same as ollama)
    /// winget install Microsoft.FoundryLocal
    /// https://learn.microsoft.com/en-us/windows/ai/foundry-local/get-started
    /// Microsoft.AI.Foundry.Local.Core.WinML
    /// Betalgo.Ranul.OpenAI
    /// </summary>
    public MicrosoftFoundryLOCALAgent(string system_prompt, bool useMock)
    {
        _useMock = useMock;
        _system_prompt = system_prompt;
    }

    public async Task<string> RunAsync(string user_input)
    {
        if (_useMock)
        {
            return "---";
        }

        DateTime dt1 = DateTime.UtcNow;
        DateTime dt2 = DateTime.UtcNow;

        // 1. Initialize Foundry Local. The SDK starts the service automatically if needed.
        await FoundryLocalManager.CreateAsync(
            new Configuration { AppName = "my-app" },
            NullLogger.Instance);

        var manager = FoundryLocalManager.Instance;
        try
        {
            // 2. Look up the model in the catalog by alias.
            var catalog = await manager.GetCatalogAsync();
            var model = await catalog.GetModelAsync("phi-3.5-mini")
                ?? throw new Exception(
                    "Model 'phi-3.5-mini' not found in catalog. " +
                    "Ensure Foundry Local is installed and has internet access.");

            // 3. Download the model if it is not already cached (2.53 GB).
            if (!await model.IsCachedAsync())
            {
                //Console.Write("Downloading phi-3.5-mini...");
                await model.DownloadAsync(progress =>
                {
                    //Console.Write($"\rDownloading phi-3.5-mini  {progress,5:F1}%");
                });
                //Console.WriteLine();
            }

            // 4. Load the model into memory.
            await model.LoadAsync();

            //Betalgo.Ranul.OpenAI.Contracts.Enums.ReasoningEffort
           //Microsoft.Extensions.AI.ReasoningEffort

            // 5. Run a chat completion.
            var chatClient = await model.GetChatClientAsync();

            // System.TypeLoadException: Could not load type 'ReasoningEfforts' from
            // assembly 'Betalgo.Ranul.OpenAI, Version=9.2.6.0, Culture=neutral, PublicKeyToken=null'.



















            var cmSystem = Betalgo.Ranul.OpenAI.ObjectModels.RequestModels.ChatMessage.FromSystem(_system_prompt);
            var cmUser = Betalgo.Ranul.OpenAI.ObjectModels.RequestModels.ChatMessage.FromUser(user_input);


            bool useStream = true;
            if (useStream)
            {
                StringBuilder sb = new StringBuilder();

                using var cts = new CancellationTokenSource();

                dt1 = DateTime.UtcNow;

                await foreach (var chunk in chatClient.CompleteChatStreamingAsync(
                    new[] {
                            cmSystem,
                            cmUser
                            },
                    cts.Token))
                {
                    if (chunk.Choices != null && chunk.Choices.Count > 0)
                    {
                        //Console.Write(chunk.Choices?[0]?.Message?.Content);
                        sb.Append(chunk.Choices?[0]?.Message?.Content);
                    }
                    else
                    {
                        // ?
                    }
                }

                //Console.WriteLine();
                dt2 = DateTime.UtcNow;

                // {20-May-02026 3:03:09 PM}
                // {20-May-02026 3:06:11 PM}

                return sb.ToString();
            }
            else
            {
                //NON stream
                
                var response = await chatClient.CompleteChatAsync(new[]
                {
                    //new OpenAI.Chat.SystemChatMessage { Role = "system", Content = _system_prompt },
                    cmSystem,
                    cmUser
            });

                if (!response.Successful)
                    throw new Exception(
                        $"Chat completion failed: {response.Error?.Message ?? "unknown error"} " +
                        $"(code: {response.Error?.Code})");

                var content = response.Choices![0].Message.Content;
                if (string.IsNullOrEmpty(content))
                    throw new Exception(
                        "Model returned empty content. " +
                        "Verify that your device has a DirectX 12-capable GPU. " +
                        "Virtual machines without GPU passthrough are not supported.");

                //Console.WriteLine(content);
                return content;
            }









        }
        catch (Exception ex)
        {
            dt2 = DateTime.UtcNow;

            //3)
            // {"Index was out of range. Must be non-negative and less than the size of the collection. (Parameter 'index')"}

            //2)
            // {"Error from chat_completions command: Operation was cancelled\0"}

            //1)
            //System.TypeLoadException: Could not load type 'ReasoningEfforts' from assembly 'Betalgo.Ranul.OpenAI, Version=9.2.6.0, Culture=neutral, PublicKeyToken=null'.

            //TODO - log message

            return ex.Message;
        }
        finally
        {
            // 6. Clean up — always runs even if an earlier step throws.
            manager.Dispose();
        }
    }
}
