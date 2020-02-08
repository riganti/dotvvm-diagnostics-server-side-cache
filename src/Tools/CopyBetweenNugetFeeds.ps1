Param(
    [string]$version,
    [string]$server, 
    [string]$internalServer, 
    [string]$apiKey
)


### Configuration

$packages = @(
	[pscustomobject]@{ Package = "DotVVM.Diagnostics.ServerSideCache"; Directory = "DotVVM.Diagnostics.ServerSideCache" }
)

foreach($package in $packages){

    $packageId = $package.Package
    $webClient = New-Object System.Net.WebClient
    $url = "$internalServer/package/" + $packageId + "/" + $version
    $nupkgFile = Join-Path $PSScriptRoot ($packageId + "." + $version + ".nupkg")

    Write-Host "Downloading from $url"
    $webClient.DownloadFile($url, $nupkgFile)
    Write-Host "Package downloaded from '$internalServer'."

    Write-Host "Uploading package..."
    & .\Tools\nuget.exe push $nupkgFile -source $server -apiKey $apiKey
    Write-Host "Package uploaded to $server."

    Remove-Item $nupkgFile
}