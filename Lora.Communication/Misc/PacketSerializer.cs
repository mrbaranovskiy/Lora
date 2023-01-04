using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using MCP.DeviceDiscovery.Contracts;

namespace MCP.Communication.Misc;

public static class PacketSerializer
{
    public static object Deserialize(Type type, ReadOnlySpan<byte> span, out int sz)
    {
        if (type.IsValueType)
            throw new NotSupportedException("Struct cannot be serialized");

        //todo: implement a lot of checks.
        var instance = Activator.CreateInstance(type);

        if (instance is null)
            throw new InvalidOperationException($"Instance {type.Name} cannot be activated");
        
        var shift = 0;

        var props = GetProperties(instance);

        foreach (var propInfo in props)
        {
            if (propInfo.PropertyType.IsValueType)
            {
                var val = propInfo.GetValue(instance);

                //if (val is not null)
                {
                    var size = Marshal.SizeOf(val!);

                    if (shift + size > span.Length)
                    {
                        var defaultVal = Activator.CreateInstance(propInfo.PropertyType);
                        propInfo.SetValue(instance, defaultVal);
                    }
                    else
                    {
                        var fieldVal = DeserializeType(span.Slice(shift, size), propInfo.PropertyType);
                        propInfo.SetValue(instance, fieldVal);
                    }

                    shift += size;

                    continue;
                }
            }

            if (propInfo.PropertyType.IsArray)
            {
                var posAttr = propInfo.GetCustomAttributes(typeof(PositionAttribute)).FirstOrDefault();
                if (posAttr is null)
                    throw new InvalidOperationException("Attibute is not set");

                if (posAttr is PositionAttribute attr)
                {
                    var attrArrayElemCount = attr.ArrayElemCount;
                    var elemType = propInfo.PropertyType.GetElementType()!;

                    if (!elemType.IsValueType) 
                        throw new NotSupportedException("Arrays can only contains value types.");

                    if (attr.ArrayElemCount <= 0)
                    {
                        var len = FindLen(instance, props, propInfo.Name);
                        if (len == -1)
                            throw new InvalidOperationException($"{nameof(ArrayLengthAttribute)} is not specified or has invalid data");

                        attrArrayElemCount = len;
                    }

                    //todo: speed-up deserialization with explicit data len specifying
                    var elem = FormatterServices.GetUninitializedObject(elemType);

                    var elemSize = Marshal.SizeOf(elem);
                    var newArr = Array.CreateInstance(elemType, attrArrayElemCount);

                    for (int i = 0; i < attrArrayElemCount; i++)
                    {
                        var desElem = DeserializeType(span.Slice(shift, elemSize), elemType);
                        newArr.SetValue(desElem, i);
                        shift += elemSize;
                    }

                    propInfo.SetValue(instance, newArr);

                    continue;
                }
            }

            if (propInfo.PropertyType.IsAssignableFrom(typeof(string)))
            {
                var strSlice = span.Slice(shift);
                var indexOf = strSlice.IndexOf((byte) '\0');

                if (indexOf == -1)
                    throw new InvalidOperationException("Smth wrong with structure");

                var str = Encoding.ASCII.GetString(strSlice[0..indexOf]);
                propInfo.SetValue(instance, str);

                shift += str.Length;

                continue;
            }

            if (propInfo.PropertyType.IsClass)
            {
                var val = Deserialize(propInfo.PropertyType, span.Slice(shift), out var outSize);
                propInfo.SetValue(instance, val);
                shift += outSize;

                continue;
            }
        }

        sz = shift;
        return instance;
    }

    private static int FindLen(object o, PropertyInfo[] props, string propName)
    {
        foreach (var propertyInfo in props)
        {
            if (propertyInfo.GetCustomAttributes(typeof(ArrayLengthAttribute))
                    .FirstOrDefault() is ArrayLengthAttribute attr)
            {
                if (attr.RelatedTo(propName))
                {
                    var value = propertyInfo.GetValue(o);
                    if (value is not null)
                    {
                        var val = Convert.ToInt32(value);
                        if (val > 0) return val;
                    }
                }
            }
        }

        return -1;
    }

    private static PropertyInfo[] GetProperties(object o)
    {
        var fields = o.GetType().GetProperties();
        var cnt = 0;
        var list = new List<(int, PropertyInfo)>();
        foreach (var fieldInfo in fields)
        {
            var attrs = fieldInfo.GetCustomAttributes(typeof(PositionAttribute));
            var attributes = attrs as Attribute[] ?? attrs.ToArray();
            cnt += attributes.Count();

            if (attributes.Count() > 0)
            {
                var att = (PositionAttribute) attributes[0];
                list.Add((att.Position, fieldInfo));
            }
        }

        if (fields.Count() == cnt)
            return list.OrderBy(s => s.Item1).Select(s => s.Item2).ToArray();

        return o.GetType().GetProperties();
    }

    /// <summary> Serializes the packet and add the len and crc. </summary>
    public static byte[] SerializePacket<T>(T obj)
    {
        var buffer = Serialize(obj!);
        if (buffer.Length < 6)
            throw new ArgumentException("Packet is to short");

        if (buffer[0] != 0xAA)
            throw new InvalidDataException("Packet has no start");

        if (buffer.Length > ushort.MaxValue)
            throw new InvalidDataException("Packet is to long");

        var len = MemExt.ToBytes((ushort) (buffer.Length));

        buffer[1] = len.l;
        buffer[2] = len.h;

        var crc = Crc.Crc16(buffer[..^2]);
        var crc16 = MemExt.ToBytes(crc);

        buffer[^1] = crc16.h;
        buffer[^2] = crc16.l;

        return buffer;
    }

    public static byte[] Serialize<T>([DisallowNull] T obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        if (obj.GetType().GetConstructor(Type.EmptyTypes) is null)
            throw new NotSupportedException($"{nameof(obj)} has constructors");

        var buffer = new List<byte>(16);

        foreach (var propInfo in GetProperties(obj))
        {
            var value = propInfo.GetValue(obj);
            var buf = SerializeType(value!, propInfo.PropertyType);

            foreach (var b in buf)
            {
                buffer.Add(b);
            }
        }

        return buffer.ToArray();
    }

    private static object DeserializeType(ReadOnlySpan<byte> buff, Type type)
    {
        var ptr = Marshal.AllocHGlobal(buff.Length);
        Marshal.Copy(buff.ToArray(), 0, ptr, buff.Length);
        var structure = Marshal.PtrToStructure(ptr, type);
        Marshal.FreeHGlobal(ptr);

        if (structure is null)
            throw new InvalidCastException("Structure cannot be converted.");

        return structure;
    }

    private static byte[] SerializeType(object o, Type type)
    {
        if (type.IsArray)
        {
            if (type.GetArrayRank() > 1)
                throw new NotSupportedException("Multidimensional arrays are not supported");
            Array arr = (Array) o;

            var result = new List<byte>(24);
            var arrayType = type.GetElementType();

            foreach (var item in arr)
            {
                var elemBuffer = SerializeType(item, arrayType);
                result.AddRange(elemBuffer);
            }

            return result.ToArray();
        }

        if (type.IsValueType)
        {
            unsafe
            {
                var size = Marshal.SizeOf(o);
                var ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(o, ptr, false);
                var buf = new byte[size];
                Span<byte> bufSpan = new Span<byte>(buf);
                Span<byte> span = new Span<byte>((void*) ptr, buf.Length);
                span.CopyTo(bufSpan);
                Marshal.FreeHGlobal(ptr);

                return buf;
            }
        }
        else if (type.IsAssignableFrom(typeof(string)))
        {
            var str = Encoding.ASCII.GetBytes((string) o);
            var buff = new byte[str.Length + 1];
            str.CopyTo(buff, 0);
            buff[^1] = (byte) '\0';

            return buff;
        }
        else if (type.IsClass)
        {
            return Serialize(o);
        }

        return null;
    }
}