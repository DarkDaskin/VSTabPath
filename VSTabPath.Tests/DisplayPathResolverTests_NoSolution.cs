using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSTabPath.Models;

namespace VSTabPath.Tests
{
    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class DisplayPathResolverTests_NoSolution
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

            Assert.AreEqual(@"c:\Directory1", models[0].DisplayPath);
            Assert.AreEqual(null, models[1].DisplayPath);
            Assert.AreEqual(@"c:\Directory2", models[2].DisplayPath);
        }

        [TestMethod]
        public void WhenDuplicatesInDeepSiblingDirectories_ShowPathsWithEllipsis()
        {
            var models = new[]
            {
                new TabModel(@"c:\root\Directory1\1.txt"), 
                new TabModel(@"c:\root\Directory2\1.txt"), 
            };

            var resolver = new DisplayPathResolver
            {
                models[0],
                models[1],
            };

            Assert.AreEqual(@"c:\…\Directory1", models[0].DisplayPath);
            Assert.AreEqual(@"c:\…\Directory2", models[1].DisplayPath);
        }

        [TestMethod]
        public void WhenDuplicatesInVeryDeepSiblingDirectories_ShowPathsWithSingleEllipsis()
        {
            var models = new[]
            {
                new TabModel(@"c:\root\very\deep\Directory1\1.txt"), 
                new TabModel(@"c:\root\very\deep\Directory2\1.txt"), 
            };

            var resolver = new DisplayPathResolver
            {
                models[0],
                models[1],
            };

            Assert.AreEqual(@"c:\…\Directory1", models[0].DisplayPath);
            Assert.AreEqual(@"c:\…\Directory2", models[1].DisplayPath);
        }

        [TestMethod]
        public void WhenDuplicatesInNonSiblingDirectories_ShowPathsEndWithSingleDirectory()
        {
            var models = new[]
            {
                new TabModel(@"c:\root\Directory1\1.txt"), 
                new TabModel(@"c:\root\1.txt"), 
            };

            var resolver = new DisplayPathResolver
            {
                models[0],
                models[1],
            };

            Assert.AreEqual(@"c:\…\Directory1", models[0].DisplayPath);
            Assert.AreEqual(@"c:\root", models[1].DisplayPath);
        }

        [TestMethod]
        public void WhenDuplicatesInSameNamedDirectories_ShowPathsEndWithMultipleDirectory()
        {
            var models = new[]
            {
                new TabModel(@"c:\root\Directory1\SubDirectory1\1.txt"), 
                new TabModel(@"c:\root\Directory2\SubDirectory1\1.txt"), 
            };

            var resolver = new DisplayPathResolver
            {
                models[0],
                models[1],
            };

            // TODO: VSCode has better display: c:\…\Directory1\…
            Assert.AreEqual(@"c:\…\Directory1\SubDirectory1", models[0].DisplayPath);
            Assert.AreEqual(@"c:\…\Directory2\SubDirectory1", models[1].DisplayPath);
        }

        [TestMethod]
        public void WhenDuplicatesDifferByCase_ShowPaths()
        {
            var models = new[]
            {
                new TabModel(@"c:\root\Directory1\1.txt"), 
                new TabModel(@"c:\Root\Directory2\1.txt"),
                new TabModel(@"c:\root\Directory1\2.txt"), 
                new TabModel(@"c:\root\Directory2\2.TXT"),             
            };

            var resolver = new DisplayPathResolver
            {
                models[0],
                models[1],
                models[2],
                models[3],
            };

            Assert.AreEqual(@"c:\…\Directory1", models[0].DisplayPath);
            Assert.AreEqual(@"c:\…\Directory2", models[1].DisplayPath);
            Assert.AreEqual(@"c:\…\Directory1", models[2].DisplayPath);
            Assert.AreEqual(@"c:\…\Directory2", models[3].DisplayPath);
        }

        // TODO: VSCode with different drives: c:\… d:\…

        [TestMethod]
        public void WhenDuplicateFullPaths_DoNotShowPaths()
        {
            var models = new[]
            {
                new TabModel(@"c:\root\Directory1\1.txt"),
                new TabModel(@"c:\root\Directory1\1.txt"),
                new TabModel(@"c:\Root\Directory1\1.txt"),
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
        public void WhenDuplicatesHaveSamePaths_ShowPaths()
        {
            var models = new[]
            {
                new TabModel(@"c:\root\Directory1\1.txt"),
                new TabModel(@"c:\root\Directory1\1.txt"),
                new TabModel(@"c:\Root\Directory2\1.txt"),
            };

            var resolver = new DisplayPathResolver
            {
                models[0],
                models[1],
                models[2],
            };
            Assert.AreEqual(@"c:\…\Directory1", models[0].DisplayPath);
            Assert.AreEqual(@"c:\…\Directory1", models[1].DisplayPath);
            Assert.AreEqual(@"c:\…\Directory2", models[2].DisplayPath);
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

            Assert.AreEqual(@"c:\Directory1", models[0].DisplayPath);
            Assert.AreEqual(null, models[1].DisplayPath);
            Assert.AreEqual(@"c:\Directory2", models[2].DisplayPath);
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

            Assert.AreEqual(@"c:\Directory1", models[0].DisplayPath);
            Assert.AreEqual(null, models[1].DisplayPath);
            Assert.AreEqual(@"c:\Directory2", models[2].DisplayPath);

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

            Assert.AreEqual(@"c:\Directory1", models[0].DisplayPath);
            Assert.AreEqual(null, models[1].DisplayPath);
            Assert.AreEqual(@"c:\Directory2", models[2].DisplayPath);
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

            Assert.AreEqual(@"c:\Directory1", models[0].DisplayPath);
            Assert.AreEqual(null, models[1].DisplayPath);
            Assert.AreEqual(@"c:\Directory2", models[2].DisplayPath);

            models[2].FullPath = @"c:\Directory3\1.txt";

            Assert.AreEqual(@"c:\Directory1", models[0].DisplayPath);
            Assert.AreEqual(null, models[1].DisplayPath);
            Assert.AreEqual(@"c:\Directory3", models[2].DisplayPath);
        }
    }
}
