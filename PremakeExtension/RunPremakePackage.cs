//------------------------------------------------------------------------------
// <copyright file="RunPremakePackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace PremakeExtension
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(RunPremakePackage.PackageGuidString)]
    [ProvideOptionPage(typeof(PremakeSettings), "Premake", "General", 0, 0, true)]
    [ProvideProfile(typeof(PremakeSettings), "Premake", "General", 106, 107, isToolsOptionPage: true, DescriptionResourceID = 108)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class RunPremakePackage : Package
    {
        /// <summary>
        /// RunPremakePackage GUID string.
        /// </summary>
        public const string PackageGuidString = "db8cddeb-01c5-4a44-ab0d-a71142738aaa";

        /// <summary>
        /// Initializes a new instance of the <see cref="RunPremake"/> class.
        /// </summary>
        public RunPremakePackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            RunPremake.Initialize(this);
            base.Initialize();
        }

        #endregion
        public string PremakePath
        {
            get
            {
                var page = (PremakeSettings)GetDialogPage(typeof(PremakeSettings));
                return page.PremakePath;
            }
        }

        public bool UseGlobalSetting
        {
            get
            {
                var page = (PremakeSettings)GetDialogPage(typeof(PremakeSettings));
                return page.UseGlobalSetting;
            }
        }
    }
}
