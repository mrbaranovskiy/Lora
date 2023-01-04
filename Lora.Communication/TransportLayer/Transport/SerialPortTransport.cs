using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace MCP.Communication.TransportLayer.Transport;

public sealed class SerialPortTransport : ITransport<byte>
{
    private IObserver<byte[]>? _observer;
    private readonly SerialPort _serialPort;
    private CancellationTokenSource _ct = new();
    private bool disposedValue;

    public SerialPortTransport(string portName, int baud,
        Parity parity = Parity.None, int databits = 8)
    {
        _serialPort = new SerialPort(portName, baud, parity, databits);
        _serialPort.RtsEnable = true;
    }

    public Task WriteAsync(byte[] data)
    {
        if (disposedValue)
        {
            OnCompleted();
            return Task.CompletedTask;
        } 

        if (_serialPort.IsOpen)
        {
            try
            {
                _serialPort.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                //LogExt.Error("Transport closed" + ex.Message);
            }
        }
        else
        {
            OnError(new Exception("Transport is closed"));
        }

        return Task.CompletedTask;
    }

    public void Start()
    {
        if (!_serialPort.IsOpen)
        {
            try
            {
                _serialPort.Open();
            }
            catch (Exception ex)
            {
                //LogExt.Debug("Unable to start serial transport");
                OnError(ex);
                return;
            }
        }

        GetReadingTask();
    }

    private void GetReadingTask()
    {
        _ct = new CancellationTokenSource();
        Task.Factory.StartNew(() =>
        {
            while (!_ct.IsCancellationRequested)
            {
                try
                {
                    if (!_serialPort.IsOpen) 
                        return;
                

                    if (_serialPort.BytesToRead > 0)
                    {
                        SerialPortOnDataReceived();
                    }
                }
                catch(Exception ex)
                {
                    //LogExt.Fatal("Driver down " + ex);
                   continue;
                }
            }
        }, _ct.Token);
    }

    private void SerialPortOnDataReceived()
    {
        var buffer = ReadFromSerial();
        if(buffer.Length != 0)
            OnNext(buffer);
    }


    private byte[] ReadFromSerial()
    {
        try
        {
            var count = _serialPort.BytesToRead;
            var buffer = new byte[count];
            var read = _serialPort.Read(buffer, 0, count);

            //LogExt.Verbose($"{nameof(ReadFromSerial)}: Read {read} form {_serialPort.BytesToRead}");

            return buffer;
        }
        catch (Exception ex)
        {
            //LogExt.Fatal("Cannot read from device " + ex);
            OnError(ex);
            return Array.Empty<byte>();
        }
      
    }

    public void OnCompleted() => _observer?.OnCompleted();
    public void OnError(Exception error) => _observer?.OnError(error);
    public void OnNext(byte[] value) => _observer?.OnNext(value);

    public IDisposable Subscribe(IObserver<byte[]> observer)
    {
        _observer = observer;
        Start();
        return this;
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (_serialPort.IsOpen)
                {
                    try
                    {
                        _serialPort.DiscardInBuffer();
                        _serialPort.DiscardOutBuffer();
                        _serialPort.Close();
                        _serialPort.Dispose();

                        _ct.Cancel();
                    }
                    catch (IOException e)
                    {
                        //LogExt.Error("Error when disposing the serial device" + e);
                    }
                    finally
                    {
                        OnCompleted();
                    }
                }
            }

            disposedValue = true;
        }
    }

    ~SerialPortTransport()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}