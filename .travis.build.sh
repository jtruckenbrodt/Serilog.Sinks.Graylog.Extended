#!/bin/bash

set -e # Exit with nonzero exit code if anything fails

# execute pre-build
dotnet restore
dotnet clean

# Only continue for 'develop' or 'release/*' branches
regexReleaseBranch="^[rR]elease/([0-9]+\.[0-9]+\.[0-9]+(-[0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*)*)$"

if [[ $TRAVIS_BRANCH =~ $regexReleaseBranch ]];
then
	assemblyVersion=${BASH_REMATCH[1]}
    versionSuffix=${BASH_REMATCH[2]}
	let "versionLength=${#assemblyVersion} - ${#versionSuffix}"
    assemblyVersion=${assemblyVersion:0:$versionLength}
	isRelease="true"
    echo "Detected release build: version '$assemblyVersion' with version suffix '$versionSuffix'"
else
	assemblyVersion="0.0.0"
	isRelease="false"
    echo "Detected non-release build '$TRAVIS_BRANCH'"
fi

buildProjectPath="./Serilog.Sinks.Graylog.Extended"

#execute build
dotnet build "$buildProjectPath" --configuration Release /p:AssemblyVersion=$assemblyVersion /p:AssemblyFileVersion=$assemblyVersion /p:AssemblyInformationalVersion=$assemblyVersion

# publish if it is a release build
if [ "$isRelease" = "true" ]; then
    if [ "$versionSuffix" != "" ]; then
        echo "Publishing pre-release: version '$assemblyVersion$versionSuffix'"
		dotnet pack "$buildProjectPath" --no-build --configuration Release /p:PackageVersion=$assemblyVersion --version-suffix ${versionSuffix:1} --include-source --include-symbols
	else
        echo "Publishing release: version '$assemblyVersion'"
		dotnet pack "$buildProjectPath" --no-build --configuration Release /p:PackageVersion=$assemblyVersion --include-source --include-symbols
    fi
    dotnet nuget push "$buildProjectPath/bin/Release/Serilog.Sinks.Graylog.Extended.$assemblyVersion$versionSuffix.nupkg" --source "https://www.nuget.org/api/v2/package" --api-key "$NUGET_API_KEY"
fi
