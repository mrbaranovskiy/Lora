using System;
using System.Linq;
using System.Reactive.Linq;

namespace MCP.Communication.DataStructures;

public static class ObservableExt
{
    public static IObservable<TSource> CatchDefault<TSource>(this IObservable<TSource> source, TSource item) 
        => source.Catch<TSource, Exception>(_ => Observable.Return(item));

    /// <summary>
    /// Selects single value by the <see cref="selector"/> from the Buffer which works in Buffer(bufferLen, bufferLen - 1) mode.
    /// Initially the sequence is auto-prepended by the <see cref="@default"/> values.
    /// </summary>
    /// <param name="source">Input observable</param>
    /// <param name="selector">Select the value which is valid for sequence</param>
    /// <param name="default">default value if non of the buffered values are ok</param>
    /// <param name="bufferLen">Length of the buffer</param>
    /// <typeparam name="TSource">Type of the input observables</typeparam>
    /// <returns></returns>
    public static IObservable<TSource> BufferedSelect<TSource>(this IObservable<TSource> source, Func<TSource, bool> selector, TSource @default, int bufferLen = 3)
    {
       return source
           .AutoPrepend(@default, bufferLen)
           .Buffer(bufferLen, bufferLen - 1).Select(s =>
                {
                    var item = s.Where(selector).ToArray();
                    if (item.Any())
                        return Observable.Return(item.First());
                    return Observable.Return(@default);
                }).SelectMany(s=>s);
    }

    private static IObservable<TSource> AutoPrepend<TSource>(this IObservable<TSource> source, TSource @default, int count)
    {
        for (int i = 0; i < count; i++)
        {
            source.Prepend(@default);
        }
        return source;
    }
}
