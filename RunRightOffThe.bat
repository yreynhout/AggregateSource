cls
echo off
echo -------------------------------
echo Script that builds this project
echo    --- RIGHT OFF THE BAT ---
echo -------------------------------
"%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe" /v:n "build/build.proj" /target:NonInteractiveBuild /logger:FileLogger,Microsoft.Build.Engine;LogFile=build/build.log
pause