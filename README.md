
1. `dotnet publish -f net472`
2. in otel-startup-hook2/Library.cs change the path to assembly of otel-startup-hook
3. `dotnet publish -f net472 otel-startup-hook2\otel-startup-hook2.csproj`


3. copy publish contents of otel-startup-hook2 to MSBuild Fx bootstrap
4. msbuild\artifacts\bin\bootstrap\net472\MSBuild\Current\Bin\MSBuild.exe

Unhandled Exception: System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation. ---> System.IO.FileLoadException: Could not load file or assembly 'System.Memory, Version=4.0.1.1, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51' or one of its dependencies. The located assembly's manifest definition does not match the assembly reference. (Exception from HRESULT: 0x80131040)
   at System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointerInternal(IntPtr ptr, Type t)
   at System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer(IntPtr ptr, Type t)
   at Grpc.Core.Internal.UnmanagedLibrary.GetNativeMethodDelegate[T](String methodName)
   at Grpc.Core.Internal.NativeMethods.GetMethodDelegate[T](UnmanagedLibrary library)
   at Grpc.Core.Internal.NativeMethods..ctor(UnmanagedLibrary library)
   at Grpc.Core.Internal.NativeExtension.LoadNativeMethodsUsingExplicitLoad()
   at Grpc.Core.Internal.NativeExtension.LoadNativeMethods()
   at Grpc.Core.Internal.NativeExtension..ctor()
   at Grpc.Core.Internal.NativeExtension.Get()
   at Grpc.Core.GrpcEnvironment.GrpcNativeInit()
   at Grpc.Core.GrpcEnvironment..ctor()
   at Grpc.Core.GrpcEnvironment.AddRef()
   at Grpc.Core.Channel..ctor(String target, ChannelCredentials credentials, IEnumerable`1 options)
   at OpenTelemetry.Exporter.OtlpExporterOptionsExtensions.CreateChannel(OtlpExporterOptions options)
   at OpenTelemetry.Exporter.OpenTelemetryProtocol.Implementation.ExportClient.OtlpGrpcTraceExportClient..ctor(OtlpExporterOptions options, TraceServiceClient traceServiceClient)
   at OpenTelemetry.Exporter.OtlpExporterOptionsExtensions.GetTraceExportClient(OtlpExporterOptions options)
   at OpenTelemetry.Exporter.OtlpExporterOptionsExtensions.GetTraceExportTransmissionHandler(OtlpExporterOptions options, ExperimentalOptions experimentalOptions)
   at OpenTelemetry.Trace.OtlpTraceExporterHelperExtensions.BuildOtlpExporterProcessor(IServiceProvider serviceProvider, OtlpExporterOptions exporterOptions, SdkLimitOptions sdkLimitOptions, ExperimentalOptions experimentalOptions, ExportProcessorType exportProcessorType, BatchExportProcessorOptions`1 batchExportProcessorOptions, Boolean skipUseOtlpExporterRegistrationCheck, Func`2 configureExporterInstance)
   at OpenTelemetry.Trace.OtlpTraceExporterHelperExtensions.BuildOtlpExporterProcessor(IServiceProvider serviceProvider, OtlpExporterOptions exporterOptions, SdkLimitOptions sdkLimitOptions, ExperimentalOptions experimentalOptions, Func`2 configureExporterInstance)
   at OpenTelemetry.Trace.OtlpTraceExporterHelperExtensions.<>c__DisplayClass2_0.<AddOtlpExporter>b__1(IServiceProvider sp)
   at OpenTelemetry.Trace.TracerProviderBuilderExtensions.<>c__DisplayClass8_0.<AddProcessor>b__0(IServiceProvider sp, TracerProviderBuilder builder)
   at OpenTelemetry.Trace.OpenTelemetryDependencyInjectionTracingServiceCollectionExtensions.ConfigureTracerProviderBuilderCallbackWrapper.ConfigureBuilder(IServiceProvider serviceProvider, TracerProviderBuilder tracerProviderBuilder)
   at OpenTelemetry.Trace.TracerProviderSdk..ctor(IServiceProvider serviceProvider, Boolean ownsServiceProvider)
   at OpenTelemetry.Trace.TracerProviderBuilderBase.Build()
   at OpenTelemetry.Trace.TracerProviderBuilderExtensions.Build(TracerProviderBuilder tracerProviderBuilder)
   at StartupHook.InitOtel()
   at StartupHook.Initialize()
   --- End of inner exception stack trace ---
   at System.RuntimeMethodHandle.InvokeMethod(Object target, Object[] arguments, Signature sig, Boolean constructor)
   at System.Reflection.RuntimeMethodInfo.UnsafeInvokeInternal(Object obj, Object[] parameters, Object[] arguments)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
   at System.Reflection.MethodBase.Invoke(Object obj, Object[] parameters)
   at Hook.Initialize()
   at MyAppDomainManager.InitializeNewDomain(AppDomainSetup appDomainInfo)
   at System.AppDomain.CreateAppDomainManager()