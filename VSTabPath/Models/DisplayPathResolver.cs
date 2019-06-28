using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace VSTabPath.Models
{
    public class DisplayPathResolver : IEnumerable<TabModel>
    {
        private const string Ellipsis = "…";

        private static readonly char[] DirectorySeparators =
            {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar};
        private static readonly IEqualityComparer<string> PathComparer = StringComparer.OrdinalIgnoreCase;

        private Dictionary<string, List<TabModel>> _modelsByFullPath = new Dictionary<string, List<TabModel>>(PathComparer);
        private string _solutionRootPath;

        private IEnumerable<TabModel> ModelsWithUniqueFullPaths => _modelsByFullPath.Values.Select(list => list[0]);

        public string SolutionRootPath
        {
            get => _solutionRootPath;
            set
            {
                _solutionRootPath = value;

                UpdateModels();
            }
        }

        public DisplayPathResolver()
        {
        }

        public DisplayPathResolver(string solutionRootPath)
        {
            _solutionRootPath = solutionRootPath;
        }

        public void Add(TabModel model)
        {
            if (!_modelsByFullPath.TryGetValue(model.FullPath, out var models))
                _modelsByFullPath.Add(model.FullPath, models = new List<TabModel>());
            models.Add(model);

            model.PropertyChanged += OnModelPropertyChanged;

            UpdateModels(model.FileName);
        }

        public void Remove(TabModel model)
        {
            model.PropertyChanged -= OnModelPropertyChanged;

            if (_modelsByFullPath.TryGetValue(model.FullPath, out var models))
            {
                models.Remove(model);
                if (models.Count == 0)
                    _modelsByFullPath.Remove(model.FullPath);
            }

            UpdateModels(model.FileName);
        }

        public IEnumerator<TabModel> GetEnumerator()
        {
            return _modelsByFullPath.Values.SelectMany(m => m).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(TabModel.FullPath))
            {
                RegroupTabs();
                UpdateModels();
            }
        }

        private void RegroupTabs()
        {
            _modelsByFullPath = this
                .GroupBy(m => m.FullPath)
                .ToDictionary(g => g.Key, g => g.ToList(), PathComparer);
        }

        private void UpdateModels(string fileName)
        {
            var modelsToUpdate = ModelsWithUniqueFullPaths
                .Where(m => PathEquals(m.FileName, fileName))
                .ToList();
            if (modelsToUpdate.Count == 1)
                UpdateDisplayPathsByFullPath(modelsToUpdate[0], null);
            else
                UpdateModelsWithDuplicateFilename(modelsToUpdate);
        }

        private void UpdateModels()
        {
            var modelsByFileName = ModelsWithUniqueFullPaths
                .ToLookup(m => m.FileName, PathComparer);

            var modelsWithDuplicateFileName = modelsByFileName
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .ToList();
            UpdateModelsWithDuplicateFilename(modelsWithDuplicateFileName);

            var modelsWithUniqueFileName = modelsByFileName
                .Where(g => g.Count() == 1)
                .SelectMany(g => g);
            foreach (var model in modelsWithUniqueFileName)
                UpdateDisplayPathsByFullPath(model, null);
        }

        private void UpdateDisplayPathsByFullPath(TabModel example, string displayPath)
        {
            foreach (var model in _modelsByFullPath[example.FullPath])
                model.DisplayPath = displayPath;
        }

        private void UpdateModelsWithDuplicateFilename(IReadOnlyCollection<TabModel> models)
        {
            var modelsAtSolutionRoot = models
                .Where(m => PathEquals(m.DirectoryName, SolutionRootPath));
            foreach (var model in modelsAtSolutionRoot)
                UpdateDisplayPathsByFullPath(model, @".\");

            var modelsOutsideSolutionRoot = models
                .Where(m => !IsBaseOf(SolutionRootPath, m.DirectoryName) && !PathEquals(m.DirectoryName, SolutionRootPath));
            ResolvePartialDisplayPaths(modelsOutsideSolutionRoot, false);

            var modelsUnderSolutionRoot = models
                .Where(m => IsBaseOf(SolutionRootPath, m.DirectoryName));
            ResolvePartialDisplayPaths(modelsUnderSolutionRoot, true);
        }

        private static bool PathEquals(string x, string y)
        {
            return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsBaseOf(string root, string path)
        {
            if (root == null)
                return false;
            return new Uri(Path.Combine(root, ".")).IsBaseOf(new Uri(path));
        }

        private static string GetRelativePath(string root, string path)
        {
            return new Uri(Path.Combine(root, ".")).MakeRelativeUri(new Uri(path)).ToString()
                .Replace('/', '\\');
        }

        private void ResolvePartialDisplayPaths(IEnumerable<TabModel> models, bool isUnderSolutionRoot)
        {
            var modelsByFileName = models.GroupBy(m => m.FileName, 
                (key, g) => g.ToDictionary(m => m, 
                    m => GetPathParts(m.DirectoryName, isUnderSolutionRoot).Select(p => (p, false)).ToList()),
                StringComparer.OrdinalIgnoreCase);
            foreach (var group in modelsByFileName)
                ResolvePartialDisplayPaths(group, isUnderSolutionRoot);
        }

        private string[] GetPathParts(string path, bool isUnderSolutionRoot)
        {
            if (isUnderSolutionRoot)
                path = GetRelativePath(SolutionRootPath, path);

            return path.Split(DirectorySeparators, StringSplitOptions.RemoveEmptyEntries);
        }

        private void ResolvePartialDisplayPaths(Dictionary<TabModel, List<(string value, bool isIncluded)>> models, bool isUnderSolutionRoot)
        {
            // When path is outside solution root, always include the root segment (i.e. drive letter).
            // A separator has to be appended to keep it after Path.Combine.
            if (!isUnderSolutionRoot)
                foreach (var segments in models.Values)
                    segments[0] = (segments[0].value + Path.DirectorySeparatorChar, true);

            ResolvePartialDisplayPaths(models);
        }

        private void ResolvePartialDisplayPaths(Dictionary<TabModel, List<(string value, bool isIncluded)>> models)
        {
            // Include one more segment from the end.
            foreach (var segments in models.Values)
            {
                for (var i = segments.Count - 1; i >= 0; i--)
                {
                    if (segments[i].isIncluded)
                        continue;

                    segments[i] = (segments[i].value, true);
                    break;
                }
            }

            // Replace gaps with ellipsis and build the display path.
            foreach (var model in models)
            {
                var finalSegments = new List<string>();
                var hasEllipsis = false;
                foreach (var (value, isIncluded) in model.Value)
                {
                    if (isIncluded)
                    {
                        hasEllipsis = false;
                        finalSegments.Add(value);
                        continue;
                    }

                    if (!hasEllipsis)
                    {
                        hasEllipsis = true;
                        finalSegments.Add(Ellipsis);
                    }
                }

                UpdateDisplayPathsByFullPath(model.Key, Path.Combine(finalSegments.ToArray()));
            }

            // Find ambiguous paths and resolve them.
            var modelsWithDuplicateDisplayPath = models
                .Where(m => m.Value.Any(p => !p.isIncluded))
                .GroupBy(m => m.Key.DisplayPath, (key, g) => g.ToDictionary(m => m.Key, m => m.Value),
                    StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1);
            foreach (var group in modelsWithDuplicateDisplayPath)
                ResolvePartialDisplayPaths(group);
        }
    }
}