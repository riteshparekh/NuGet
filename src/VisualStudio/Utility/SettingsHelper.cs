﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet;
using NuGet.VisualStudio.Resources;

namespace NuGet.VisualStudio
{
    /// <summary>
    /// This class is used by functional tests to change NuGet user settings.
    /// </summary>
    public static class SettingsHelper
    {
        /// <summary>
        /// Sets a NuGet user settings property.
        /// </summary>
        /// <param name="property">The name of the settings property to set.</param>
        /// <param name="value">The value of the settings property. 
        /// If null, the settings property will be deleted.</param>
        public static void Set(string property, string value)
        {
            var settings = ServiceLocator.GetInstance<ISettings>();
            var packageRestoreConsent = new PackageRestoreConsent(settings);
            if (string.Equals(property, "PackageRestoreConsentGranted", StringComparison.OrdinalIgnoreCase))
            {
                packageRestoreConsent.IsGrantedInSettings = string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
            }
            else if (string.Equals(property, "PackageRestoreIsAutomatic", StringComparison.OrdinalIgnoreCase))
            {
                packageRestoreConsent.IsAutomatic = string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                if (value == null)
                {
                    settings.DeleteConfigValue(property);
                }
                else
                {
                    settings.SetConfigValue(property, value);
                }
            }
        }
    }
}
