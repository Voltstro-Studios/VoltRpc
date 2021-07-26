using System.Threading.Tasks;
using VoltRpc.Communication;
using VoltRpc.IO;
using VoltRpc.Tests.IO;

namespace VoltRpc.Tests.Communication
{
    public class MemoryStreamHost : Host
    {
        private BufferedReader reader;
        private BufferedWriter writer;
        
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
}