/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimTests
 * FILE:        SlimTests/RenameViewTests.cs
 * PURPOSE:     I wanted to avoid it, but here we are tests for Slimviewer
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlimViews;

namespace SlimTests
{
    /// <summary>
    /// Basic tests for renamer
    /// </summary>
    [TestClass]
    public class RenameViewTests
    {
        private RenameView _renameView;

        private string _directory;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _directory = Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/Rename").FullName;

            // Create the files if they do not exist
            CreateTestFile(Path.Combine(_directory, "file1.txt"), "This is file 1.");
            CreateTestFile(Path.Combine(_directory, "file2.txt"), "This is file 2.");
            CreateTestFile(Path.Combine(_directory, "file3.txt"), "This is file 3.");

            _renameView = new RenameView
            {
                Observer = new ConcurrentDictionary<int, string>(new[]
                {
                    new KeyValuePair<int, string>(1, Path.Combine(_directory, "file1.txt")),
                    new KeyValuePair<int, string>(2, Path.Combine(_directory, "file2.txt")),
                    new KeyValuePair<int, string>(3, Path.Combine(_directory, "file3.txt"))
                })
            };
        }

        /// <summary>
        /// Cleanups the test directory.
        /// </summary>
        private void CleanupTestDirectory()
        {
            if (!Directory.Exists(_directory)) return;
            // Delete all files in the directory
            foreach (var file in Directory.GetFiles(_directory))
            {
                File.Delete(file);
            }

            // Optionally, delete the directory itself if you want a fresh start each time
            // Directory.Delete(_directory, true);
        }

        /// <summary>
        /// Creates the test file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="content">The content.</param>
        private static void CreateTestFile(string path, string content)
        {
            File.WriteAllText(path, content); // Create the file with the specified content
        }

        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            CleanupTestDirectory(); // Clean up after each test
        }

        /// <summary>
        /// Tests the private method add action asynchronous.
        /// </summary>
        [TestMethod]
        public async Task TestPrivateMethod_AddActionAsync()
        {
            _renameView.Replacement = "new_";
            // Act
            var method = typeof(RenameView).GetMethod("AddActionAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            try
            {
                await (Task)method.Invoke(_renameView, new object[] { null });
            }
            catch (TargetInvocationException ex)
            {
                Assert.Fail($"Method invocation failed: {ex.InnerException?.Message}");
            }

            // Assert - Compare only the filenames
            Assert.AreEqual("new_file1.txt", Path.GetFileName(_renameView.Observer[1]));
            Assert.AreEqual("new_file2.txt", Path.GetFileName(_renameView.Observer[2]));
            Assert.AreEqual("new_file3.txt", Path.GetFileName(_renameView.Observer[3]));
        }

        /// <summary>
        /// Tests the private method remove appendage action asynchronous.
        /// </summary>
        [TestMethod]
        public async Task TestPrivateMethod_RemoveAppendageActionAsync()
        {
            // Arrange
            _renameView.Replacement = "_new";
            // Act
            var method = typeof(RenameView).GetMethod("RemoveAppendageActionAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            await (Task)method.Invoke(_renameView, new object[] { null });

            // Assert
            Assert.AreEqual("file1.txt", Path.GetFileName(_renameView.Observer[1]));
            Assert.AreEqual("file2.txt", Path.GetFileName(_renameView.Observer[2]));
            Assert.AreEqual("file3.txt", Path.GetFileName(_renameView.Observer[3]));
        }

        /// <summary>
        /// Tests the private method replace command action asynchronous.
        /// </summary>
        [TestMethod]
        public async Task TestPrivateMethod_ReplaceCommandActionAsync()
        {
            // Arrange
            _renameView.Replacement = "file";
            _renameView.Replacer = "document";

            // Act
            var method = typeof(RenameView).GetMethod("ReplaceCommandActionAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            await (Task)method.Invoke(_renameView, new object[] { null });

            // Assert
            Assert.AreEqual("document1.txt", Path.GetFileName(_renameView.Observer[1]));
            Assert.AreEqual("document2.txt", Path.GetFileName(_renameView.Observer[2]));
            Assert.AreEqual("document3.txt", Path.GetFileName(_renameView.Observer[3]));
        }

        /// <summary>
        /// Tests the private method appendages at action asynchronous.
        /// </summary>
        [TestMethod]
        public async Task TestPrivateMethod_AppendagesAtActionAsync()
        {
            // Arrange
            _renameView.Numbers = 4; // Remove first 4 characters

            // Act
            var method = typeof(RenameView).GetMethod("AppendagesAtActionAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            await (Task)method.Invoke(_renameView, new object[] { null });

            // Assert
            Assert.AreEqual("1.txt", Path.GetFileName(_renameView.Observer[1]));
            Assert.AreEqual("2.txt", Path.GetFileName(_renameView.Observer[2]));
            Assert.AreEqual("3.txt", Path.GetFileName(_renameView.Observer[3]));
        }

        /// <summary>
        /// Tests the private method reorder command action asynchronous.
        /// </summary>
        [TestMethod]
        public async Task TestPrivateMethod_ReorderCommandActionAsync()
        {
            // Act
            var method = typeof(RenameView).GetMethod("ReorderCommandActionAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            await (Task)method.Invoke(_renameView, new object[] { null });

            // Assert
            Assert.AreEqual("file_1.txt", Path.GetFileName(_renameView.Observer[1]));
            Assert.AreEqual("file_2.txt", Path.GetFileName(_renameView.Observer[2]));
            Assert.AreEqual("file_3.txt", Path.GetFileName(_renameView.Observer[3]));
        }
    }
}
