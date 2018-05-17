using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace CompareI
{
    class SelectedItemUtil
    {

        public IEnumerable<string> GetFullFilePath(IServiceProvider serviceProvider)
        {
            var selectionMonitor = serviceProvider.GetService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
            if (selectionMonitor == null)
            {
                Debug.Fail("Failed to get SVsShellMonitorSelection service.");

                return Enumerable.Empty<string>();
            }

            int hr = selectionMonitor.GetCurrentSelection(out IntPtr hierarchyPtr, out uint itemid, out IVsMultiItemSelect multiSelect, out IntPtr containerPtr);

            if (hierarchyPtr == IntPtr.Zero)
            {
                return Enumerable.Empty<string>();
            }

            if (IntPtr.Zero != containerPtr)
            {
                Marshal.Release(containerPtr);
                containerPtr = IntPtr.Zero;
            }
            Debug.Assert(hr == VSConstants.S_OK, "GetCurrentSelection failed.");

            if (itemid == (uint)VSConstants.VSITEMID.Selection)
            {
                multiSelect.GetSelectionInfo(out uint count, out int somethingElse);
                if (count > 0)
                {
                    VSITEMSELECTION[] selectedItems = new VSITEMSELECTION[count];
                    var sel = multiSelect.GetSelectedItems((uint)__VSGSIFLAGS.GSI_fOmitHierPtrs, count, selectedItems);

                    //multiple selection
                    return selectedItems.Select(info => GetItemPath(hierarchyPtr, info.itemid));
                }
                else
                {
                    return Enumerable.Empty<string>();
                }
            }
            else
            {
                return new string[] { GetItemPath(hierarchyPtr, itemid) };
            }
        }

        string GetItemPath(IntPtr hirarchryPtr, uint temId)
        {
            IVsProject project = Marshal.GetUniqueObjectForIUnknown(hirarchryPtr) as IVsProject;
            string itemFullPath = null;
            project?.GetMkDocument(temId, out itemFullPath);

            return itemFullPath;
        }
    }
}
