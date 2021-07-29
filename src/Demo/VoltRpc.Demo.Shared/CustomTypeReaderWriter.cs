using VoltRpc.IO;
using VoltRpc.Types;

namespace VoltRpc.Demo.Shared
{
    public class CustomTypeReaderWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            CustomType customType = (CustomType) obj;
            writer.WriteFloat(customType.Floaty);
            writer.WriteString(customType.Message);
        }

        public object Read(BufferedReader reader)
        {
            return new CustomType
            {
                Floaty = reader.ReadFloat(),
                Message = reader.ReadString()
            };
        }
    }
}