using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using EnvDTE;
using Microsoft.VisualStudio.Platform.WindowManagement;
using Microsoft.VisualStudio.PlatformUI.Shell;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSTabPath.Models;
using VSTabPath.ViewModels;

namespace VSTabPath
{
    public class TabTitleManager
    {
        #region TabTitleManagerProperty

        public static readonly DependencyProperty TabTitleManagerProperty = 
            DependencyProperty.RegisterAttached(
                nameof(TabTitleManager), typeof(TabTitleManager), typeof(TabTitleManager));

        public static TabTitleManager GetTabTitleManager(ViewGroup target)
        {
            return (TabTitleManager) target.GetValue(TabTitleManagerProperty);
        }

        public static void SetTabTitleManager(ViewGroup target, TabTitleManager value)
        {
            target.SetValue(TabTitleManagerProperty, value);
        }

        public static TabTitleManager EnsureTabTitleManager(ViewGroup target)
        {
            return EnsurePropertyValue(target, TabTitleManagerProperty, () => new TabTitleManager());
        }

        #endregion

        #region TitleWithPathProperty

        public static readonly DependencyProperty TabViewModelProperty = DependencyProperty.RegisterAttached(
            "TabViewModel", typeof(TabViewModel), typeof(TabTitleManager));

        public static void SetTabViewModel(DependencyObject element, TabViewModel value)
        {
            element.SetValue(TabViewModelProperty, value);
        }

        public static TabViewModel GetTabViewModel(DependencyObject element)
        {
            return (TabViewModel) element.GetValue(TabViewModelProperty);
        }

        #endregion

        private readonly Dictionary<DocumentView, TabViewModel> _viewModels = new Dictionary<DocumentView, TabViewModel>();
        private readonly DisplayPathResolver _displayPathResolver = new DisplayPathResolver();
        private readonly DTE _dte;

        public TabTitleManager()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _dte = (DTE) Package.GetGlobalService(typeof(SDTE));

            _displayPathResolver.SolutionRootPath = GetSolutionRootPath();

            _dte.Events.SolutionEvents.Opened += () => 
                _displayPathResolver.SolutionRootPath = GetSolutionRootPath();
            _dte.Events.SolutionEvents.Renamed += _ => 
                _displayPathResolver.SolutionRootPath = GetSolutionRootPath();
            _dte.Events.SolutionEvents.AfterClosing += () => 
                _displayPathResolver.SolutionRootPath = null;
        }

        private string GetSolutionRootPath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var solution = _dte.Solution;
            if (solution == null)
                return null;

            var fullName = solution.FullName;
            if (string.IsNullOrEmpty(fullName))
            {
                // A project has been opened without a solution, so a temporary one is created.
                // Use the project root path instead.
                fullName = solution.Projects.Cast<Project>().FirstOrDefault()?.FullName;
            }

            return string.IsNullOrEmpty(fullName) ? null : Path.GetDirectoryName(fullName);
        }

        public void RegisterDocumentView(DocumentView view)
        {
            if (_viewModels.ContainsKey(view))
                return;

            InstallTabTitlePath(view);
        }

        private void InstallTabTitlePath(DocumentView view)
        {
            if (!(view.Title is WindowFrameTitle title))
                return;

            var bindingExpression = BindingOperations.GetBindingExpression(title, WindowFrameTitle.TitleProperty);
            if (!(bindingExpression?.DataItem is WindowFrame frame))
                return;

            var model = new TabModel(FixFileNameCase(frame.FrameMoniker.Filename));
            frame.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(WindowFrame.AnnotatedTitle))
                    model.FullPath = FixFileNameCase(frame.FrameMoniker.Filename);
            };

            _displayPathResolver.Add(model);

            var viewModel = new TabViewModel(title, view.TabTitleTemplate, model);
            SetTabViewModel(view, viewModel);

            view.DocumentTabTitleTemplate = view.TabTitleTemplate =
                (DataTemplate) Application.Current.FindResource("TabPathTemplate");
            
            _viewModels.Add(view, viewModel);

            frame.FrameDestroyed += (sender, args) =>
            {
                _displayPathResolver.Remove(_viewModels[view].Model);
                _viewModels.Remove(view);
            };
        }

        private static TProperty EnsurePropertyValue<T, TProperty>(T target, DependencyProperty property, Func<TProperty> factory)
            where T : DependencyObject
        {
            var value = (TProperty) target.GetValue(property);
            if (value == null)
            {
                value = factory();
                target.SetValue(property, value);
            }
            return value;
        }

        private static string FixFileNameCase(string fileName)
        {
            try
            {
                var pathParts = fileName.Split(Path.DirectorySeparatorChar);
                var fixedPath = pathParts[0].ToUpperInvariant() + "\\";
                for (var i = 1; i < pathParts.Length; i++)
                    fixedPath = Directory.GetFileSystemEntries(fixedPath, pathParts[i])[0];
                return fixedPath;
            }
            catch
            {
                return fileName;
            }
        }
    }
}