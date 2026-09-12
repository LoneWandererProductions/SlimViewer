/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Plugins
 * FILE:        WebPDecoderPlugin.cs
 * PURPOSE:     IImageDecoderPlugin implementation for WebP images using Imazen.WebP.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.Drawing;
using Imaging.Plugins.Interface;
using Imazen.WebP;

namespace Imaging.Plugins
{
    /// <summary>
    ///     Decodes WebP images using the lightweight Imazen.WebP micro-library.
    /// </summary>
    public sealed class WebPDecoderPlugin : IImageDecoderPlugin
    {
        /// <inheritdoc />
        public string Name => "WebP Decoder";

        /// <inheritdoc />
        public IReadOnlyCollection<string> SupportedExtensions { get; } = new[] { ".webp" };

        /// <inheritdoc />
        public bool CanDecode(byte[] header)
        {
            // A WebP file must be at least 12 bytes long to check RIFF + Webp container markers
            if (header == null || header.Length < 12)
            {
                return false;
            }

            // Check for "RIFF" at bytes 0-3 and "WEBP" at bytes 8-11
            bool isRiffWebp = header[0] == 'R' &&
                               header[1] == 'I' &&
                               header[2] == 'F' &&
                               header[3] == 'F' &&
                               header[8] == 'W' &&
                               header[9] == 'E' &&
                               header[10] == 'B' &&
                               header[11] == 'P';

            if (!isRiffWebp)
            {
                return false;
            }

            // Optional: If you want to verify the sub-chunk type (VP8, VP8L, VP8X) at bytes 12-15
            if (header.Length >= 16)
            {
                // header[12..15] contains 'VP8 ', 'VP8L', or 'VP8X'
                // You can inspect them here if needed, or just return true for any valid WebP container
            }

            return true;
        }

        /// <inheritdoc />
        public Bitmap Decode(string path)
        {
            // Read the file into memory since the micro-library expects raw bytes
            var fileBytes = File.ReadAllBytes(path);

            var decoder = new SimpleDecoder();
            return decoder.DecodeFromBytes(fileBytes, fileBytes.Length);
        }
    }
}