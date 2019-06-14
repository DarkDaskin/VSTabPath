using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Platform.WindowManagement;

namespace VSTabPath
{
    public class WindowFrameTitleWithPath : INotifyPropertyChanged
    {
        private readonly WindowFrame _frame;
        private readonly TabTitleManager _titleManager;
        public WindowFrameTitle Title { get; private set; }
        public DataTemplate TitleTemplate { get; private set; }

        public string OriginalTitle => _frame.Title;

        public string DirectoryPath => Path.GetFileName(Path.GetDirectoryName(_frame.FrameMoniker.Filename));

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

        public WindowFrameTitleWithPath(WindowFrameTitle title, DataTemplate titleTemplate,
            WindowFrame frame, TabTitleManager titleManager)
        {
            Title = title;
            TitleTemplate = titleTemplate;
            _frame = frame;
            _frame.PropertyChanged += OnFramePropertyChanged;
            _titleManager = titleManager;
        }

        private void OnFramePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WindowFrame.AnnotatedTitle))
            {
                OnPropertyChanged(nameof(Title));

                // File rename may change tab title uniqueness.
                _titleManager.UpdateTabTitles();
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