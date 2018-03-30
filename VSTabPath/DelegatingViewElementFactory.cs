using System;
using Microsoft.VisualStudio.PlatformUI.Shell;

namespace VSTabPath
{
    public class DelegatingViewElementFactory : ViewElementFactory
    {
        private readonly ViewElementFactory _innerFactory;

        public DelegatingViewElementFactory(ViewElementFactory innerFactory)
        {
            _innerFactory = innerFactory;
        }

        protected override TabGroup CreateTabGroupCore()
        {
            return _innerFactory?.CreateTabGroup() ?? base.CreateTabGroupCore();
        }

        protected override DockGroup CreateDockGroupCore()
        {
            return _innerFactory?.CreateDocumentGroupContainer() ?? base.CreateDockGroupCore();
        }

        protected override DocumentGroup CreateDocumentGroupCore()
        {
            return _innerFactory?.CreateDocumentGroup() ?? base.CreateDocumentGroupCore();
        }

        protected override DocumentGroupContainer CreateDocumentGroupContainerCore()
        {
            return _innerFactory?.CreateDocumentGroupContainer() ?? base.CreateDocumentGroupContainerCore();
        }

        protected override AutoHideGroup CreateAutoHideGroupCore()
        {
            return _innerFactory?.CreateAutoHideGroup() ?? base.CreateAutoHideGroupCore();
        }

        protected override AutoHideChannel CreateAutoHideChannelCore()
        {
            return _innerFactory?.CreateAutoHideChannel() ?? base.CreateAutoHideChannelCore();
        }

        protected override AutoHideRoot CreateAutoHideRootCore()
        {
            return _innerFactory?.CreateAutoHideRoot() ?? base.CreateAutoHideRootCore();
        }

        protected override DockRoot CreateDockRootCore()
        {
            return _innerFactory?.CreateDockRoot() ?? base.CreateDockRootCore();
        }

        protected override FloatSite CreateFloatSiteCore()
        {
            return _innerFactory?.CreateFloatSite() ?? base.CreateFloatSiteCore();
        }

        protected override MainSite CreateMainSiteCore()
        {
            return _innerFactory?.CreateMainSite() ?? base.CreateMainSiteCore();
        }

        protected override View CreateViewCore(Type viewType)
        {
            return _innerFactory?.CreateView() ?? base.CreateViewCore(viewType);
        }

        protected override ViewBookmark CreateViewBookmarkCore()
        {
            return _innerFactory?.CreateViewBookmark() ?? base.CreateViewBookmarkCore();
        }
    }
}