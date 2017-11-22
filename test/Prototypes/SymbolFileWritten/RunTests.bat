@echo off

SETLOCAL ENABLEEXTENSIONS
SET me=%~n0
SET mydir=%~dp0

bin\debug\net461\SymbolFileWritten.exe
IF %ERRORLEVEL% NEQ 0 (GOTO error)

dotnet bin\debug\netcoreapp2.0\SymbolFileWritten.dll
IF %ERRORLEVEL% NEQ 0 (GOTO error)

:success
ECHO %me%: Test success! 
EXIT /b 0

:error
ECHO %me%: Error %ERRORLEVEL%
EXIT /b %ERRORLEVEL%