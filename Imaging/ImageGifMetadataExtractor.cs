/*
* COPYRIGHT:   See COPYING in the top level directory
* PROJECT:     Imaging
* FILE:        Imaging/ImageGifMetadataExtractor.cs
* PURPOSE:     Get all the infos of a gif
* PROGRAMER:   Peter Geinitz (Wayfarer)
*/

using System;
using System.IO;

namespace Imaging
{
    internal static class ImageGifMetadataExtractor
    {
        internal static ImageGifInfo ExtractGifMetadata(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found.", filePath);

            var metadata = new ImageGifInfo
            {
                Name = Path.GetFileName(filePath),
                Size = new FileInfo(filePath).Length
            };

            double lastFrameDelay = 0;

            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(stream);

            // Read GIF Header (6 bytes)
            metadata.Header = new string(reader.ReadChars(6)); // "GIF87a" or "GIF89a"
            if (!metadata.Header.StartsWith("GIF", StringComparison.Ordinal))
                throw new InvalidDataException("Not a valid GIF file.");

            // Logical Screen Descriptor (7 bytes)
            metadata.Width = reader.ReadInt16(); // Logical Screen Width
            metadata.Height = reader.ReadInt16(); // Logical Screen Height
            var packedFields = reader.ReadByte(); // Packed fields
            metadata.BackgroundColorIndex = reader.ReadByte(); // Background Color Index
            metadata.PixelAspectRatio = reader.ReadByte(); // Pixel Aspect Ratio

            // Check for Global Color Table
            metadata.HasGlobalColorTable = (packedFields & 0x80) != 0;
            metadata.ColorResolution = ((packedFields & 0x70) >> 4) + 1; // Color Resolution
            metadata.GlobalColorTableSize = metadata.HasGlobalColorTable
                ? 3 * (1 << ((packedFields & 0x07) + 1))
                : 0;

            if (metadata.HasGlobalColorTable)
                reader.BaseStream.Seek(metadata.GlobalColorTableSize, SeekOrigin.Current); // Skip global color table

            // Read Extensions and Frames
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var blockId = reader.ReadByte();

                // Skip padding or sub-block terminator
                if (blockId == 0x00)
                {
                    Console.WriteLine("Skipping padding or sub-block terminator (0x00)");
                    continue;
                }

                Console.WriteLine($"Processing block: 0x{blockId:X2}");

                byte packed;
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    blockId = reader.ReadByte();

                    // Skip padding or sub-block terminator
                    if (blockId == 0x00)
                    {
                        Console.WriteLine("Skipping padding or sub-block terminator (0x00)");
                        continue;
                    }

                    Console.WriteLine($"Processing block: 0x{blockId:X2}");

                    switch (blockId)
                    {
                        case 0x21: // Extension Introducer
                            var extensionLabel = reader.ReadByte();
                            if (extensionLabel == 0xFF) // Application Extension
                            {
                                var blockSize = reader.ReadByte(); // Block Size
                                var appIdentifier = new string(reader.ReadChars(8));
                                var appAuthCode = new string(reader.ReadChars(3));

                                if (appIdentifier == "NETSCAPE")
                                {
                                    var subBlockSize = reader.ReadByte();
                                    var loopFlag = reader.ReadByte();
                                    metadata.LoopCount = reader.ReadInt16();
                                }
                                else
                                {
                                    SkipExtensionBlocks(reader);
                                }
                            }
                            else if (extensionLabel == 0xF9) // Graphics Control Extension
                            {
                                reader.BaseStream.Seek(1, SeekOrigin.Current); // Skip Block Size
                                packed = reader.ReadByte();
                                var delay = reader.ReadInt16();
                                lastFrameDelay = delay / 100.0;
                                reader.BaseStream.Seek(1, SeekOrigin.Current); // Skip Transparent Color Index
                            }
                            else
                            {
                                SkipExtensionBlocks(reader);
                            }
                            break;

                        case 0x2C: // Image Descriptor
                            metadata.Frames.Add(new FrameInfo
                            {
                                Description = "Image Frame",
                                DelayTime = lastFrameDelay
                            });

                            // Handle Image Descriptor and Local Color Table
                            reader.BaseStream.Seek(9, SeekOrigin.Current);
                            packed = reader.ReadByte();
                            if ((packed & 0x80) != 0) // Local Color Table present
                            {
                                var tableSize = 3 * (1 << ((packed & 0x07) + 1));
                                reader.BaseStream.Seek(tableSize, SeekOrigin.Current);
                            }

                            // Skip image data sub-blocks
                            while (true)
                            {
                                var subBlockSize = reader.ReadByte();
                                if (subBlockSize == 0x00) break;
                                reader.BaseStream.Seek(subBlockSize, SeekOrigin.Current);
                            }

                            break;

                        case 0x3B: // Trailer
                            Console.WriteLine("GIF Trailer found, parsing complete.");
                            return metadata;

                        default:
                            Console.WriteLine($"Unknown block encountered: 0x{blockId:X2}. Skipping.");
                            SkipUnknownBlock(reader, blockId);
                            break;
                    }
                }


            }


            return metadata;
        }

        private static void SkipUnknownBlock(BinaryReader reader, byte blockId)
        {
            Console.WriteLine($"Skipping unknown block: 0x{blockId:X2}");
            while (true)
            {
                var subBlockSize = reader.ReadByte();
                if (subBlockSize == 0x00) break; // End of sub-blocks
                reader.BaseStream.Seek(subBlockSize, SeekOrigin.Current);
            }
        }


        /// <summary>
        /// Skips the extension blocks.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private static void SkipExtensionBlocks(BinaryReader reader)
        {
            while (true)
            {
                var blockSize = reader.ReadByte();
                if (blockSize == 0x00) // Terminator
                    break;

                Console.WriteLine($"Skipping extension block of size: {blockSize}");
                reader.BaseStream.Seek(blockSize, SeekOrigin.Current);
            }
        }

    }
}
