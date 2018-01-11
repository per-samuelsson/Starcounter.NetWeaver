@ECHO OFF
SETLOCAL EnableDelayedExpansion

SET NugetOutputPath=%STAR_NUGET%
IF "%NugetOutputPath%"=="" (
    SET NugetOutputPath=%~dp0\..\%%STAR_NUGET%%
    IF EXIST %NugetOutputPath%\*.nupkg DEL %NugetOutputPath%\*.nupkg
)

IF "%1"=="" (
    FOR /F "tokens=* USEBACKQ" %%F IN (`git rev-list --count HEAD`) DO (
        SET GitCommitCountSixLeadingZeros=000000%%F
        SET GitCommitCount=!GitCommitCountSixLeadingZeros:~-6!
    )
    FOR /F "tokens=* USEBACKQ" %%F IN (`git rev-parse --abbrev-ref HEAD`) DO (
        SET GitBranchName=%%F
    )
)

SET VersionSuffix=%1
IF "%1"=="" (
    IF NOT "%GitBranchName%"=="develop" (
        ECHO You may not use default version suffix outside of "develop" branch
        EXIT /B 1
    )
    SET VersionSuffix=alpha-%GitCommitCount%
)

ECHO NuGet packing using version suffix %VersionSuffix% to %NugetOutputPath%

PUSHD ..\src
FOR /d %%i IN (*weave*) DO (
    CD %%~i
    REM see https://github.com/NuGet/Home/issues/4337
    dotnet restore /p:VersionSuffix=%VersionSuffix%
    dotnet pack --output %NugetOutputPath% --include-symbols --configuration Debug --version-suffix %VersionSuffix%
    CD ..
)
POPD
