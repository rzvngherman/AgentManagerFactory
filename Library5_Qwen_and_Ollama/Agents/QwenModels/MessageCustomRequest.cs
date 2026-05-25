using System.Xml.Serialization;

namespace ConsoleApp3.Agents.QwenModels;

public class MessageCustomRequest
{
    public string role { get; set; }   // "system", "user", "assistant"
    public string content { get; set; } // <tool_call><function=get_weather><parameter=city>Paris</parameter></function></tool_call>

    //public object[] tool_calls { get; set; }
    public ToolCallResponse[] tool_calls { get; set; }

    public object reasoning { get; set; }

    public string tool_call_id { get; set; }
    public string type { get; set; }
}

[XmlRoot("tool_call")]
public class ToolCallResponse
{
    [XmlElement("id")]
    public string id { get; set; }

    [XmlElement("function")]
    public FunctionResponse function { get; set; }

    public string type { get; set; }
}

public class FunctionResponse
{
    [XmlAttribute("name")]
    public string name { get; set; }

    [XmlElement("parameter")]
    public string arguments { get; set; }
}




public class ChatRequestCustom
{
    public string model { get; set; }
    public MessageCustomRequest[] messages { get; set; }

    public ToolCustom[] tools { get; set; }
    public object tool_choice { get; set; }    // optional
    public bool enable_thinking { get; set; }
}

public class ToolCustom
{
    public string type { get; set; } = "function";
    public FunctionCustom function { get; set; }
}

public class FunctionCustom
{
    public string name { get; set; }
    public string description { get; set; }
    public object parameters { get; set; } // JSON Schema
}