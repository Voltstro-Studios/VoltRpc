# Communication Layers

Currently two communication layers are provided.

- TCP
- Named Pipes

## TCP

- TCP is provided within the base VoltRpc library.
- TCP uses the .NET provided <xref:System.Net.Sockets.TcpListener> and <xref:System.Net.Sockets.TcpClient>. 
- It has the most compatibly between systems. 
- It is however the slowest as shown from the [benchmarks](benchmarks.md#tcp-benchmark).

It can be used with <xref:VoltRpc.Communication.TCP.TCPHost> and <xref:VoltRpc.Communication.TCP.TCPHost> classes.

## Pipes

Named Pipes is provided from the [VoltRpc.Communication.Pipes NuGet package](https://www.nuget.org/packages/VoltRpc.Communication.Pipes/).

You can use the command below to install the package.

```powershell
Install-Package VoltRpc.Communication.Pipes
``` 

- Pipes uses the .NET provided <xref:System.IO.Pipes.NamedPipeServerStream> and <xref:System.IO.Pipes.NamedPipeClientStream>.  
- It is currently the fastest communication layer as shown from the [benchmarks](benchmarks.md#pipes-benchmark).
- Some implementations such as [Unity's mono](https://github.com/Unity-Technologies/mono/blob/unity-2021.1-mbe/mcs/class/System.Core/System.IO.Pipes/NamedPipeClientStream.cs) may just throw <xref:System.NotImplementedException> on certain platforms.

It can be used with <xref:VoltRpc.Communication.Pipes.PipesHost> and <xref:VoltRpc.Communication.Pipes.PipesHost> classes.