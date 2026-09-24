/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging.Objects.Documents
 * FILE:        RasterCodecs.cs
 * PURPOSE:     How the pixels of one raster layer are stored inside a document file.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.Drawing;
using System.Drawing.Imaging;

namespace Imaging.Objects.Documents
{
    /// <summary>
    ///     Reads and writes the pixels of a single layer.
    /// </summary>
    public interface IRasterCodec
    {
        /// <summary>Gets the file extension including the dot, for example ".png". Used to pick the codec when loading.</summary>
        string Extension { get; }

        /// <summary>Gets a value indicating whether the output is already compressed (the zip container then stores it as is).</summary>
        bool IsCompressed { get; }

        /// <summary>Writes the buffer.</summary>
        void Write(UnmanagedImageBuffer buffer, Stream output);

        /// <summary>Reads a buffer of exactly the expected size, or throws <see cref="DocumentFormatException" />.</summary>
        UnmanagedImageBuffer Read(Stream input, int expectedWidth, int expectedHeight);
    }

    /// <summary>
    ///     Raw straight-alpha BGRA with a tiny header; the zip container deflates it. No GDI+ needed, so it works
    ///     everywhere and is what the unit tests use.
    /// </summary>
    public sealed class RawBgraCodec : IRasterCodec
    {
        private static readonly byte[] Magic = { (byte)'S', (byte)'B', (byte)'G', (byte)'R' };

        /// <inheritdoc />
        public string Extension => ".bgra";

        /// <inheritdoc />
        public bool IsCompressed => false;

        /// <inheritdoc />
        public void Write(UnmanagedImageBuffer buffer, Stream output)
        {
            ArgumentNullException.ThrowIfNull(buffer);
            ArgumentNullException.ThrowIfNull(output);

            using var writer = new BinaryWriter(output, System.Text.Encoding.UTF8, leaveOpen: true);
            writer.Write(Magic);
            writer.Write(buffer.Width);
            writer.Write(buffer.Height);
            writer.Flush();

            output.Write(buffer.BufferSpan);
        }

        /// <inheritdoc />
        public UnmanagedImageBuffer Read(Stream input, int expectedWidth, int expectedHeight)
        {
            ArgumentNullException.ThrowIfNull(input);

            var header = new byte[12];
            try
            {
                input.ReadExactly(header);
            }
            catch (EndOfStreamException ex)
            {
                throw new DocumentFormatException("Raster layer data is truncated.", ex);
            }

            if (!header.AsSpan(0, 4).SequenceEqual(Magic))
            {
                throw new DocumentFormatException("Raster layer data has a wrong header.");
            }

            var width = BitConverter.ToInt32(header, 4);
            var height = BitConverter.ToInt32(header, 8);

            if (width != expectedWidth || height != expectedHeight)
            {
                throw new DocumentFormatException(
                    $"Raster layer is {width}x{height} but the document is {expectedWidth}x{expectedHeight}.");
            }

            var buffer = new UnmanagedImageBuffer(width, height);
            try
            {
                input.ReadExactly(buffer.BufferSpan);
                return buffer;
            }
            catch (EndOfStreamException ex)
            {
                buffer.Dispose();
                throw new DocumentFormatException("Raster layer data is truncated.", ex);
            }
            catch
            {
                buffer.Dispose();
                throw;
            }
        }
    }

    /// <summary>
    ///     PNG through GDI+. Layers can be opened in any image tool, alpha is preserved.
    /// </summary>
    public sealed class PngRasterCodec : IRasterCodec
    {
        /// <inheritdoc />
        public string Extension => ".png";

        /// <inheritdoc />
        public bool IsCompressed => true;

        /// <inheritdoc />
        public void Write(UnmanagedImageBuffer buffer, Stream output)
        {
            ArgumentNullException.ThrowIfNull(buffer);
            ArgumentNullException.ThrowIfNull(output);

            // GDI+ needs a seekable stream to encode PNG, a zip entry stream is not.
            using var bitmap = buffer.ToBitmap();
            using var memory = new MemoryStream();
            bitmap.Save(memory, ImageFormat.Png);

            memory.Position = 0;
            memory.CopyTo(output);
        }

        /// <inheritdoc />
        public UnmanagedImageBuffer Read(Stream input, int expectedWidth, int expectedHeight)
        {
            ArgumentNullException.ThrowIfNull(input);

            using var memory = new MemoryStream();
            input.CopyTo(memory);
            memory.Position = 0;

            try
            {
                using var bitmap = new Bitmap(memory);

                if (bitmap.Width != expectedWidth || bitmap.Height != expectedHeight)
                {
                    throw new DocumentFormatException(
                        $"Raster layer is {bitmap.Width}x{bitmap.Height} but the document is {expectedWidth}x{expectedHeight}.");
                }

                return UnmanagedImageBuffer.FromBitmap(bitmap);
            }
            catch (ArgumentException ex)
            {
                // GDI+ reports undecodable data as ArgumentException.
                throw new DocumentFormatException("Raster layer is not a valid image.", ex);
            }
        }
    }
}
