using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSTabPath.Models;

namespace VSTabPath.Tests
{
    [TestClass]
    public class DisplayPathResolverTests_Solution
    {
        [TestMethod]
        public void WhenNoDuplicateFileNames_DoNotShowPaths()
        {
            var models = new[]
            {
                new TabModel(@"c:\Solution\Directory1\1.txt"),
                new TabModel(@"c:\Solution\Directory1\2.txt"),
                new TabModel(@"c:\Solution\Directory2\3.txt"),
            };

            var resolver = new DisplayPathResolver(@"c:\Solution")
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
                new TabModel(@"c:\Solution\Directory1\1.txt"),
                new TabModel(@"c:\Solution\Directory1\2.txt"),
                new TabModel(@"c:\Solution\Directory2\1.txt"),
            };

            var resolver = new DisplayPathResolver(@"c:\Solution")
            {
                models[0],
                models[1],
                models[2],
            };

            Assert.AreEqual(@"Directory1", models[0].DisplayPath);
            Assert.AreEqual(null, models[1].DisplayPath);
            Assert.AreEqual(@"Directory2", models[2].DisplayPath);
        }

        [TestMethod]
        public void WhenDuplicatesInDeepSiblingDirectories_ShowPathsWithEllipsis()
        {
            var models = new[]
            {
                new TabModel(@"c:\Solution\Project\Directory1\1.txt"),
                new TabModel(@"c:\Solution\Project\Directory2\1.txt"),
            };

            var resolver = new DisplayPathResolver(@"c:\Solution")
            {
                models[0],
                models[1],
            };

            Assert.AreEqual(@"…\Directory1", models[0].DisplayPath);
            Assert.AreEqual(@"…\Directory2", models[1].DisplayPath);
        }

        [TestMethod]
        public void WhenDuplicatesInVeryDeepSiblingDirectories_ShowPathsWithSingleEllipsis()
        {
            var models = new[]
            {
                new TabModel(@"c:\Solution\Project\very\deep\Directory1\1.txt"),
                new TabModel(@"c:\Solution\Project\very\deep\Directory2\1.txt"),
            };

            var resolver = new DisplayPathResolver(@"c:\Solution")
            {
                models[0],
                models[1],
            };

            Assert.AreEqual(@"…\Directory1", models[0].DisplayPath);
            Assert.AreEqual(@"…\Directory2", models[1].DisplayPath);
        }

        [TestMethod]
        public void WhenDuplicatesInSameNamedDirectories_ShowPathsEndWithMultipleDirectory()
        {
            var models = new[]
            {
                new TabModel(@"c:\Solution\Project1\SubDirectory1\1.txt"),
                new TabModel(@"c:\Solution\Project2\SubDirectory1\1.txt"),
            };

            var resolver = new DisplayPathResolver(@"c:\Solution")
            {
                models[0],
                models[1],
            };

            Assert.AreEqual(@"Project1\SubDirectory1", models[0].DisplayPath);
            Assert.AreEqual(@"Project2\SubDirectory1", models[1].DisplayPath);
        }

        [TestMethod]
        public void WhenOneDuplicateOutsideSolutionRoot_ShowPathWithDriveLetter()
        {
            var models = new[]
            {
                new TabModel(@"c:\Solution\Project1\SubDirectory1\1.txt"),
                new TabModel(@"c:\Other\SubDirectory1\1.txt"),
            };

            var resolver = new DisplayPathResolver(@"c:\Solution")
            {
                models[0],
                models[1],
            };

            Assert.AreEqual(@"…\SubDirectory1", models[0].DisplayPath);
            Assert.AreEqual(@"c:\…\SubDirectory1", models[1].DisplayPath);
        }

        [TestMethod]
        public void WhenOneDuplicateAtSolutionRoot_ShowDotPath()
        {
            var models = new[]
            {
                new TabModel(@"c:\Solution\Project1\1.txt"),
                new TabModel(@"c:\Solution\1.txt"),
            };

            var resolver = new DisplayPathResolver(@"c:\Solution")
            {
                models[0],
                models[1],
            };

            Assert.AreEqual(@"Project1", models[0].DisplayPath);
            Assert.AreEqual(@".\", models[1].DisplayPath);
        }

        [TestMethod]
        public void WhenSolutionIsLoaded_UpdatePaths()
        {
            var models = new[]
            {
                new TabModel(@"c:\Solution\Project1\1.txt"),
                new TabModel(@"c:\Solution\Project2\1.txt"),
            };

            var resolver = new DisplayPathResolver
            {
                models[0],
                models[1],
            };

            Assert.AreEqual(@"c:\…\Project1", models[0].DisplayPath);
            Assert.AreEqual(@"c:\…\Project2", models[1].DisplayPath);

            resolver.SolutionRootPath = @"c:\Solution";

            Assert.AreEqual(@"Project1", models[0].DisplayPath);
            Assert.AreEqual(@"Project2", models[1].DisplayPath);
        }

        [TestMethod]
        public void WhenSolutionIsUnloaded_UpdatePaths()
        {
            var models = new[]
            {
                new TabModel(@"c:\Solution\Project1\1.txt"),
                new TabModel(@"c:\Solution\Project2\1.txt"),
            };

            var resolver = new DisplayPathResolver(@"c:\Solution")
            {
                models[0],
                models[1],
            };

            Assert.AreEqual(@"Project1", models[0].DisplayPath);
            Assert.AreEqual(@"Project2", models[1].DisplayPath);
            
            resolver.SolutionRootPath = null;

            Assert.AreEqual(@"c:\…\Project1", models[0].DisplayPath);
            Assert.AreEqual(@"c:\…\Project2", models[1].DisplayPath);
        }

        [TestMethod]
        public void WhenSolutionIsChanged_UpdatePaths()
        {
            var models = new[]
            {
                new TabModel(@"c:\Solution1\Project1\1.txt"),
                new TabModel(@"c:\Solution2\Project2\1.txt"),
            };

            var resolver = new DisplayPathResolver(@"c:\Solution1")
            {
                models[0],
                models[1],
            };

            Assert.AreEqual(@"Project1", models[0].DisplayPath);
            Assert.AreEqual(@"c:\…\Project2", models[1].DisplayPath);
            
            resolver.SolutionRootPath = @"c:\Solution2";

            Assert.AreEqual(@"c:\…\Project1", models[0].DisplayPath);
            Assert.AreEqual(@"Project2", models[1].DisplayPath);
        }
    }
}
