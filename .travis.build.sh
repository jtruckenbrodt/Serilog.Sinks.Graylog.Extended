#!/bin/bash

echo "Starting build script!"
echo

export DOTNET_CLI_TELEMETRY_OUTPUT="1" # stop gathering telemetry data
set -E # Exit with nonzero exit code if anything fails

# execute pre-build

echo "Cleaning possible left-over from last build..."
echo
dotnet clean

echo "Restoring NuGet packages..."
echo
dotnet restore

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
echo "Building project '$buildProjectPath'"
echo

#execute build
dotnet build "$buildProjectPath" --configuration Release --no-restore /property:AssemblyVersion=$assemblyVersion;AssemblyFileVersion=$assemblyVersion;AssemblyInformationalVersion=$assemblyVersion

# publish if it is a release build
if [ "$isRelease" = "true" ]; then
    if [ "$versionSuffix" != "" ]; then
        echo "Packing pre-release: version '$assemblyVersion$versionSuffix'"
		echo
		dotnet pack "$buildProjectPath" --no-build --no-restore --configuration Release /property:PackageVersion=$assemblyVersion --version-suffix ${versionSuffix:1} --include-source --include-symbols
	else
        echo "Packing release: version '$assemblyVersion'"
		echo
		dotnet pack "$buildProjectPath" --no-build --no-restore --configuration Release /property:PackageVersion=$assemblyVersion --include-source --include-symbols
    fi

	echo "Publishing NuGet package"
	echo
	
	set +E # Do not exit with nonzero exit code if anything fails

    dotnet nuget push "$buildProjectPath/bin/Release/netstandard2.0/Serilog.Sinks.Graylog.Extended.$assemblyVersion$versionSuffix.nupkg" --source "https://www.nuget.org/api/v2/package" --api-key "$NUGET_API_KEY"
	
	echo "Publishing MyGet package"
	echo
	
    dotnet nuget push "$buildProjectPath/bin/Release/netstandard2.0/Serilog.Sinks.Graylog.Extended.$assemblyVersion$versionSuffix.nupkg" --source "https://rwe.myget.org/F/rwest-ect-packages/api/v2/package" --api-key "$MYGET_API_KEY"

fi

echo
echo "DONE!!"
