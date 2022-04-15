using NUnit.Framework;
using VoltRpc.Tests.TestObjects.Interfaces;

namespace VoltRpc.Tests.TestObjects.Objects;

public class ArrayBasicInterface : IArrayBasicInterface
{
    private readonly bool shouldBeNull;
    
    public ArrayBasicInterface(bool shouldBeNull)
    {
        this.shouldBeNull = shouldBeNull;
    }
    
    public void Array(byte[] array)
    {
        if(shouldBeNull)
            Assert.IsNull(array);
        else
            Assert.AreEqual(new byte[]{1, 3 ,4, 8}, array);
    }
}