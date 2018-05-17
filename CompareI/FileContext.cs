using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;

namespace CompareI
{
    /// <summary>
    /// Command handler
    /// </summary>.
    [ProvideAutoLoad("f1536ef8-92ec-443c-9ed7-fdadf150da82")]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [Guid(SolutionMenuContextPackage.PackageGuidString)]
    internal sealed class FileContext
    {
        const string SUPPORTED_EXT = ".sql";
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 256;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("eee616b9-f58d-40d6-ac19-4a3ee4334503");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        private readonly SelectedItemUtil m_SelectedItemUtil;
        private readonly SqlClipboard m_SqlClipboard;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileContext"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private FileContext(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            m_SelectedItemUtil = new SelectedItemUtil();
            m_SqlClipboard = new SqlClipboard();

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                OleMenuCommand menuItem = new OleMenuCommand(this.MenuItemCallback, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            if (sender is OleMenuCommand menuCommand)
            {
                var selectedItems = m_SelectedItemUtil.GetFullFilePath(ServiceProvider);

                if (selectedItems.All(path =>
                    string.Equals(Path.GetExtension(path), SUPPORTED_EXT, StringComparison.CurrentCultureIgnoreCase)))
                {
                    menuCommand.Visible = true;
                }
                else
                {
                    menuCommand.Visible = false;
                }
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static FileContext Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private System.IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new FileContext(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var selectedItems = m_SelectedItemUtil.GetFullFilePath(ServiceProvider);
            m_SqlClipboard.CopyContents(selectedItems);

        }
    }
}
