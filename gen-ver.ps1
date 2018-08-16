$majorVersion=4
$minorVersion=0
$assemblyFilePath = ".\common\Ntreev.Crema.AssemblyInfo\AssemblyInfo.cs"

$assemblyPath = Join-Path (Split-Path $myInvocation.MyCommand.Definition) $assemblyFilePath -Resolve
$version = "$majorVersion.$minorVersion"
$fileVersion = "$majorVersion.$minorVersion" + "." + (Get-Date -Format yy) + (Get-Date).DayOfYear + "." + (Get-Date -Format HHmm)

if (Test-Path $assemblyPath) {
    $content = Get-Content $assemblyPath -Encoding UTF8

    $pattern1 = "(AssemblyVersion[(]`").+(`"[)]])"
    if ($content -match $pattern1) {
        $content = $content -replace $pattern1, "`${1}$version`$2"
    }

    $pattern2 = "(AssemblyFileVersion[(]`").+(`"[)]])"
    if ($content -match $pattern2) {
        $content = $content -replace $pattern2, "`${1}$fileVersion`$2"
    }

    $pattern3 = "(AssemblyInformationalVersion[(]`").+(`"[)]])"
    if ($content -match $pattern3) {
        $content = $content -replace $pattern3, "`${1}$fileVersion`$2"
    }

    $backupPath = $assemblyPath + ".bak"
    Copy-Item $assemblyPath $backupPath
    Set-Content $assemblyPath $content -Encoding UTF8

    if ($null -eq (Get-Content $assemblyPath)) {
        Remove-Item $assemblyPath
        Copy-Item $backupPath $assemblyPath
        Remove-Item $backupPath
        throw "replace version failed: $assemblyPath"
    }
    else {
        Remove-Item $backupPath
    }
}
else {
    throw "assembly path not found: $assemblyPath"
}

Set-Content version.txt $fileVersion
Write-Host $fileVersion