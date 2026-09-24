/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging.Objects.Tests
 * FILE:        TestSupport.cs
 * PURPOSE:     Small helpers shared by the document tests.
 */

using Imaging.Objects;
using Imaging.Objects.Documents;
using Imaging.Objects.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Imaging.Objects.Tests
{
    internal static class TestSupport
    {
        /// <summary>Asserts that <paramref name="action" /> throws <typeparamref name="T" /> (or a subclass).</summary>
        internal static void Throws<T>(Action action)
            where T : Exception
        {
            try
            {
                action();
            }
            catch (T)
            {
                return;
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected {typeof(T).Name} but got {ex.GetType().Name}: {ex.Message}");
                return;
            }

            Assert.Fail($"Expected {typeof(T).Name} but nothing was thrown.");
        }

        internal static void Fill(RasterLayer layer, PixelRect area, byte r, byte g, byte b, byte a = 255)
        {
            for (var y = area.Y; y < area.Bottom; y++)
            {
                for (var x = area.X; x < area.Right; x++)
                {
                    layer.Pixels.SetPixel(x, y, r, g, b, a);
                }
            }
        }

        internal static (byte r, byte g, byte b, byte a) Pixel(RasterLayer layer, int x, int y)
        {
            return layer.Pixels.GetPixel(x, y);
        }

        internal static (byte r, byte g, byte b, byte a) Pixel(UnmanagedImageBuffer buffer, int x, int y)
        {
            return buffer.GetPixel(x, y);
        }

        internal static byte[] Snapshot(RasterLayer layer)
        {
            return layer.Pixels.BufferSpan.ToArray();
        }

        internal static bool SamePixels(RasterLayer layer, byte[] expected)
        {
            return layer.Pixels.BufferSpan.SequenceEqual(expected);
        }

        /// <summary>Asserts that two colours differ by at most <paramref name="tolerance" /> per channel.</summary>
        internal static void AssertColor((byte r, byte g, byte b, byte a) actual, (int r, int g, int b, int a) expected,
            int tolerance = 1)
        {
            var ok = Math.Abs(actual.r - expected.r) <= tolerance && Math.Abs(actual.g - expected.g) <= tolerance &&
                     Math.Abs(actual.b - expected.b) <= tolerance && Math.Abs(actual.a - expected.a) <= tolerance;

            Assert.IsTrue(ok, $"Expected ~{expected} but was {actual}.");
        }

        /// <summary>A rasterizer that fills the rectangle of every solid-filled RectShape. Good enough to test compositing.</summary>
        internal sealed class FakeRasterizer : IShapeRasterizer
        {
            internal int Calls { get; private set; }

            public void Rasterize(IReadOnlyList<Shape> shapes, UnmanagedImageBuffer target)
            {
                Calls++;

                foreach (var shape in shapes)
                {
                    if (shape is not RectShape rect || rect.Fill is not SolidFill solid) continue;

                    var a = (byte)(solid.Argb >> 24);
                    var r = (byte)(solid.Argb >> 16);
                    var g = (byte)(solid.Argb >> 8);
                    var b = (byte)solid.Argb;

                    for (var y = (int)rect.Y; y < (int)(rect.Y + rect.Height); y++)
                    {
                        for (var x = (int)rect.X; x < (int)(rect.X + rect.Width); x++)
                        {
                            target.SetPixel(x, y, r, g, b, a);
                        }
                    }
                }
            }
        }

        /// <summary>A command that always fails, to test that history stays consistent.</summary>
        internal sealed class FailingCommand : IDocumentCommand
        {
            public string Description => "Fail";

            public void Apply(Document document)
            {
                throw new InvalidOperationException("boom");
            }

            public void Revert(Document document)
            {
            }
        }
    }
}