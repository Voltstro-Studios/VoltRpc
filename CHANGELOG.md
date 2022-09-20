# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.1.0] - 2022-09-20

### Added

- Added `ForcePublic` to GenerateProxy attribute

### Changed

- Support internal methods/interface
- Override values in GenerateProxy attribute will be checked to not to empty/whitespace
- Override values in GenerateProxy attribute are now specifically marked as nullable

## [3.0.0] - 2022-08-05

### Added

- Added internal version checking
    - VoltRpc hosts/clients will only work together if they are both the same version
- Added user protocol versions
- Added Guid type reader/writer
- Added SourceLink to packages (for better debug symbols)
- Added `Host.StartListeningAsync` method, which internally will call `Task.Run`
    - Can be overridden by Host implementations if needed
- Added missing `Vector4TypeReadWriter` from VoltRpc.Extension.Vectors

### Changed

- Changed `Host.StartListening` to void
    - Use `Host.StartListeningAsync().ConfigureAwait(false)` if you want the old behavior
- Updated package descriptions
- BufferReader can read from the underlying stream multiple times in one go (if needed)
- BufferReader can resize it's own buffer if need to
- `BufferReader/Writer.Length` getter is aggressively inlined
- `Client.InvokeMethod` was made `EditorBrowsableState.Never`
    - This should only be used by generated proxies anyway
- All type readers/writers for all types used in it need to be added before calling `AddService`
- `MessageResponse` and `MessageType` are now internal
    - You shouldn't be using theses anyway
- Update dispose in `BufferedReader` and `BufferedWriter`

### Fixed

- Proxy generator will add an `@` to the start of arguments, fixing problems with certain arguments names
- Proxy generator will give an error on Properties (as they are not supported)
- Fixed some more XML referencing the wrong things

## [2.1.0] - 2022-05-22

### Added

- Added VoltRpc.Extension.Vectors, which supports most types provided by System.Numerics.Vectors
- Added VoltRpc.Extension.Memory, which supports reading and writing `Span` and `Memory`
- Packages will now have symbols package
- Support null arrays

### Changed

- A lot of the methods in BufferedReader/Writer were changed to be extensions
- On .NET 6 (and higher), the buffers for BufferedReader and Writer are created using `GC.AllocateArray` and are pinned.

### Fixed

- Fixed some XML docs referencing the wrong objects

## [2.0.0] - 2022-01-06

### Added

- Added better array support, arrays are handled internally and don't need a custom type reader/writer for it anymore
- Added default type readers for `DateTime`, `TimeSpan` and `Uri`
- Added the ability to change a generated proxy's namespace

### Changed

- Changed `ConnectionFailed` to `ConnectionFailedException`
- Made some exception's constructors internal
- Updated some exception messages
- Moved `GenerateProxyAttribute` to the base VoltRpc assembly (instead of it being generated)
- Proxy generator no longer uses Scriban anymore
- Generated proxies are marked with a `GeneratedCode` attribute
- Made some methods in `TypeReaderWriterManager` internal
- Made `TypeReaderWriterManager` sealed
- Updated the way that type read/writers are implemented, they now need to inherit from `TypeReadWriter<T>` and override `Read` and `Write`. They Should look like this:

```csharp
using VoltRpc.IO;
using VoltRpc.Types;

public class CustomTypeReaderWriter : TypeReadWriter<CustomType>
{
    public override void Write(BufferedWriter writer, CustomType value)
    {
        //Write here
    }

    public override CustomType Read(BufferedReader reader)
    {
        return new CustomType();
    }
}
```

### Fixed

- Fixed proxy generator not handling refs correctly

## [1.3.0] - 2021-12-30

### Changed

* Updated TCPClient's constructors
* Improve disposing on both the client and host

### Fixed

* Fix issue with disposing on TCPClient when the client has not connected before
* Don't repeatedly call listener.Stop() when the connection cap has been reached in TCPHost

## [1.2.2] - 2021-12-28

### Added

* Added Length property to buffers

### Changed

* Made position in buffers public
* Made Flush and Reset in buffered writer public

## [1.2.1] - 2021-12-27

### Changed

* Expose buffered reader & writer's constructor

## [1.2.0] - 2021-11-13

### Changed

* Drop support for .NET 5, add .NET 6
* Update trimming support
* Improve TCP's client connect
* Handle isConnectedInternal completly internally
* Use Scriban for code generation
* Check if a member is a method
* Target Microsoft.CodeAnalysis.CSharp version 3.9.0 for projects that use an older compiler (e.g Unity)

## [1.1.1] - 2021-08-07

### Fixed

* Fixed TCPClient not setting IsConnected

## [1.1.0] - 2021-08-07

### Added

* Added Host.ConnectionCount
* Added Host.MaxConnectionsCount
* Added Host.IsRunning

### Changed

* BREAKING: Renamed Host.ReaderWriterManager to TypeReaderWriterManager to match the client's name

### Fixed

* Client.InvokeMethod will check if the client is connected

## [1.0.0] - 2021-08-01

* Initial Release