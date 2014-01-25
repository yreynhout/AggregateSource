cls
echo off
echo -------------------------------
echo Script that builds this project
echo Please make sure you've run the
echo RunMeFirst.bat, well, first :-) 
echo -------------------------------
"%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe" /v:n "build.proj" /target:Build /logger:FileLogger,Microsoft.Build.Engine;LogFile=build.log
pause