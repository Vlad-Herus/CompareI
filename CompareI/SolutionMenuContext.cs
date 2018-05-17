using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;

namespace CompareI
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class SolutionMenuContext
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("8b140887-7db5-442a-8ad3-7f83b95914c0");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        private readonly SelectedItemUtil m_SelectedItemUtil;
        private readonly RegistryMap m_RegistryMap;
        private readonly BeyondCompareRunner m_BeyondCompareRunner;


        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionMenuContext"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private SolutionMenuContext(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;
            m_SelectedItemUtil = new SelectedItemUtil();
            m_RegistryMap = new RegistryMap();
            m_BeyondCompareRunner = new BeyondCompareRunner();

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
                DTE dte = (DTE)ServiceProvider.GetService(typeof(DTE));
                string solutionPath = dte.Solution.FullName;

                var presentInMap = m_RegistryMap.SolutionMap.Any(item =>
                item.Key.Equals(solutionPath, StringComparison.CurrentCultureIgnoreCase));

                if (presentInMap)
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
        public static SolutionMenuContext Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
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
            Instance = new SolutionMenuContext(package);
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
            //TODO: Put your code here

            DTE dte = (DTE)ServiceProvider.GetService(typeof(DTE));
            string solutionPath = dte.Solution.FullName;
            var matches = m_RegistryMap.SolutionMap.Where(item =>
                item.Key.Equals(solutionPath, StringComparison.CurrentCultureIgnoreCase));

            if (matches.Count() == 1)
            {
                m_BeyondCompareRunner.LaunchNamedComparison(matches.First().Value);
            }

        }
    }
}
