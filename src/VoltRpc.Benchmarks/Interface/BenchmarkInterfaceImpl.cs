using System;
using VoltRpc.Benchmarks.Core;
using VoltRpc.Benchmarks.Core.FastArray;

namespace VoltRpc.Benchmarks.Interface;

public class BenchmarkInterfaceImpl : IBenchmarkInterface
{
    private readonly byte[] smallArray;

    private FastArrayContainer fastArrayContainer;

    public BenchmarkInterfaceImpl(byte[] smallArray, byte[] largeArray)
    {
        this.smallArray = smallArray;
        fastArrayContainer = new()
        {
            Data = largeArray
        };
    }

    public void BasicVoid()
    {
    }

    public void BasicParameterVoid(string message)
    {
    }

    public string BasicReturn()
    {
        return "Hello World!";
    }

    public string BasicParameterReturn(string message)
    {
        return "Hello World!";
    }

    public void ArrayParameterVoid(byte[] array)
    {
    }

    public byte[] ArrayReturn()
    {
        return smallArray;
    }

    public byte[] ArrayParameterReturn(byte[] array)
    {
        return array;
    }

    public FastArrayContainer ArrayFast()
    {
        return fastArrayContainer;
    }
}