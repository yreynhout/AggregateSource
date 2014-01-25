cls
echo off
echo ------------------------------------------------
echo Script that prepares your working copy for usage
echo ------------------------------------------------
"%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe" /v:n "winbuild\run_me_first.proj" /logger:FileLogger,Microsoft.Build.Engine;LogFile=winbuild\run_me_first.log
pause