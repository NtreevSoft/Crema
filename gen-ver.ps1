$majorVersion=4
$minorVersion=0
$sourcePath = Join-Path (Split-Path $myInvocation.MyCommand.Definition) ".\common\Ntreev.Crema.AssemblyInfo\AssemblyInfo.cs" -Resolve
$version="$majorVersion.$minorVersion"
$fileVersion="$majorVersion.$minorVersion"+"."+(Get-Date -Format yy)+(Get-Date).DayOfYear+"."+(Get-Date -Format HHmm)

$content = Get-Content $sourcePath -Encoding UTF8

$pattern1 = "(AssemblyVersion[(]`").+(`"[)]])"
if ($content -match $pattern1) {
    $content = $content -replace $pattern1, "`${1}$version`$2"
}
else {
    throw "AssemblyVersion not found."
}

$pattern2 = "(AssemblyFileVersion[(]`").+(`"[)]])"
if ($content -match $pattern2) {
    $content = $content -replace $pattern2, "`${1}$fileVersion`$2"
}
else {
    throw "AssemblyFileVersion not found."
}

$pattern3 = "(AssemblyInformationalVersion[(]`").+(`"[)]])"
if ($content -match $pattern3) {
    $content = $content -replace $pattern3, "`${1}$fileVersion`$2"
}
else {
    throw "AssemblyInformationalVersion not found."
}

Set-Content $sourcePath $content -Encoding UTF8
Set-Content version.txt $fileVersion
Write-Host $fileVersion
