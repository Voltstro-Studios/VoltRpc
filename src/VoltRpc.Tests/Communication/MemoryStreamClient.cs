using VoltRpc.Communication;
using VoltRpc.IO;

namespace VoltRpc.Tests.Communication;

public class MemoryStreamClient : Client
{
    private readonly BufferedReader reader;
    private readonly BufferedWriter writer;
    
    public MemoryStreamClient(BufferedReader bufferedReader, BufferedWriter bufferedWriter)
    {
        reader = bufferedReader;
        writer = bufferedWriter;
    }
    
    public override void Connect()
    {
        Initialize(reader, writer);
    }
}