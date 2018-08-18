using System;
using System.ComponentModel.Design;
using System.IO;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

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
        public static readonly Guid CommandSet = new Guid("153eb81e-b6f2-43ec-8c00-f48d3f957d30");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="RunPremake"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private RunPremake(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private async void MenuItemOnBeforeQueryStatus(object sender, EventArgs eventArgs)
        {
            var menuItem = sender as OleMenuCommand;
            if (menuItem != null)
            {
                menuItem.Visible = !string.IsNullOrEmpty(await GetPremakeScriptAsync());
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static RunPremake Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get { return this.package; }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in RunPremake's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new RunPremake(package, commandService);
        }

        public string PremakePath
        {
            get { return ((RunPremakePackage)package).PremakePath; }
        }

        public bool UseGlobalSetting
        {
            get { return ((RunPremakePackage)package).UseGlobalSetting; }
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private async void Execute(object sender, EventArgs e)
        {
            var script = await GetPremakeScriptAsync();
            if (!string.IsNullOrEmpty(script))
            {
                var premakePath = UseGlobalSetting ? PremakePath : await GetPremakeBinaryAsync();
                var premakeArguments = string.Format("--file=\"{0}\" {1}", script, await GetPremakeArgumentsAsync());
                var premakeWorkingDir = Path.GetDirectoryName(script);

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
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
                        var proc = new System.Diagnostics.Process();
                        proc.StartInfo.CreateNoWindow = true;
                        proc.StartInfo.RedirectStandardOutput = true;
                        proc.StartInfo.RedirectStandardError = true;
                        proc.StartInfo.UseShellExecute = false;
                        proc.StartInfo.FileName = premakePath;
                        proc.StartInfo.WorkingDirectory = premakeWorkingDir;
                        proc.StartInfo.Arguments = premakeArguments;
                        proc.Start();

                        proc.OutputDataReceived += (o, args) =>
                        {
                            ThreadHelper.ThrowIfNotOnUIThread();
                            pane.OutputString(args.Data + "\n");
                        };
                        proc.ErrorDataReceived += (o, args) =>
                        {
                            ThreadHelper.ThrowIfNotOnUIThread();
                            pane.OutputString(args.Data + "\n");
                        };

                        proc.BeginOutputReadLine();
                        proc.BeginErrorReadLine();
                    }
                }
            }
        }

        private async Task<string> GetPremakeScriptAsync()
        {
            var path = await GetSolutionPropertyAsync("PremakeScript");
            if (string.IsNullOrEmpty(path))
                return null;

            var dte = await ServiceProvider.GetServiceAsync(typeof(DTE)) as DTE2;
            if (dte == null)
                return null;

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            var dir = Path.GetDirectoryName(dte.Solution.FullName);
            return string.IsNullOrEmpty(dir) ? path : Path.GetFullPath(Path.Combine(dir, path));
        }

        private async Task<string> GetPremakeArgumentsAsync()
        {
            return await GetSolutionPropertyAsync("PremakeArguments") ?? "vs2017";
        }

        private async Task<string> GetPremakeBinaryAsync()
        {
            return await GetSolutionPropertyAsync("PremakeBinary") ?? PremakePath;
        }

        private async Task<string> GetSolutionPropertyAsync(string name)
        {
            var dte = await ServiceProvider.GetServiceAsync(typeof(DTE)) as DTE2;
            if (dte == null || dte.Solution == null)
                return null;

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            var globals = dte.Solution.Globals;
            if (!globals.VariableExists[name])
                return null;

            return (string)globals[name];
        }
    }
}
