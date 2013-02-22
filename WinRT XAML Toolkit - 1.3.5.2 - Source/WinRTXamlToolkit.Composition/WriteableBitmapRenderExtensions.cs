﻿using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace WinRTXamlToolkit.Composition
{
    // Thanks to Christoph Wille and his WinRT-snippets
    // https://github.com/christophwille/winrt-snippets/tree/master/RenderTextToBitmap
    // http://stackoverflow.com/questions/9151615/how-does-one-use-a-memory-stream-instead-of-files-when-rendering-direct2d-images

    public static class WriteableBitmapRenderExtensions
    {
        public static async Task Render(this WriteableBitmap wb, FrameworkElement fe)
        {
            var ms = RenderToStream(fe);
            var msrandom = new MemoryRandomAccessStream(ms);
            await wb.SetSourceAsync(msrandom);
        }

        public static MemoryStream RenderToStream(FrameworkElement fe)
        {
            return new CompositionEngine().RenderToPngStream(fe);
        }
    }
}
