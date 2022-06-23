using System;
using System.Numerics;
using VoltRpc.Proxy;

namespace VoltRpc.Demo.Shared;

/// <summary>
///     Testing interface for VoltRpc demo
/// </summary>
[GenerateProxy(GeneratedName = "TestProxy")]
public interface ITest
{
    /// <summary>
    ///     Basic void
    /// </summary>
    public void BasicTest();

    /// <summary>
    ///     Basic parameter
    /// </summary>
    /// <param name="message"></param>
    /// <param name="num"></param>
    public void ParmTest(string message, float num);

    /// <summary>
    ///     Basic return
    /// </summary>
    /// <returns></returns>
    public string ReturnTest();

    /// <summary>
    ///     Basic array test
    /// </summary>
    /// <param name="array"></param>
    public void ArrayTest(string[] array);

    /// <summary>
    ///     Basic ref test
    /// </summary>
    /// <param name="refTest"></param>
    public void RefTest(ref string refTest);

    /// <summary>
    ///     Basic ref and return test
    /// </summary>
    /// <param name="refTest"></param>
    /// <returns></returns>
    public byte RefReturnTest(ref uint refTest);

    /// <summary>
    ///     Basic out test
    /// </summary>
    /// <param name="outTest"></param>
    public void OutTest(out string outTest);

    /// <summary>
    ///     Basic custom type test
    /// </summary>
    /// <param name="customType"></param>
    public void CustomTypeTest(CustomType customType);

    /// <summary>
    ///     Basic testing using <see cref="Guid"/>
    /// </summary>
    /// <param name="guid"></param>
    public void GuidTest(Guid guid);

    /// <summary>
    ///     Basic custom type return
    /// </summary>
    /// <returns></returns>
    public CustomType CustomTypeReturnTest();

    /// <summary>
    ///     Basic custom array type thingy test
    /// </summary>
    /// <returns></returns>
    public CustomTypeArrays CustomTypeArraysSmall();

    /// <summary>
    ///     Basic test using a <see cref="Vector3"/>
    /// </summary>
    /// <returns></returns>
    public Vector3 Vector3TypeReturnTest();
}