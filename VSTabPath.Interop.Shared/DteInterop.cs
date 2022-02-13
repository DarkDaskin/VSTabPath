using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.IO;
using System.Linq;
using VSTabPath.Interop.Contracts;

namespace VSTabPath.Interop
{
    // ReSharper disable once UnusedMember.Global
    public class DteInterop : IDteInterop
    {
        private readonly DTE2 _dte;

        public DteInterop()
        {
            _dte = (DTE2)Package.GetGlobalService(typeof(SDTE));
        }

        public string GetSolutionRootPath()
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

        public event Action SolutionOpened
        {
            add => _dte.Events.SolutionEvents.Opened += new _dispSolutionEvents_OpenedEventHandler(value);
            remove => _dte.Events.SolutionEvents.Opened -= new _dispSolutionEvents_OpenedEventHandler(value);
        }

        public event Action<string> SolutionRenamed
        {
            add => _dte.Events.SolutionEvents.Renamed += new _dispSolutionEvents_RenamedEventHandler(value);
            remove => _dte.Events.SolutionEvents.Renamed -= new _dispSolutionEvents_RenamedEventHandler(value);
        }

        public event Action SolutionClosed
        {
            add => _dte.Events.SolutionEvents.AfterClosing += new _dispSolutionEvents_AfterClosingEventHandler(value);
            remove => _dte.Events.SolutionEvents.AfterClosing -= new _dispSolutionEvents_AfterClosingEventHandler(value);
        }
    }
}