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

        private readonly Dictionary<DocumentView, TabTitleProxy> _views = new Dictionary<DocumentView, TabTitleProxy>();

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

            var titleProxy = new TabTitleProxy(frame, this);
            BindingOperations.SetBinding(title, WindowFrameTitle.TitleProperty,
                new Binding(nameof(TabTitleProxy.Title)) {Source = titleProxy});

            _views.Add(view, titleProxy);

            frame.FrameDestroyed += (sender, args) =>
            {
                _views.Remove(view);
                UpdateTabTitles();
            };
        }

        public void UpdateTabTitles()
        {
            var proxiesByTitle = _views.ToLookup(kv => kv.Value.OriginalTitle, kv => kv.Value);

            var proxiesWithDuplicateTitles = proxiesByTitle
                .Where(g => g.Count() > 1)
                .SelectMany(g => g);
            foreach (var titleProxy in proxiesWithDuplicateTitles)
                titleProxy.IsPathVisible = true;

            var proxiesWithUniqueTitles = proxiesByTitle
                .Where(g => g.Count() == 1)
                .SelectMany(g => g);
            foreach (var titleProxy in proxiesWithUniqueTitles)
                titleProxy.IsPathVisible = false;
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