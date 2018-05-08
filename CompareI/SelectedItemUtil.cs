using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Test3
{
    class SelectedItemUtil
    {

        public static string GetFullFilePath(IServiceProvider serviceProvider)
        {
            var selectionMonitor = serviceProvider.GetService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
            if (selectionMonitor == null)
            {
                Debug.Fail("Failed to get SVsShellMonitorSelection service.");

                return null;
            }

            int hr = selectionMonitor.GetCurrentSelection(out IntPtr hierarchyPtr, out uint itemid, out IVsMultiItemSelect multiSelect, out IntPtr containerPtr);
            if (IntPtr.Zero != containerPtr)
            {
                Marshal.Release(containerPtr);
                containerPtr = IntPtr.Zero;
            }
            Debug.Assert(hr == VSConstants.S_OK, "GetCurrentSelection failed.");

            if (itemid == (uint)VSConstants.VSITEMID.Selection)
            {
                //multiple selection
                return null;
            }

            if (hierarchyPtr != IntPtr.Zero)
            {
                IVsProject project = Marshal.GetUniqueObjectForIUnknown(hierarchyPtr) as IVsProject;
                string itemFullPath = null;
                project?.GetMkDocument(itemid, out itemFullPath);

                return itemFullPath;

            }


            return null;
        }
    }
}
