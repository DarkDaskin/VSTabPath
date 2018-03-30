using System;
using System.Diagnostics;
using Microsoft.VisualStudio.Platform.WindowManagement;
using Microsoft.VisualStudio.PlatformUI.Shell;

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

            if (view is DocumentView)
                view.Shown += OnDocumentViewShown;

            return view;
        }

        private static void OnDocumentViewShown(object sender, EventArgs e)
        {
            var view = (DocumentView) sender;
            Debug.Assert(view.Parent != null, "view.Parent != null");
            var titleManager = TabTitleManager.EnsureTabTitleManager(view.Parent);
            titleManager.RegisterDocumentView(view);
        }
    }
}