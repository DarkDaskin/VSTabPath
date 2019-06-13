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
        private readonly TabTitleManager _tabTitleManager;

        public string Title => IsPathVisible ? Path.Combine(ParentDirectoryName, OriginalTitle) : OriginalTitle;

        public string OriginalTitle => _frame.Title;

        public string ParentDirectoryName => Path.GetFileName(Path.GetDirectoryName(_frame.FrameMoniker.Filename));

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

        public TabTitleProxy(WindowFrame frame, TabTitleManager tabTitleManager)
        {
            _frame = frame;
            _frame.PropertyChanged += OnFramePropertyChanged;
            _tabTitleManager = tabTitleManager;
        }

        private void OnFramePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WindowFrame.AnnotatedTitle))
            {
                OnPropertyChanged(nameof(Title));

                // File rename may change tab title uniqueness.
                _tabTitleManager.UpdateTabTitles();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}