using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace VSTabPath.Models
{
    public class DisplayPathResolver : IEnumerable<TabModel>
    {
        private readonly List<TabModel> _models = new List<TabModel>();

        public void Add(TabModel model)
        {
            _models.Add(model);

            model.PropertyChanged += OnModelPropertyChanged;

            UpdateModels(model.FileName);
        }

        public void Remove(TabModel model)
        {
            model.PropertyChanged -= OnModelPropertyChanged;

            _models.Remove(model);

            UpdateModels(model.FileName);
        }

        public IEnumerator<TabModel> GetEnumerator()
        {
            return _models.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if(args.PropertyName == nameof(TabModel.FullPath))
                UpdateModels();
        }

        private void UpdateModels(string fileName)
        {
            var modelsToUpdate = _models.Where(m => m.FileName == fileName).ToList();
            if (modelsToUpdate.Count == 1)
                modelsToUpdate[0].DisplayPath = null;
            else
            {
                foreach (var model in modelsToUpdate)
                    model.DisplayPath = GetParentDirectoryName(model);
            }
        }

        private void UpdateModels()
        {
            var modelsByFileName = _models.ToLookup(m => m.FileName);

            var modelsWithDuplicateFileName = modelsByFileName
                .Where(g => g.Count() > 1)
                .SelectMany(g => g);
            foreach (var model in modelsWithDuplicateFileName)
                model.DisplayPath = GetParentDirectoryName(model);

            var modelsWithUniqueFileName = modelsByFileName
                .Where(g => g.Count() == 1)
                .SelectMany(g => g);
            foreach (var model in modelsWithUniqueFileName)
                model.DisplayPath = null;
        }

        private static string GetParentDirectoryName(TabModel model)
        {
            return Path.GetFileName(Path.GetDirectoryName(model.FullPath));
        }
    }
}