param([String]$key)

$span = New-TimeSpan -Start 2010/1/1 -End (Get-Date)
$version = 'beta-' + $span.Days.ToString() + ($span.Hours * 60 + $span.Minutes).ToString("0000")
& dotnet restore
cd .\src\Folke.Identity.Server
& dotnet pack --version-suffix $version
$file = Get-Item "bin\Debug\*-$version.nupkg"
nuget push $file.FullName $key 
cd ..\..
