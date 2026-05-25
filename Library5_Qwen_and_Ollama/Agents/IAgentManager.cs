namespace ConsoleApp3.Agents
{
    public interface IAgentManager
    {
        Task<string> RunAsync(string user_input);
    }
}
