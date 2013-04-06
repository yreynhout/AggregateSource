cls
echo off
echo ------------------------------------------------
echo Script that prepares your working copy for usage
echo ------------------------------------------------
"%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe" /v:n "build\run_me_first.proj" /logger:FileLogger,Microsoft.Build.Engine;LogFile=build\run_me_first.log
pause