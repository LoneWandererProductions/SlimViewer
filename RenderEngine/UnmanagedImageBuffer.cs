﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        UnmanagedImageBuffer.cs
 * PURPOSE:     A way to store images in a fast way.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace RenderEngine
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents an unmanaged memory buffer for storing image pixel data with direct memory access,
    ///     optimized for fast pixel manipulation and bulk operations using SIMD acceleration where available.
    /// </summary>
    /// <remarks>
    ///     This class allocates unmanaged memory of size Width * Height * BytesPerPixel to store image data in BGRA format by
    ///     default.
    ///     It supports setting individual pixels, clearing the buffer to a uniform color,
    ///     applying multiple pixel changes at once, and replacing the entire buffer efficiently.
    /// </remarks>
    public sealed unsafe class UnmanagedImageBuffer : IDisposable
    {
        /// <summary>
        ///     The buffer PTR
        /// </summary>
        private readonly IntPtr _bufferPtr;

        /// <summary>
        ///     The buffer size
        /// </summary>
        private readonly int _bufferSize;

        /// <summary>
        ///     The bytes per pixel
        /// </summary>
        private readonly int _bytesPerPixel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UnmanagedImageBuffer" /> class with specified dimensions and bytes per
        ///     pixel.
        ///     The buffer is allocated in unmanaged memory and initially cleared to transparent black.
        /// </summary>
        /// <param name="width">The width of the image in pixels. Must be positive.</param>
        /// <param name="height">The height of the image in pixels. Must be positive.</param>
        /// <param name="bytesPerPixel">The number of bytes per pixel (default is 4 for BGRA).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if width or height is less than or equal to zero.</exception>
        public UnmanagedImageBuffer(int width, int height, int bytesPerPixel = 4)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            if (bytesPerPixel <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bytesPerPixel));
            }

            Width = width;
            Height = height;
            _bytesPerPixel = bytesPerPixel;
            _bufferSize = width * height * bytesPerPixel;

            _bufferPtr = Marshal.AllocHGlobal(_bufferSize);
            Clear(0, 0, 0, 0);
        }

        /// <summary>
        ///     Gets a span representing the entire unmanaged buffer memory as a byte sequence.
        ///     Modifications to this span directly update the unmanaged image data.
        /// </summary>
        public Span<byte> BufferSpan => new(_bufferPtr.ToPointer(), _bufferSize);

        /// <summary>
        ///     Gets the width of the image in pixels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        ///     Gets the height of the image in pixels.
        /// </summary>
        public int Height { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Frees the unmanaged buffer memory.
        /// </summary>
        public void Dispose()
        {
            if (_bufferPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_bufferPtr);
            }
        }

        /// <summary>
        ///     Gets the color of the pixel at the specified coordinates.
        /// </summary>
        /// <param name="x">X coordinate (0-based)</param>
        /// <param name="y">Y coordinate (0-based)</param>
        /// <returns>The pixel color as a System.Drawing.Color</returns>
        public Color GetPixel(int x, int y)
        {
            var offset = GetPixelOffset(x, y);
            var buffer = BufferSpan;
            var b = buffer[offset];
            var g = buffer[offset + 1];
            var r = buffer[offset + 2];
            var a = buffer[offset + 3];
            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        ///     Sets the pixel color at the specified coordinates using a System.Drawing.Color.
        /// </summary>
        /// <param name="x">X coordinate (0-based)</param>
        /// <param name="y">Y coordinate (0-based)</param>
        /// <param name="color">The color to set</param>
        public void SetPixel(int x, int y, Color color)
        {
            SetPixel(x, y, color.A, color.R, color.G, color.B);
        }

        /// <summary>
        ///     Sets the pixel with alpha blend.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="a">a.</param>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        public void SetPixelAlphaBlend(int x, int y, byte a, byte r, byte g, byte b)
        {
            var offset = GetPixelOffset(x, y);
            var buffer = BufferSpan;

            // Old pixel BGRA
            var oldB = buffer[offset];
            var oldG = buffer[offset + 1];
            var oldR = buffer[offset + 2];
            var oldA = buffer[offset + 3];

            var alpha = a / 255f;

            var newR = (byte)((r * alpha) + (oldR * (1 - alpha)));
            var newG = (byte)((g * alpha) + (oldG * (1 - alpha)));
            var newB = (byte)((b * alpha) + (oldB * (1 - alpha)));
            var newA = (byte)(a + (oldA * (1 - alpha))); // Approximate new alpha

            buffer[offset] = newB;
            buffer[offset + 1] = newG;
            buffer[offset + 2] = newR;
            buffer[offset + 3] = newA;
        }


        /// <summary>
        ///     Calculates the byte offset in the buffer for the pixel at coordinates (x, y).
        /// </summary>
        /// <param name="x">The horizontal pixel coordinate (0-based).</param>
        /// <param name="y">The vertical pixel coordinate (0-based).</param>
        /// <returns>The byte offset of the pixel in the buffer.</returns>
        public int GetPixelOffset(int x, int y)
        {
            return ((y * Width) + x) * _bytesPerPixel;
        }

        /// <summary>
        ///     Sets the color of a single pixel at coordinates (x, y) in BGRA order.
        /// </summary>
        /// <param name="x">The horizontal pixel coordinate (0-based).</param>
        /// <param name="y">The vertical pixel coordinate (0-based).</param>
        /// <param name="a">Alpha channel byte value.</param>
        /// <param name="r">Red channel byte value.</param>
        /// <param name="g">Green channel byte value.</param>
        /// <param name="b">Blue channel byte value.</param>
        public void SetPixel(int x, int y, byte a, byte r, byte g, byte b)
        {
            var offset = GetPixelOffset(x, y);
            var buffer = BufferSpan;
            buffer[offset] = b;
            buffer[offset + 1] = g;
            buffer[offset + 2] = r;
            buffer[offset + 3] = a;
        }

        /// <summary>
        ///     Sets the pixels simd.
        /// </summary>
        /// <param name="pixels">The pixels.</param>
        public void SetPixelsSimd(List<(int x, int y, Color color)> pixels)
        {
            var changes = new (int x, int y, uint bgra)[pixels.Count];

            for (var i = 0; i < pixels.Count; i++)
            {
                var (x, y, color) = pixels[i];
                // Pack color into BGRA uint
                var packed = PackBgra(color.A, color.R, color.G, color.B);
                changes[i] = (x, y, packed);
            }

            ApplyChanges(changes);
        }

        /// <summary>
        ///     Clears the entire buffer by setting every pixel to the specified color in BGRA order.
        ///     Uses SIMD vectorized operations for performance when available.
        /// </summary>
        /// <param name="a">Alpha channel byte value.</param>
        /// <param name="r">Red channel byte value.</param>
        /// <param name="g">Green channel byte value.</param>
        /// <param name="b">Blue channel byte value.</param>
        public void Clear(byte a, byte r, byte g, byte b)
        {
            var buffer = BufferSpan;

            var pixelVector = CreatePixelVector(a, r, g, b);

            var vectorSize = Vector<byte>.Count;
            var i = 0;

            for (; i <= buffer.Length - vectorSize; i += vectorSize)
            {
                pixelVector.CopyTo(buffer.Slice(i, vectorSize));
            }

            // Fill any remaining bytes one pixel at a time
            for (; i < buffer.Length; i += 4)
            {
                buffer[i] = b;
                buffer[i + 1] = g;
                buffer[i + 2] = r;
                buffer[i + 3] = a;
            }
        }

        /// <summary>
        ///     Creates a SIMD vector filled with the specified BGRA pixel color repeated to fill the vector.
        /// </summary>
        /// <param name="a">Alpha channel byte value.</param>
        /// <param name="r">Red channel byte value.</param>
        /// <param name="g">Green channel byte value.</param>
        /// <param name="b">Blue channel byte value.</param>
        /// <returns>A <see cref="Vector{Byte}" /> filled with the repeated pixel color pattern.</returns>
        private static Vector<byte> CreatePixelVector(byte a, byte r, byte g, byte b)
        {
            var pixelBytes = new byte[Vector<byte>.Count];
            for (var i = 0; i < pixelBytes.Length; i += 4)
            {
                pixelBytes[i] = b;
                pixelBytes[i + 1] = g;
                pixelBytes[i + 2] = r;
                pixelBytes[i + 3] = a;
            }

            return new Vector<byte>(pixelBytes);
        }

        /// <summary>
        ///     Applies multiple pixel changes to the buffer in-place, given a span of coordinate-color tuples.
        ///     Each tuple contains the x and y pixel coordinates and a packed 32-bit BGRA color.
        ///     Pixels outside the valid image bounds are ignored.
        /// </summary>
        /// <param name="changes">A read-only span of pixel changes, each specified as (x, y, BGRA color).</param>
        public void ApplyChanges(ReadOnlySpan<(int x, int y, uint bgra)> changes)
        {
            var buffer = BufferSpan;

            foreach (var (x, y, bgra) in changes)
            {
                if ((uint)x >= (uint)Width || (uint)y >= (uint)Height)
                {
                    continue;
                }

                var offset = GetPixelOffset(x, y);

                // Decompose packed uint BGRA color into bytes:
                buffer[offset] = (byte)(bgra & 0xFF); // Blue
                buffer[offset + 1] = (byte)((bgra >> 8) & 0xFF); // Green
                buffer[offset + 2] = (byte)((bgra >> 16) & 0xFF); // Red
                buffer[offset + 3] = (byte)((bgra >> 24) & 0xFF); // Alpha
            }
        }

        /// <summary>
        ///     Replaces the entire unmanaged buffer with a new byte span.
        ///     The input buffer must match the internal buffer size exactly.
        ///     Uses hardware-accelerated AVX2 instructions for bulk copy if supported.
        /// </summary>
        /// <param name="fullBuffer">The source byte span representing the full image buffer to copy.</param>
        /// <exception cref="ArgumentException">Thrown if the input buffer length does not match the internal buffer size.</exception>
        public void ReplaceBuffer(ReadOnlySpan<byte> fullBuffer)
        {
            if (fullBuffer.Length != _bufferSize)
            {
                throw new ArgumentException(RenderResource.ErrorInputBuffer);
            }

            var buffer = BufferSpan;

            if (Avx2.IsSupported)
            {
                const int vectorSize = 32; // 256 bits / 8

                var simdCount = _bufferSize / vectorSize;
                var remainder = _bufferSize % vectorSize;

                fixed (byte* srcPtr = fullBuffer)
                {
                    var dstPtr = (byte*)_bufferPtr;

                    for (var i = 0; i < simdCount; i++)
                    {
                        var vec = Avx.LoadVector256(srcPtr + (i * vectorSize));
                        Avx.Store(dstPtr + (i * vectorSize), vec);
                    }

                    // Copy any remaining bytes one by one
                    for (var i = _bufferSize - remainder; i < _bufferSize; i++)
                    {
                        buffer[i] = fullBuffer[i];
                    }
                }
            }
            else
            {
                // Fallback: simple managed copy
                fullBuffer.CopyTo(buffer);
            }
        }

        /// <summary>
        ///     Retrieves a span representing a horizontal sequence of pixels starting at (x, y).
        ///     The span length is equal to count pixels, each containing bytes per pixel.
        /// </summary>
        /// <param name="x">The starting horizontal pixel coordinate (0-based).</param>
        /// <param name="y">The vertical pixel coordinate (0-based).</param>
        /// <param name="count">The number of consecutive pixels to retrieve.</param>
        /// <returns>A span of bytes representing the requested pixels.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if the requested pixel range is out of bounds of the image dimensions.
        /// </exception>
        public Span<byte> GetPixelSpan(int x, int y, int count)
        {
            if (x < 0 || y < 0 || x + count > Width || y >= Height)
            {
                throw new ArgumentOutOfRangeException();
            }

            var offset = GetPixelOffset(x, y);
            var length = count * _bytesPerPixel;
            return BufferSpan.Slice(offset, length);
        }

        /// <summary>
        ///     Converts to bitmap.
        /// </summary>
        /// <returns>UnmanagedImageBuffer to BitmapBuffer</returns>
        public Bitmap ToBitmap()
        {
            var bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            var bmpData = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
            try
            {
                Buffer.MemoryCopy(
                    (void*)_bufferPtr,
                    bmpData.Scan0.ToPointer(),
                    _bufferSize,
                    _bufferSize);
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }

            return bmp;
        }


        /// <summary>
        ///     Froms the bitmap.
        /// </summary>
        /// <param name="bmp">The BMP.</param>
        /// <returns>Bitmap converted to UnmanagedImageBuffer</returns>
        public static UnmanagedImageBuffer FromBitmap(Bitmap bmp)
        {
            // Assumes Format32bppArgb
            var buffer = new UnmanagedImageBuffer(bmp.Width, bmp.Height);
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                var src = new Span<byte>((void*)data.Scan0, data.Stride * bmp.Height);
                buffer.ReplaceBuffer(src.Slice(0, buffer.BufferSpan.Length));
            }
            finally
            {
                bmp.UnlockBits(data);
            }

            return buffer;
        }

        /// <summary>
        ///     Blits the specified source.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="destX">The dest x.</param>
        /// <param name="destY">The dest y.</param>
        public void Blit(UnmanagedImageBuffer src, int destX, int destY)
        {
            for (var y = 0; y < src.Height; y++)
            {
                if (y + destY >= Height)
                {
                    break;
                }

                var srcRow = src.GetPixelSpan(0, y, src.Width);
                var dstRow = GetPixelSpan(destX, destY + y, src.Width);
                srcRow.CopyTo(dstRow);
            }
        }

        /// <summary>
        ///     Blits a rectangular region from the source buffer to this buffer.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="srcX">X-coordinate of the top-left corner in the source.</param>
        /// <param name="srcY">Y-coordinate of the top-left corner in the source.</param>
        /// <param name="width">Width of the region to copy.</param>
        /// <param name="height">Height of the region to copy.</param>
        /// <param name="destX">The dest x.</param>
        /// <param name="destY">The dest y.</param>
        public void BlitRegion(UnmanagedImageBuffer src, int srcX, int srcY, int width, int height, int destX,
            int destY)
        {
            for (var y = 0; y < height; y++)
            {
                var srcRow = src.GetPixelSpan(srcX, srcY + y, width);
                var dstRow = GetPixelSpan(destX, destY + y, width);
                srcRow.CopyTo(dstRow);
            }
        }

        /// <summary>
        ///     Packs the bgra.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        /// <returns>Converts color to an unsigned Int</returns>
        public static uint PackBgra(byte a, byte r, byte g, byte b)
        {
            return ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | b;
        }
    }
}
