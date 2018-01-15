@ECHO OFF

IF "%MYGET_API_KEY%"=="" (
    ECHO Push failed. The API key for myget.org is not given.
    EXIT /B 1
)

SET NugetOutputPath=%STAR_NUGET%
IF "%NugetOutputPath%"=="" (
    SET NugetOutputPath=%~dp0..\%%STAR_NUGET%%
)

PUSHD %NugetOutputPath%
FOR /f %%l IN ('dir /b Starcounter.Weaver.Runtime.*.nupkg') DO (
    dotnet nuget push %%l --api-key %MYGET_API_KEY% --source https://www.myget.org/F/starcounter/api/v2/package
    DEL %%l
)
POPD
