<#
.SYNOPSIS
    Holt alle für den CI-Build nötigen globalen Scripts aus einen zentralen Repository
.DESCRIPTION
    Holt alle für den CI-Build nötigen globalen Scripts aus einen zentralen Repository

.EXAMPLE
    .\get_scripts.ps1 '1.0.0' 'xxxxxxxx'
#>

Param(
    [parameter(mandatory = $true)][string] $version,
    [parameter(mandatory = $true)][string] $apiToken
)

function Unzip-NoTopFolders {
    param([string]$zipfile, [string]$out)
    Add-Type -AssemblyName System.IO.Compression, System.IO.Compression.FileSystem


    $Archive = [System.IO.Compression.ZipFile]::OpenRead($zipfile)
    try {
        Foreach ($entry in $Archive.Entries.Where( { $_.Name.length -gt 0 })) {
            $parts =$entry.FullName.Split("/")
            $filepath = ""
            for($idx = 2; $idx -lt $parts.length; $idx++) {
                $filepath = [System.IO.Path]::Combine($filepath, $parts[$idx])
                if($idx -lt ($parts.length - 1)) {
                    # it's sub folder create it
                    New-Item -ItemType Directory -Force "$out\$($filepath)"
                }
            }
            [System.IO.Compression.ZipFileExtensions]::ExtractToFile($entry, "$out\$($filepath)", 1)
        }
    }
    finally {
        $Archive.Dispose()
    }
}

$outputDir = "$($env:CI_PROJECT_DIR)\BuildScripts"
New-Item -ItemType Directory -Force $outputDir 

$outputFile = "$($outputDir)\archive.zip"

try {
    Invoke-RestMethod "https://git.bl.ch/api/v4/projects/270/repository/archive.zip?sha=$($version)&path=powershell&private_token=$($apitoken)" `
    -Method 'GET' `
    -OutFile $outputFile

    Unzip-NoTopFolders -zipfile $outputFile -out $outputDir
    remove-item $outputFile
}
catch [Exception]
{
    Write-Host $_.Exception.Message
    $PSItem.InvocationInfo | Format-List
    $Host.SetShouldExit(1);
    exit 1
}

