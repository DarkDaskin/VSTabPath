using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualStudio.Platform.WindowManagement;
using Microsoft.VisualStudio.PlatformUI.Shell;
using Microsoft.VisualStudio.PlatformUI.Shell.Controls;

namespace VSTabPath
{
    public class TabPathViewElementFactory : DelegatingViewElementFactory
    {
        public TabPathViewElementFactory(ViewElementFactory innerFactory) : base(innerFactory)
        {
        }

        protected override View CreateViewCore(Type viewType)
        {
            var view = base.CreateViewCore(viewType);

            if (view is DocumentView documentView)
                SetupView(documentView);

            return view;
        }

        private static void SetupView(DocumentView view)
        {
            if (!view.IsVisible)
            {
                view.Shown += OnDocumentViewShown;
                return;
            }

            Debug.Assert(view.Parent != null, "view.Parent != null");
            var titleManager = TabTitleManager.EnsureTabTitleManager(view.Parent);
            titleManager.RegisterDocumentView(view);
        }

        private static void OnDocumentViewShown(object sender, EventArgs e)
        {
            var view = (DocumentView) sender;

            view.Shown -= OnDocumentViewShown;

            SetupView(view);
        }

        public static void SetupExistingDocuments()
        {
            var documentViews = FindVisualDescendants<DocumentTabItem>(Application.Current.MainWindow)
                .Select(e => e.View)
                .OfType<DocumentView>();
            foreach (var view in documentViews)
                SetupView(view);
        }

        private static IEnumerable<T> FindVisualDescendants<T>(DependencyObject obj)
            where T : DependencyObject
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(obj);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);

                if (child is T tChild)
                    yield return tChild;

                foreach (var descendant in FindVisualDescendants<T>(child))
                    yield return descendant;
            }
        }
    }
}