/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Plugins
 * FILE:        WebPDecoderPlugin.cs
 * PURPOSE:     IImageDecoderPlugin implementation for WebP images using Imazen.WebP.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Imaging.Interfaces;
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
            // WebP magic number check (RIFF....WEBP specification)
            if (header == null || header.Length < 12)
            {
                return false;
            }

            return header[0] == 'R' && header[1] == 'I' && header[2] == 'F' && header[3] == 'F' &&
                   header[8] == 'W' && header[9] == 'E' && header[10] == 'B' && header[11] == 'P';
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