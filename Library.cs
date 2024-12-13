using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using System;
using System.Reflection;
using System.Diagnostics;

#pragma warning disable IDE1006 // Naming Styles

/// <summary>
/// Type that implements the <see href="https://github.com/dotnet/runtime/blob/52e1ad3779e57c35d2416cd10d8ad7d75b2c0c8b/docs/design/features/host-startup-hook.md"> .NET CLR Startup Hook protocol</see>
/// to allow apps that have <see cref="System.Diagnostics.Activity"/> spans created, but no direct dependency on the OpenTelemetry SDK, to still have their traces collected. 
/// </summary>
internal static class StartupHook
{
    /// <summary>
    /// Needed so that we can dispose the tracer on exit.
    /// </summary>
    private static object tracerHolder = null;

    private static readonly string version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

    /// <summary>
    /// Comma separated list of source names to feed to <see cref="TracerProviderBuilder.AddSource(string[])"/> 
    /// </summary>
    private static readonly string sourceNames = "Microsoft.Build";

    private static void InitOtel()
    {
        var resource = ResourceBuilder
                       .CreateDefault()
                .AddService(".NET CLR OpenTelemetry Hook", version);

        TracerProviderBuilder tracer =
            Sdk
               .CreateTracerProviderBuilder()
               .SetResourceBuilder(resource);

        if (sourceNames is string sources)
        {
            var sourceList = sources.Split(',');
            tracer = tracer.AddSource(sourceList);
        }


        tracerHolder =
            tracer
               .AddOtlpExporter()
               .Build();
    }

    public static void Initialize()
    {
        InitOtel();
#if NETFRAMEWORK
        System.Windows.Forms.MessageBox.Show("Hello2 From: " + Process.GetCurrentProcess().ProcessName);
#endif
        // we need to flush the messages before the process quits to ensure traces are cleaned up.
        AppDomain.CurrentDomain.ProcessExit += (a, b) => (tracerHolder as IDisposable)?.Dispose();
    }
}