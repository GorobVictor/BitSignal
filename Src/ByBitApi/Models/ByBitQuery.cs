namespace ByBitApi.Models;

public class ByBitQuery
{
    public ByBitQuery()
    {
    }

    public ByBitQuery(string op, List<string> args)
    {
        Op = op;
        Args = args;
    }

    public string Op { get; set; }
    public List<string> Args { get; set; }

    public static ByBitQuery Create(List<string> args)
    {
        return new ByBitQuery {Op = "subscribe", Args = args };
    }
}