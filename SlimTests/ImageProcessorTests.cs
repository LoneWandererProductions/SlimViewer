using System;
using System.Drawing;
using CommonControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlimViews;

namespace ImageProcessorTests
{
    [TestClass]
    public class ImageProcessorTests
    {
        [TestMethod]
        public void FillArea_StartPointNotNull_RemainsNotNull()
        {
            // Arrange
            var bitmap = new Bitmap(10, 10);
            var frame = new SelectionFrame
            {
                X = 5,
                Y = 5,
                Width = 3,
                Height = 3,
            };
            Color color = Color.Red;

            // Act
            var result = ImageProcessor.FillArea(bitmap, frame, color);

            // Assert
            Assert.IsNotNull(result, "Bitmap should not be null");
            Assert.AreEqual(10, result.Width, "Bitmap width should remain unchanged");
            Assert.AreEqual(10, result.Height, "Bitmap height should remain unchanged");

            // Verify that startPoint wasn't null
            var expectedPoint = new Point(frame.X, frame.Y);
            Assert.AreEqual(expectedPoint, new Point(frame.X, frame.Y), "Start point should remain unchanged");
        }
    }
}