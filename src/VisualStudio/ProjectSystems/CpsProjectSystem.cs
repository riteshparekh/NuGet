﻿using System;
using System.Runtime.CompilerServices;
using EnvDTE;

namespace NuGet.VisualStudio
{
    public abstract class CpsProjectSystem : VsProjectSystem
    {
        protected CpsProjectSystem(Project project, IFileSystemProvider fileSystemProvider) :
            base(project, fileSystemProvider)
        {
        }

        public override bool IsBindingRedirectSupported
        {
            get
            {
                return false;
            }
        }

        protected override void AddGacReference(string name)
        {
            // Native & JS projects don't know about GAC
        }

        public override void AddImport(string targetPath, ProjectImportLocation location)
        {
            if (VsVersionHelper.IsVisualStudio2010)
            {
                base.AddImport(targetPath, location);
            }
            else
            {
                // For VS 2012 or above, the operation has to be done inside the Writer lock
                if (String.IsNullOrEmpty(targetPath))
                {
                    throw new ArgumentNullException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "targetPath");
                }

                string relativeTargetPath = PathUtility.GetRelativePath(PathUtility.EnsureTrailingSlash(Root), targetPath);
                if (VsVersionHelper.IsVisualStudio2012)
                {
                    Project.DoWorkInWriterLock(buildProject => buildProject.AddImportStatement(relativeTargetPath, location));
                    Project.Save();
                }
                else
                {
                    AddImportStatementForVS2013(location, relativeTargetPath);
                }
            }
        }

        // IMPORTANT: The NoInlining is required to prevent CLR from loading VisualStudio12.dll assembly while running 
        // in VS2010 and VS2012
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddImportStatementForVS2013(ProjectImportLocation location, string relativeTargetPath)
        {
            NuGet.VisualStudio12.ProjectHelper.DoWorkInWriterLock(
                Project,
                Project.ToVsHierarchy(),
                buildProject => buildProject.AddImportStatement(relativeTargetPath, location));
        }

        public override void RemoveImport(string targetPath)
        {
            if (VsVersionHelper.IsVisualStudio2010)
            {
                base.RemoveImport(targetPath);
            }
            else
            {
                if (String.IsNullOrEmpty(targetPath))
                {
                    throw new ArgumentNullException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "targetPath");
                }

                // For VS 2012 or above, the operation has to be done inside the Writer lock
                string relativeTargetPath = PathUtility.GetRelativePath(PathUtility.EnsureTrailingSlash(Root), targetPath);
                if (VsVersionHelper.IsVisualStudio2012)
                {
                    Project.DoWorkInWriterLock(buildProject => buildProject.RemoveImportStatement(relativeTargetPath));
                    Project.Save();
                }
                else
                {
                    RemoveImportStatementForVS2013(relativeTargetPath);
                }
            }
        }

        // IMPORTANT: The NoInlining is required to prevent CLR from loading VisualStudio12.dll assembly while running 
        // in VS2010 and VS2012
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void RemoveImportStatementForVS2013(string relativeTargetPath)
        {
            NuGet.VisualStudio12.ProjectHelper.DoWorkInWriterLock(
                Project,
                Project.ToVsHierarchy(),
                buildProject => buildProject.RemoveImportStatement(relativeTargetPath));
        }
    }
}