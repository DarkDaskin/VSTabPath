using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Microsoft.VisualStudio.Platform.WindowManagement;
using Microsoft.VisualStudio.PlatformUI.Shell;

namespace VSTabPath
{
    public class TabTitleManager
    {
        #region TabTitleManagerProperty

        public static readonly DependencyProperty TabTitleManagerProperty = 
            DependencyProperty.RegisterAttached(nameof(TabTitleManager), typeof(TabTitleManager), typeof(ViewGroup));

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

        private readonly Dictionary<DocumentView, WindowFrameTitleWithPath> _views = new Dictionary<DocumentView, WindowFrameTitleWithPath>();

        public void RegisterDocumentView(DocumentView view)
        {
            if (_views.ContainsKey(view))
                return;

            InstallTabTitleProxy(view);

            UpdateTabTitles();
        }

        private void InstallTabTitleProxy(DocumentView view)
        {
            if (!(view.Title is WindowFrameTitle title))
                return;

            var bindingExpression = BindingOperations.GetBindingExpression(title, WindowFrameTitle.TitleProperty);
            if (!(bindingExpression?.DataItem is WindowFrame frame))
                return;

            var titleWithPath = new WindowFrameTitleWithPath(title, view.TabTitleTemplate, frame, this);
            view.Title = titleWithPath;
            view.DocumentTabTitleTemplate = view.TabTitleTemplate =
                (DataTemplate) Application.Current.FindResource("TabPathTemplate");
            
            _views.Add(view, titleWithPath);

            frame.FrameDestroyed += (sender, args) =>
            {
                _views.Remove(view);
                UpdateTabTitles();
            };
        }
        
        public void UpdateTabTitles()
        {
            var modelsByTitle = _views.ToLookup(kv => kv.Value.OriginalTitle, kv => kv.Value);

            var modelsWithDuplicateTitles = modelsByTitle
                .Where(g => g.Count() > 1)
                .SelectMany(g => g);
            foreach (var model in modelsWithDuplicateTitles)
                model.IsPathVisible = true;

            var modelsWithUniqueTitles = modelsByTitle
                .Where(g => g.Count() == 1)
                .SelectMany(g => g);
            foreach (var model in modelsWithUniqueTitles)
                model.IsPathVisible = false;
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
    }
}