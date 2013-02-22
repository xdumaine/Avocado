namespace MetroMVVM.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.Storage.Streams;

    public class CustomRandomAccessStream : IRandomAccessStream
    {
        Stream _stream;

        public CustomRandomAccessStream(Stream stream)
        {
            _stream = stream;
        }

        public IInputStream GetInputStreamAt(ulong pos)
        {
            _stream.Position = (long)pos;
            return _stream.AsInputStream();
        }

        public IOutputStream GetOutputStreamAt(ulong pos)
        {
            _stream.Position = (long)pos;
            return _stream.AsOutputStream();
        }

        public void Seek(ulong position)
        {
            // TO DO
        }

        public IRandomAccessStream CloneStream()
        {
            return _stream.AsRandomAccessStream();
        }


        public IAsyncOperationWithProgress<IBuffer, UInt32> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
        {
            return null;
        }

        public IAsyncOperationWithProgress<UInt32, UInt32> WriteAsync(IBuffer buffer)
        {
            return null;
        }

        public IAsyncOperation<bool> FlushAsync()
        {
            return null;
        }

        public void Dispose()
        {
        }

        public ulong Size
        {
            get { return (ulong)_stream.Length; }
            set { _stream.SetLength((long)value); }
        }

        public ulong Position
        {
            get { return 0; }
        }

        public bool CanRead
        {
            get
            {
                return true;
            }
        }

        public bool CanWrite { get { return true; } }
    }
}
