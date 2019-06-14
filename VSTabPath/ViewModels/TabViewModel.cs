using System.ComponentModel;
using System.Windows;
using Microsoft.VisualStudio.Platform.WindowManagement;
using VSTabPath.Models;

namespace VSTabPath.ViewModels
{
    public class TabViewModel : ObservableModel
    {
        public WindowFrameTitle Title { get; }
        public DataTemplate TitleTemplate { get; }
        public TabModel Model { get; }
        
        public string DisplayPath => Model.DisplayPath;

        public bool IsPathVisible => DisplayPath != null;

        public TabViewModel(WindowFrameTitle title, DataTemplate titleTemplate, TabModel model)
        {
            Title = title;
            TitleTemplate = titleTemplate;
            Model = model;
            Model.PropertyChanged += OnModelPropertyChanged;
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TabModel.DisplayPath))
            {
                OnPropertyChanged(nameof(DisplayPath));
                OnPropertyChanged(nameof(IsPathVisible));
            }
        }
    }
}