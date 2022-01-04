namespace VoltRpc.Tests.TestObjects.Interfaces;

public interface IRefOutInterface
{
    public void RefOutBasic(ref int value, out int outValue);
}