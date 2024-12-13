using System;
using System.Reflection;
using System.Diagnostics;

#pragma warning disable IDE1006 // Naming Styles

/// <summary>
/// Type that implements the <see href="https://github.com/dotnet/runtime/blob/52e1ad3779e57c35d2416cd10d8ad7d75b2c0c8b/docs/design/features/host-startup-hook.md"> .NET CLR Startup Hook protocol</see>
/// to allow apps that have <see cref="System.Diagnostics.Activity"/> spans created, but no direct dependency on the OpenTelemetry SDK, to still have their traces collected. 
/// </summary>
internal static class Hook
{
    private static readonly string version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

    /// <summary>
    /// Comma separated list of source names to feed to <see cref="TracerProviderBuilder.AddSource(string[])"/> 
    /// </summary>
    public static void Initialize()
    {
        var a = Assembly.Load(AssemblyName.GetAssemblyName(@"yourdevicepath\otel-startup-hook\bin\Debug\net472\publish\otel-startup-hook.dll"));
        // a.CreateInstance("StartupHook")?.GetType().GetMethod("Initialize")?.Invoke(null, null);
        // it's static
        System.Windows.Forms.MessageBox.Show("Hello From: " + Process.GetCurrentProcess().ProcessName);
        a.GetType("StartupHook")?.GetMethod("Initialize")?.Invoke(null, null);
        // we need to flush the messages before the process quits to ensure traces are cleaned up.
        // AppDomain.CurrentDomain.ProcessExit += (a, b) => (tracerHolder as IDisposable)?.Dispose();
    }
}

/// <inheritdoc/>
public sealed class MyAppDomainManager : AppDomainManager
{
    /// <inheritdoc/>
    public override void InitializeNewDomain(AppDomainSetup appDomainInfo)
    {
        base.InitializeNewDomain(appDomainInfo);
        Hook.Initialize();
    }
}