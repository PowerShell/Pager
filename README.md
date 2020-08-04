# Pager

The `Pager` is a cross platform terminal utility to view content one screen at a time.
The content are shown on an alternate screen buffer.
It currently supports scrolling up and down using arrow keys and returning the primary screen buffer using `Q`.

The `Pager` is available as a NuGet package `Microsoft.PowerShell.Pager` on [NuGet.org](https://www.nuget.org/packages/Microsoft.PowerShell.Pager/).

## Sample usage

The following demo shows the usage for showing the PowerShell help content on alternate screen and returning to the primary screen.

![Pager-Basic](./assets/Pager-Basic.gif)

The following shows the ability to scroll to the first match of a regular expression pattern.

![Pager-ScrollTo](./assets/Pager-ScrollTo.gif)

## Build status

[![Build Status](https://dev.azure.com/powershell/Microsoft.PowerShell.Pager/_apis/build/status/PSPager-CI?branchName=master)](https://dev.azure.com/powershell/Microsoft.PowerShell.Pager/_build/latest?definitionId=98&branchName=master)
