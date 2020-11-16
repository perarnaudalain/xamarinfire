#!/usr/bin/env bash
gem install bundler
bundle config set path ~/.gem
bundle install
echo "alain"
echo $APPCENTER_SOURCE_DIRECTORY
$APPCENTER_SOURCE_DIRECTORY/gradlew tasks
