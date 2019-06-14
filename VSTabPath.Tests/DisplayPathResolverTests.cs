using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSTabPath.Models;

namespace VSTabPath.Tests
{
    [TestClass]
    public class DisplayPathResolverTests
    {
        [TestMethod]
        public void WhenNoDuplicateFileNames_DoNotShowPaths()
        {
            var models = new[]
            {
                new TabModel(@"c:\Directory1\1.txt"), 
                new TabModel(@"c:\Directory1\2.txt"), 
                new TabModel(@"c:\Directory2\3.txt"), 
            };

            var resolver = new DisplayPathResolver
            {
                models[0],
                models[1],
                models[2],
            };

            Assert.AreEqual(null, models[0].DisplayPath);
            Assert.AreEqual(null, models[1].DisplayPath);
            Assert.AreEqual(null, models[2].DisplayPath);
        }

        [TestMethod]
        public void WhenDuplicateFileNames_ShowPaths()
        {
            var models = new[]
            {
                new TabModel(@"c:\Directory1\1.txt"), 
                new TabModel(@"c:\Directory1\2.txt"), 
                new TabModel(@"c:\Directory2\1.txt"), 
            };

            var resolver = new DisplayPathResolver
            {
                models[0],
                models[1],
                models[2],
            };

            Assert.AreEqual("Directory1", models[0].DisplayPath);
            Assert.AreEqual(null, models[1].DisplayPath);
            Assert.AreEqual("Directory2", models[2].DisplayPath);
        }

        [TestMethod]
        public void WhenNewTabIsAdded_UpdatePaths()
        {
            var models = new[]
            {
                new TabModel(@"c:\Directory1\1.txt"), 
                new TabModel(@"c:\Directory1\2.txt"), 
                new TabModel(@"c:\Directory2\1.txt"), 
            };

            var resolver = new DisplayPathResolver
            {
                models[0],
                models[1],
            };

            Assert.AreEqual(null, models[0].DisplayPath);
            Assert.AreEqual(null, models[1].DisplayPath);

            resolver.Add(models[2]);

            Assert.AreEqual("Directory1", models[0].DisplayPath);
            Assert.AreEqual(null, models[1].DisplayPath);
            Assert.AreEqual("Directory2", models[2].DisplayPath);
        }

        [TestMethod]
        public void WhenNewTabIsRemoved_UpdatePaths()
        {
            var models = new[]
            {
                new TabModel(@"c:\Directory1\1.txt"), 
                new TabModel(@"c:\Directory1\2.txt"), 
                new TabModel(@"c:\Directory2\1.txt"), 
            };

            var resolver = new DisplayPathResolver
            {
                models[0],
                models[1],
                models[2],
            };

            Assert.AreEqual("Directory1", models[0].DisplayPath);
            Assert.AreEqual(null, models[1].DisplayPath);
            Assert.AreEqual("Directory2", models[2].DisplayPath);

            resolver.Remove(models[2]);

            Assert.AreEqual(null, models[0].DisplayPath);
            Assert.AreEqual(null, models[1].DisplayPath);
        }

        [TestMethod]
        public void WhenNewTabIsRenamed_UpdatePaths()
        {
            var models = new[]
            {
                new TabModel(@"c:\Directory1\1.txt"), 
                new TabModel(@"c:\Directory1\2.txt"), 
                new TabModel(@"c:\Directory2\3.txt"), 
            };

            var resolver = new DisplayPathResolver
            {
                models[0],
                models[1],
                models[2],
            };

            Assert.AreEqual(null, models[0].DisplayPath);
            Assert.AreEqual(null, models[1].DisplayPath);
            Assert.AreEqual(null, models[2].DisplayPath);

            models[2].FullPath = @"c:\Directory2\1.txt";

            Assert.AreEqual("Directory1", models[0].DisplayPath);
            Assert.AreEqual(null, models[1].DisplayPath);
            Assert.AreEqual("Directory2", models[2].DisplayPath);
        }

        [TestMethod]
        public void WhenNewTabIsMoved_UpdatePaths()
        {
            var models = new[]
            {
                new TabModel(@"c:\Directory1\1.txt"), 
                new TabModel(@"c:\Directory1\2.txt"), 
                new TabModel(@"c:\Directory2\1.txt"), 
            };

            var resolver = new DisplayPathResolver
            {
                models[0],
                models[1],
                models[2],
            };

            Assert.AreEqual("Directory1", models[0].DisplayPath);
            Assert.AreEqual(null, models[1].DisplayPath);
            Assert.AreEqual("Directory2", models[2].DisplayPath);

            models[2].FullPath = @"c:\Directory3\1.txt";

            Assert.AreEqual("Directory1", models[0].DisplayPath);
            Assert.AreEqual(null, models[1].DisplayPath);
            Assert.AreEqual("Directory3", models[2].DisplayPath);
        }
    }
}
