@ECHO OFF
setlocal

set PROJECT_NAME=mssqldump

if "%APPVEYOR_BUILD_VERSION%" == "" echo No APPVEYOR_BUILD_VERSION in env & goto :END

pushd ..
mkdir Release 2>NUL
pushd Release

xcopy ..\choco .\choco /s /i
popd & popd

pushd ..\src
msbuild %PROJECT_NAME%\%PROJECT_NAME%.csproj /p:Configuration=Release /p:OutputPath=%~dp0..\Release\choco\tools
popd

pushd ..\Release\choco
choco pack --version %APPVEYOR_BUILD_VERSION%
if errorlevel 1 echo choco pack failed.
popd

:END
