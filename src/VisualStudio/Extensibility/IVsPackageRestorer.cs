using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EnvDTE;

namespace NuGet.VisualStudio
{
    [ComImport]
    [Guid("CAAEA598-1393-48E7-9617-1C5A62E38ABD")]
    public interface IVsPackageRestorer
    {
        bool IsUserConsentGranted();

        void RestorePackages(Project project);
    }
}
