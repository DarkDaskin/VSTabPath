using System.Collections.Specialized;
using Microsoft.VisualStudio.PlatformUI.Shell;

namespace VSTabPath
{
    public class TabPathDocumentGroup : DocumentGroup
    {
        protected override void OnChildrenChanged(NotifyCollectionChangedEventArgs args)
        {
            base.OnChildrenChanged(args);
        }
    }
}