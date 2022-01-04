namespace VoltRpc.Tests.TestObjects.Interfaces;

public interface IRefOutReturnInterface
{
    public int RefOutReturn(ref int value, out int outValue);
}