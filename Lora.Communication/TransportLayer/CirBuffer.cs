using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace MCP.Communication.TransportLayer;

internal class CirBuffer<T> : IEnumerable<T>, IDisposable
{
    private T[] _internalArray;
    private int _head;
    private int _tail;

    private readonly ReaderWriterLockSlim _rw = new(LockRecursionPolicy.SupportsRecursion);

    public CirBuffer(int capacity = 2)
    {
        if (capacity <= 1)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        _internalArray = new T[capacity];
    }

    public int BufferLength => _internalArray.Length;

    public void Push(T item)
    {
        var next = GetNextIndex(_head);
        if (next == _tail) 
            RebuildInternalArray();

        var nextIdx = GetNextIndex();
        _internalArray[_head] = item;
        _head = nextIdx;
    }


    public void Push(ReadOnlyMemory<T> memory)
    {
        _rw.EnterWriteLock();
        try
        {
            for (int i = 0; i < memory.Length; i++)
                Push(memory.Span[i]);
        }
        finally
        {
            _rw.ExitWriteLock();
        }
    }

    public void Push(ReadOnlySpan<T> span)
    {
        _rw.EnterWriteLock();
        try
        {
            for (int i = 0; i < span.Length; i++)
                Push(span[i]);
        }
        finally
        {
            _rw.ExitWriteLock();
        }
    }

    public int DataLen
    {
        get
        {
            if (_head > _tail) return _head - _tail;
            else if (_head == _tail)
                return 0;

            return (_internalArray.Length - _tail) + _head;
        }
    }

    public T PopItem()
    {
        if (DataLen == 0) throw new InvalidOperationException("No data in collection");
        var item = _internalArray[_tail];
        _tail = GetNextIndex(_tail);
    
        return item;
    }

    public void RemoveAll() => Remove(DataLen);

    public void Remove(int len)
    {
        _rw.EnterWriteLock();
      
        try
        {
            if(len <= 0) return;

            if(DataLen < len)
                throw new IndexOutOfRangeException("Out of data range");
            _tail = GetNextIndex(_tail + len - 1);
        }
        finally
        {
            _rw.ExitWriteLock();
        }
    }

    public T[] Pop(int len)
    {
        _rw.EnterWriteLock();
        try
        {
            if (len <= 0) return Array.Empty<T>();
            var arr = new T[len];

            for (int i = 0; i < len; i++)
                arr[i] = PopItem();

            return arr;
        }
        finally
        {
            _rw.ExitWriteLock();
        }
    }
   
    /// <summary> Gets item without removing. </summary>
    public T PeekItem(int indexFromTail)
    {
        _rw.EnterReadLock();
        try
        {
            var itemIdx = GetNextIndex(_tail + indexFromTail - 1);

            if (indexFromTail > DataLen - 1)
                throw new ArgumentOutOfRangeException("Out of data range");
            return _internalArray[itemIdx];


        }
        finally
        {
            _rw.ExitReadLock();
        }
    }

    private void RebuildInternalArray()
    {
        var newArr = new T[_internalArray.Length * 2];

        int idx = 0;

        foreach (var item in this)
        {
            newArr[idx] = item;
            idx++;
        }

        Array.Clear(_internalArray, 0, _internalArray.Length);
        //_internalArray = null;
        _internalArray = newArr;
        _tail = 0;
        _head = idx;
    }

    public IEnumerator<T> GetEnumerator()
    {
        var current = _tail;

        while (current != _head)
        {
            yield return _internalArray[current];
            current = GetNextIndex(current);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Dispose()
    {
        if(_internalArray is not null)
             Array.Clear(_internalArray, 0, _internalArray.Length);
        
        _internalArray = null!;
    }

    private int GetNextIndex(int idx)
    {
        return (idx + 1) % _internalArray.Length;
    }

    private int GetNextIndex()
    {
        return (_head + 1) % _internalArray.Length;
    }
}