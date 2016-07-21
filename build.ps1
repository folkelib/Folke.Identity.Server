param([String]$key,[String]$version = $null)

function setProjectionVersion([String]$fileName, [String]$version) {
    $content = (Get-Content $fileName) -join "`n" | ConvertFrom-Json
    $content.version = $version
    $newContent = ConvertTo-Json -Depth 10 $content
    Set-Content $fileName $newContent
}

if ($version -ne $null) {
    setProjectionVersion ".\src\Folke.Identity.Server\project.json" $version
    & dotnet restore
    cd .\src\Folke.Identity.Server
    & dotnet pack -c Release
    $file = Get-Item "bin\Release\*-$version.nupkg"
    # nuget push $file.FullName $key -Source https://api.nuget.org/v3/index.json
    cd ..\..
}