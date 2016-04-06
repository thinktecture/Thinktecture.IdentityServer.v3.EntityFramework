Write-Host "Pushing to NuGet gallery"

$nugetApiKey = "17d0e131-77ec-4f6b-a991-72b5032262bb"
$nugetFeedUrl = "https://repository.domain.com.au/nuget/platform/package"
$nugetExeLocation = ".\\Source\\.nuget\\NuGet.exe"
$componentPackageLocation = ".\\distribution"

Write-Host "Nuget repository Url: " $nugetFeedUrl

if (![string]::IsNullOrEmpty($nugetFeedUrl))
{	
	if (Test-Path $componentPackageLocation) 
	{
		Write-Host "The package folder exists"		
		
		$packagePaths = Get-ChildItem .\distribution\*.* -Include "*.nupkg" -Recurse | foreach-object {$_.Fullname}
		
		foreach($package in Get-ChildItem $packagePaths)
		{
			Write-Host 'Pushing component package ' $package			
			& $nugetExeLocation push $package $nugetApiKey -s $nugetFeedUrl
		}
	}
	else
	{
		Write-Host "The component package folder doesn't exist"
	}
}
else
{
	Write-Host "There is no feed URL"
}