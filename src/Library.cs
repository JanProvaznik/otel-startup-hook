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
public static class StartupHook
{
    /// <summary>
    /// Needed so that we can dispose the tracer on exit.
    /// </summary>
    private static object tracerHolder = null;

    public static string proof = "proof";

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

    private static Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
    {
        Console.WriteLine("Resolving...");
    // Check if this is System.Memory we're trying to resolve
    if (args.Name.StartsWith("System.Memory"))
    {
            return Assembly.Load("System.Memory, Version=4.0.1.2, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51");

            // Parse the original assembly name
            var name = new AssemblyName(args.Name);
        
        // Create new assembly name with desired version
        var newName = new AssemblyName
        {
            Name = name.Name,
            Version = new Version(4, 0, 1, 2),
            CultureInfo = name.CultureInfo,
            
        };

        try
        {
            // Attempt to load the assembly with new version
            return Assembly.Load(newName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load requested version: {ex.Message}");
            // Optionally fall back to whatever version is available
            return Assembly.Load("System.Memory");
        }
    } 

    if (args.Name.StartsWith("System.Runtime.CompilerServices.Unsafe")){
         return Assembly.Load("System.Runtime.CompilerServices.Unsafe, Version=4.0.6.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
         
    }

        Console.WriteLine(args.Name);
        return null;
    }
    public static event ResolveEventHandler AssemblyResolve;

    public static void Initialize()
    {
        AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);

        InitOtel();
#if NETFRAMEWORK
        System.Windows.Forms.MessageBox.Show("Hello2 From: " + Process.GetCurrentProcess().ProcessName);
#endif
        proof = "initialized correctly";
        // we need to flush the messages before the process quits to ensure traces are cleaned up.
        AppDomain.CurrentDomain.ProcessExit += (a, b) => {Console.WriteLine("exiting");(tracerHolder as IDisposable)?.Dispose();}; // this does not work though
                AppDomain.CurrentDomain.DomainUnload  += (a, b) => (tracerHolder as IDisposable)?.Dispose(); 

    }
}