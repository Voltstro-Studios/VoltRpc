#This is so we can do some custom stuff before DocFX builds
#We need to include the changelog as apart of the site, but DocFX REFUSES to include anything that isn't in the current directory

Push-Location $PSScriptRoot

#Create new CHANGELOG.md file, add custom header to it, and copy the contents of the main changelog
$content = "---`ndisableToc: true`ndisableReadingTime: true`n---`n"
$content | Set-Content 'CHANGELOG.md'
Get-Content '../CHANGELOG.md' -ReadCount 5000 | Add-Content 'CHANGELOG.md'

#Get Last Modfied of changelog and set our duplicate's one to it
$modfiedDate = (Get-Item '../CHANGELOG.md').LastWriteTime
(Get-Item 'CHANGELOG.md').LastWriteTime = $modfiedDate

#Now build the site
& docfx build

#Remove duplicate changelog
Remove-Item 'CHANGELOG.md'

Pop-Location