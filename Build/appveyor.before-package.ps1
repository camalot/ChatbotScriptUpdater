if ( !$env:APPVEYOR_PULL_REQUEST_NUMBER -and ($env:APPVEYOR_REPO_BRANCH -eq "develop") ) {
  $env:CI_DEPLOY_GITHUB = $false;

	if ( !(Test-Path -Path "../deps/$ENV:APPVEYOR_PROJECT_SLUG" ) ) {
			Write-Host "Create ../deps/$ENV:APPVEYOR_PROJECT_SLUG";
			New-Item -Path "../deps/$ENV:APPVEYOR_PROJECT_SLUG" -ItemType Directory | Out-Null;
	}
	Write-Host "Copy * to ../deps/$ENV:APPVEYOR_PROJECT_SLUG";
	Copy-Item -Path * -Include "*.exe", "*.dll" -Destination "../deps/$ENV:APPVEYOR_PROJECT_SLUG";

	Compress-Archive -Path ../deps/$ENV:APPVEYOR_PROJECT_SLUG/* -CompressionLevel Optimal -DestinationPath "../deps/$($ENV:APPVEYOR_PROJECT_SLUG)-v$($ENV:APPVEYOR_BUILD_VERSION).zip";

	Get-ChildItem -Path ../deps/$ENV:APPVEYOR_PROJECT_SLUG/ | Write-Host;

	Remove-Item -Path ../deps/$ENV:APPVEYOR_PROJECT_SLUG -Force -Recurse;
} else {
	# Do not assign a release number or deploy
  $env:CI_DEPLOY_GITHUB = $false;
}
