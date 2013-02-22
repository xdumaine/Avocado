namespace MetroMVVM.Helpers
{
    using System;
    using System.Reflection;

    public class VersionHelper
    {
        public static Version ParseVersionNumber(Assembly assembly)
        {
            var assemblyName = new AssemblyName(assembly.FullName);
            return assemblyName.Version;
        }

        public static DateTime RetrieveLinkerTimestamp(Type appType)
        {
            // See: http://social.msdn.microsoft.com/Forums/en-BZ/winappswithcsharp/thread/012db5c6-a811-48a9-8d27-9a21878263b4
            //Assembly assembly = appType.GetTypeInfo().Assembly;
            //var version = assembly.GetCustomAttribute(typeof(AssemblyFileVersionAttribute)) as AssemblyFileVersionAttribute;

            Assembly assembly = appType.GetTypeInfo().Assembly;
            var assemblyName = new AssemblyName(assembly.FullName);

            DateTime buildDate = new DateTime(2000, 1, 1).AddDays(assemblyName.Version.Build).AddSeconds(assemblyName.Version.Revision * 2);

            return TimeZoneInfo.Local.IsDaylightSavingTime(buildDate) ? buildDate.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours) : buildDate.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours - 1);
        }
    }
}