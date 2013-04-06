cls
echo off
echo ---------------------------------------------
echo Script that runs the ci build of this project
echo ---------------------------------------------
"%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe" /v:n "ci.proj" /target:TeamCityCodeBetter /logger:FileLogger,Microsoft.Build.Engine;LogFile=ci.log
pause