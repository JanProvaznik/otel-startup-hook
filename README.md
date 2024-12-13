# hooking .net framework app like core Startup Hook
- docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard mcr.microsoft.com/dotnet/aspire-dashboard:9.0
- build everything (set where hook1 is in hook2)
- move binaries of otel-startup-hook2 to binary folder of frameconsole
```
$env:APPDOMAIN_MANAGER_ASM = "otel-startup-hook2, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
$env:APPDOMAIN_MANAGER_TYPE = "MyAppDomainManager"
$env:COMPLUS_Version = "v4.0.30319"
```

- run frameconsole.exe

- yay events visible