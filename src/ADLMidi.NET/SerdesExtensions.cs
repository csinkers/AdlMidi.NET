using System;
using System.Text;
using SerdesNet;

namespace ADLMidi.NET;

internal static class SerdesExtensions
{
    public static string FixedLengthString(this ISerdes serdes, SerdesName name, string value, int length)
    {
        byte[] bytes = new byte[length];
        if (serdes.IsWriting() && value != null)
            Encoding.ASCII.GetBytes(value).CopyTo(bytes, 0);

        if (bytes?.Length > 0)
        {
            var temp = serdes.Bytes(name, bytes, length);

            Array.Clear(bytes, 0, length);
            temp.CopyTo(bytes, 0);

            if (serdes.IsReading())
                value = Encoding.ASCII.GetString(bytes);
        }

        return value;
    }
}