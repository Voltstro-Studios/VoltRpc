<img align="right" width="15%" src="media/icon.svg">

# VoltRpc

[![License](https://img.shields.io/github/license/Voltstro-Studios/VoltRpc)](/LICENSE.md)
[![Build Status](https://img.shields.io/azure-devops/build/Voltstro-Studios/63163ef8-da1d-42b6-b8b9-689420a730e5/9?logo=azure-pipelines)](https://dev.azure.com/Voltstro-Studios/VoltRpc/_build/latest?definitionId=9&branchName=master)
[![Discord](https://img.shields.io/badge/Discord-Voltstro-7289da.svg?logo=discord)](https://discord.voltstro.dev)

VoltRpc - Library designed for high performance RPC communication.

## Features

TODO

## Getting Started

### Installation

TODO

### Example

TODO

## Benchmarks

TODO

## Authors

**Voltstro** - *Initial work* - [Voltstro](https://github.com/Voltstro)

## License

This project is licensed under the MIT license – see the [LICENSE.md](/LICENSE.md) file for details.

## Credits

- [Mirror](https://github.com/vis2k/Mirror) 
  - [`NetworkReader.cs`](https://github.com/vis2k/Mirror/blob/ca4c2fd9302b1ece4240b09cc562e25bcb84407f/Assets/Mirror/Runtime/NetworkReader.cs) used as a base for [`BufferedReader.cs`](/src/VoltRpc/IO/BufferedReader.cs)
  - [`NetworkWriter.cs`](https://github.com/vis2k/Mirror/blob/ca4c2fd9302b1ece4240b09cc562e25bcb84407f/Assets/Mirror/Runtime/NetworkWriter.cs) used as a base for [`BufferedWriter.cs`](/src/VoltRpc/IO/BufferedWriter.cs)
- Parts of [`BufferedStream.cs`](https://github.com/dotnet/runtime/blob/release/5.0/src/libraries/System.Private.CoreLib/src/System/IO/BufferedStream.cs) from the .NET Runtime was also used in the reader. 
