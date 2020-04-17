if ( !$env:APPVEYOR_PULL_REQUEST_NUMBER -and ($env:APPVEYOR_REPO_BRANCH -eq "develop") ) {
  $env:CI_DEPLOY_GITHUB = $true;

	# if ( !(Test-Path -Path "$env:APPVEYOR_BUILD_FOLDER/deps/$ENV:APPVEYOR_PROJECT_SLUG" ) ) {
	#		Write-Host "Create $env:APPVEYOR_BUILD_FOLDER/deps/$ENV:APPVEYOR_PROJECT_SLUG";
	#		New-Item -Path "$env:APPVEYOR_BUILD_FOLDER/deps/$ENV:APPVEYOR_PROJECT_SLUG" -ItemType Directory | Out-Null;
	# }
	# Write-Host "Copy * to $env:APPVEYOR_BUILD_FOLDER/deps/$ENV:APPVEYOR_PROJECT_SLUG";
	# Copy-Item -Path $env:APPVEYOR_BUILD_FOLDER/ChatbotScriptUpdater/bin/debug/* -Include "*.exe", "*.dll" -Destination "$env:APPVEYOR_BUILD_FOLDER/deps/$ENV:APPVEYOR_PROJECT_SLUG/" | Write-Host;

	# Get-ChildItem -Path $env:APPVEYOR_BUILD_FOLDER/deps/$ENV:APPVEYOR_PROJECT_SLUG/ | Write-Host;


	# Compress-Archive -Path $env:APPVEYOR_BUILD_FOLDER/deps/$ENV:APPVEYOR_PROJECT_SLUG/* -CompressionLevel Optimal -DestinationPath "$env:APPVEYOR_BUILD_FOLDER/deps/$($ENV:APPVEYOR_PROJECT_SLUG)-v$($ENV:APPVEYOR_BUILD_VERSION).zip";

	# Get-ChildItem -Path $env:APPVEYOR_BUILD_FOLDER/deps/ | Write-Host;

	# Remove-Item -Path $env:APPVEYOR_BUILD_FOLDER/deps/$ENV:APPVEYOR_PROJECT_SLUG -Force -Recurse;
} else {
	# Do not assign a release number or deploy
  $env:CI_DEPLOY_GITHUB = $false;
}
