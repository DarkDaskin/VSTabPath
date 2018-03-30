using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Platform.WindowManagement;

namespace VSTabPath
{
    public class TabTitleProxy : INotifyPropertyChanged
    {
        private readonly WindowFrame _frame;

        public string Title => IsPathVisible ? Path.Combine(ParentDirectoryName, OriginalTitle) : OriginalTitle;

        public string OriginalTitle => _frame.Title;

        public string ParentDirectoryName => Path.GetFileName(Path.GetDirectoryName(_frame.DocumentMoniker));

        private bool _isPathVisible;
        public bool IsPathVisible
        {
            get => _isPathVisible;
            set
            {
                if (value == _isPathVisible) return;
                _isPathVisible = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Title));
            }
        }

        public TabTitleProxy(WindowFrame frame)
        {
            _frame = frame;
            _frame.PropertyChanged += OnFramePropertyChanged;
        }

        private void OnFramePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Path information might be missing at solution load.
            if (e.PropertyName == nameof(WindowFrame.DocumentMoniker))
                OnPropertyChanged(nameof(Title));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}