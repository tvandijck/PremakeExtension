//------------------------------------------------------------------------------
// <copyright file="RunPremake.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace PremakeExtension
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class RunPremake
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("fca319f8-1356-4986-bff8-a705a3034021");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="RunPremake"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private RunPremake(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            this.package = package;

            var commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new OleMenuCommand(this.ExecutePremake, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        private void MenuItemOnBeforeQueryStatus(object sender, EventArgs eventArgs)
        {
            var menuItem = sender as OleMenuCommand;
            if (menuItem != null)
            {
                menuItem.Visible = !string.IsNullOrEmpty(GetPremakeScript());
            }
        }

        public static RunPremake Instance
        {
            get;
            private set;
        }

        private IServiceProvider ServiceProvider
        {
            get { return this.package; }
        }

        public static void Initialize(Package package)
        {
            Instance = new RunPremake(package);
        }

        public string PremakePath
        {
            get { return ((RunPremakePackage)package).PremakePath; }
        }

        public bool UseGlobalSetting
        {
            get { return ((RunPremakePackage)package).UseGlobalSetting; }
        }

        private void ExecutePremake(object sender, EventArgs e)
        {
            var script = GetPremakeScript();
            if (!string.IsNullOrEmpty(script))
            {
                var premakePath = UseGlobalSetting ? PremakePath : GetPremakeBinary();
                var premakeArguments = string.Format("--file=\"{0}\" {1}", script, GetPremakeArguments());

                var outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
                if (outputWindow != null)
                {
                    var guidGeneral = new Guid("519B77C0-1DCC-4561-AB6C-3181A1B75A6C");
                    IVsOutputWindowPane pane;
                    outputWindow.CreatePane(guidGeneral, "Premake", 1, 0);
                    outputWindow.GetPane(guidGeneral, out pane);
                    pane.Activate();

                    if (string.IsNullOrEmpty(premakePath))
                    {
                        pane.OutputString("Premake Executable Path not setup, go to 'Tools / Option' and browse to the 'Premake' page\n");
                    }
                    else
                    {
                        #if DEBUG
                            pane.OutputString( "[Debug] Premake Binary: " + GetPremakeBinary() + " (" + PremakePath + ")\n" );
                            pane.OutputString( "[Debug] Premake Script: " + script + "\n" );
                            pane.OutputString( "[Debug] Premake Arguments: " + GetPremakeArguments() + "\n" );
                            pane.OutputString( "[Debug] Premake Execute: " + premakePath + " " + premakeArguments + "\n" );
                        #endif

                        var proc = new System.Diagnostics.Process();
                        proc.StartInfo.CreateNoWindow = true;
                        proc.StartInfo.RedirectStandardOutput = true;
                        proc.StartInfo.RedirectStandardError = true;
                        proc.StartInfo.UseShellExecute = false;
                        proc.StartInfo.FileName = premakePath;
                        proc.StartInfo.Arguments = premakeArguments;
                        proc.Start();

                        proc.OutputDataReceived += (o, args) =>
                        {
                            pane.OutputString(args.Data + "\n");
                        };
                        proc.ErrorDataReceived += (o, args) =>
                        {
                            pane.OutputString(args.Data + "\n");
                        };

                        proc.BeginOutputReadLine();
                        proc.BeginErrorReadLine();
                    }
                }
            }
        }

        private string GetPremakeScript()
        {
            var path = GetSolutionProperty("PremakeScript");
            if (string.IsNullOrEmpty(path))
                return null;

            var dte = (DTE2)ServiceProvider.GetService(typeof(DTE));
            var dir = Path.GetDirectoryName(dte.Solution.FullName);
            return string.IsNullOrEmpty(dir) ? path : Path.GetFullPath(Path.Combine(dir, path));
        }

        private string GetPremakeArguments()
        {
            return GetSolutionProperty("PremakeArguments") ?? "vs2015";
        }

        private string GetPremakeBinary()
        {
            return GetSolutionProperty("PremakeBinary") ?? PremakePath;
        }

        private string GetSolutionProperty(string name)
        {
            var dte = (DTE2)ServiceProvider.GetService(typeof(DTE));
            if (dte.Solution == null)
                return null;

            var globals = dte.Solution.Globals;
            if (!globals.VariableExists[name])
                return null;

            return globals[name].ToString();
        }
    }
}
