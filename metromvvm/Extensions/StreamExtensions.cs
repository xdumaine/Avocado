// See: http://social.msdn.microsoft.com/Forums/en-IE/winappswithcsharp/thread/ac415510-d650-4c70-b4d4-dc109315e699
namespace MetroMVVM.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Storage.Streams;

    public static class StreamExtensions
    {
        public static IRandomAccessStream AsRandomAccessStream(this System.IO.Stream stream)
        {
            return new CustomRandomAccessStream(stream);
        }
    }
}
