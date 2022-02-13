using System;

namespace VSTabPath.Interop.Contracts
{
    public interface IDteInterop
    {
        string GetSolutionRootPath();

        event Action SolutionOpened;
        event Action<string> SolutionRenamed;
        event Action SolutionClosed;
    }
}