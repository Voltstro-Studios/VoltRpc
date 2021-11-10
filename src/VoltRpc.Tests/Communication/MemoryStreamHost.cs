using System.Threading.Tasks;
using VoltRpc.Communication;
using VoltRpc.IO;

namespace VoltRpc.Tests.Communication;

public class MemoryStreamHost : Host
{
    private readonly BufferedReader reader;
    private readonly BufferedWriter writer;

    public MemoryStreamHost(BufferedReader bufferedReader, BufferedWriter bufferedWriter)
    {
        reader = bufferedReader;
        writer = bufferedWriter;
    }

    public override Task StartListening()
    {
        ProcessRequest(reader, writer);
        return Task.CompletedTask;
    }
}