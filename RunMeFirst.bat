cls
echo off
echo ----------------------------------------------------------------------
echo Script that downloads and unzips GetEventStore in a specific directory
echo so that the solution may compile and integration tests may run
echo ----------------------------------------------------------------------

".nuget/NuGet.exe" install lib/packages.config -o lib
"%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe" /v:n "lib/GEventStoreIntegration.proj" /logger:FileLogger,Microsoft.Build.Engine;LogFile=lib\GEventStoreIntegration.log
pause