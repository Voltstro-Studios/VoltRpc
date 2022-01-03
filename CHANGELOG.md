# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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