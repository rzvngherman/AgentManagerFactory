using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3.Agents.QwenModels;

public class ChatCustomResponse
{
    public ChoiceCustomResponse[] choices { get; set; }
}

public class ChoiceCustomResponse
{
    public MessageCustomRequest message { get; set; }
    //public OllamaSharp.Models.Chat.Message message { get; set; }
    //public Microsoft.Extensions.AI.ChatMessage message { get; set; }
}
