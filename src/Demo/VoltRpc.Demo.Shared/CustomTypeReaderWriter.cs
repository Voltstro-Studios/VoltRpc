using VoltRpc.IO;
using VoltRpc.Types;

namespace VoltRpc.Demo.Shared;

public class CustomTypeReaderWriter : TypeReadWriter<CustomType>
{
    public override void Write(BufferedWriter writer, CustomType obj)
    { ;
        writer.WriteFloat(obj.Floaty);
        writer.WriteString(obj.Message);
    }

    public override CustomType Read(BufferedReader reader)
    {
        return new CustomType
        {
            Floaty = reader.ReadFloat(),
            Message = reader.ReadString()
        };
    }
}