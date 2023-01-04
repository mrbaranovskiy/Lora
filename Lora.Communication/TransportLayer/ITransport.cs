using System;
using System.Reactive.Subjects;

namespace MCP.Communication.TransportLayer;

public interface ITransport<T> : ISubject<T[]>, IWriteTransport<T>, IDisposable where T : struct { }
