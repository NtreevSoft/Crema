//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;
using Ntreev.Crema.Code.Reader.IO;

namespace Ntreev.Crema.Code.Reader.Internal
{
    class RemoteStream : MemoryStream
    {
        private const int bufferLength = 1024 * 5000;
        //private readonly TcpClient client;
        //private long position;
        //private long length;
        //private byte[] buffer;
        //private long bufferpos;

        public RemoteStream(string ipAddress, int port, string name)
        {
            var client = new TcpClient(ipAddress, port);

            var stream = client.GetStream();

            var writer = new BinaryWriter(stream);
            var reader = new BinaryReader(stream);

            writer.Write((int)HeaderType.Size);
            writer.Write(name);

            var length = (int)reader.ReadInt64();
            this.Capacity = length;
            var buffer = new byte[bufferLength];

            var len = 0;
            while (len < length)
            {
                var bufferInfo = new BufferInfo(len, bufferLength);
                //bufferInfo.pos = pos;
                //bufferInfo.size = SocketStream.bufferLength;
                //bufferInfo.dummy = 0;

                writer.Write((int)HeaderType.Buffer);
                writer.WriteValue(bufferInfo);

                var readBytes = 0;
                var read = 0;
                while (readBytes < bufferLength)
                {
                    read = reader.Read(buffer, 0, bufferLength);
                    this.Write(buffer, 0, read);
                    readBytes += read;
                    len += read;
                }
            }
            client.Close();
            this.Position = 0;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        //public override long Length
        //{
        //    get { return this.length; }
        //}

        //public override long Position
        //{
        //    get
        //    {
        //        return this.position;
        //    }
        //    set
        //    {
        //        this.position = value;
        //    }
        //}

        //public override int Read(byte[] buffer, int offset, int count)
        //{
        //    int readCount = 0;
        //    for (int i = 0; i < count; i++)
        //    {
        //        if (this.buffer == null || this.position - this.bufferpos + i >= this.buffer.Length)
        //        {
        //            this.SocketRead(this.position);
        //        }
        //        byte b = this.buffer[this.position - this.bufferpos + i];
        //        buffer[offset + i] = b;
        //        readCount++;
        //    }

        //    this.position += readCount;

        //    return readCount;
        //}

        //public override long Seek(long offset, SeekOrigin origin)
        //{
        //    long p = this.position;
        //    switch (origin)
        //    {
        //        case SeekOrigin.Begin:
        //            p = offset;
        //            break;
        //        case SeekOrigin.Current:
        //            p += offset;
        //            break;
        //        case SeekOrigin.End:
        //            p = this.length - offset;
        //            break;
        //    }
        //    this.position = p;

        //    if ((this.buffer != null && this.position - this.bufferpos >= this.buffer.Length) || this.position < this.bufferpos)
        //    {
        //        this.buffer = null;
        //    }
        //    return p;
        //}

        //public override void SetLength(long value)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Write(byte[] buffer, int offset, int count)
        //{
        //    throw new NotImplementedException();
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    base.Dispose(disposing);

        //    if (this.stream != null)
        //    {
        //        this.stream.Close();
        //        this.client.Close();
        //        this.stream = null;
        //    }
        //}

        //private int SocketRead(long pos)
        //{
        //    BufferInfo bufferInfo = new BufferInfo(pos, SocketStream.bufferLength);
        //    //bufferInfo.pos = pos;
        //    //bufferInfo.size = SocketStream.bufferLength;
        //    //bufferInfo.dummy = 0;

        //    this.bw.Write((int)HeaderType.Buffer);
        //    this.bw.WriteValue(bufferInfo);

        //    this.buffer = this.br.ReadBytes(SocketStream.bufferLength);
        //    this.bufferpos = pos;
        //    return this.buffer.Length;
        //}

        #region classes

        enum HeaderType
        {
            Identify,
            Size,
            Buffer,
            Compare,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct BufferInfo
        {
            public BufferInfo(long pos, int size)
                : this()
            {
                this.pos = pos;
                this.size = size;
                //this.dummy = 0;
            }
            public long pos;
            public int size;
            public int dummy;
        }

        #endregion
    }
}
