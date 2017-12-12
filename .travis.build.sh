#!/bin/bash

set -e # Exit with nonzero exit code if anything fails

# Only continue for 'develop' or 'release/*' branches
regexReleaseBranch="^release/([0-9]+\.[0-9]+\.[0-9]+(-[0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*)*)$"

if [[ $TRAVIS_BRANCH =~ $regexReleaseBranch ]];
then
    version=$1
    echo "Detected release buld: version '$version'"
else
    echo "Detected non-release build"
fi
