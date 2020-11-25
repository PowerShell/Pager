## Copyright (c) Microsoft Corporation.
## Licensed under the MIT License.

[CmdletBinding()]
param (
    [Parameter()]
    [string]
    $Configuration = "Debug",

    [Parameter()]
    [switch]
    $Clean
)

try {
    Push-Location "$PSScriptRoot/src"

    if ($Clean) {
        dotnet clean
    }

    dotnet build --configuration $Configuration
}
finally {
    Pop-Location
}
