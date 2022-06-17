Import-Module Microsoft.PowerShell.SecretManagement

$s = New-PSSession -HostName (Get-Secret HomeAssistantHost -AsPlainText) -UserName (Get-Secret HomeAssistantUsername -AsPlainText)

$sourcePath = Get-Location;
$secretFiles = Get-Childitem -Path "$($sourcePath)/**/secret*" -Recurse;
$secretFiles += Get-Childitem -Path "$($sourcePath)/**/service_account.json" -Recurse
$secretFiles += Get-Childitem -Path "$($sourcePath)/.env"
$secretFiles += Get-Childitem -Path "$($sourcePath)/reverseproxy/traefik.toml"
Foreach($secretFile in $secretFiles)
{
    if ($secretFile -notmatch ".stubs")
    {
        $childPath = "$secretFile".substring("$($sourcePath)".length+1)
        $dest = "$(Get-Secret HomeAssistantConfigPath -AsPlainText)\$($childPath)"
        Invoke-Command -Session $s -ScriptBlock { New-Item -Path $args[0] -type File -Force } -ArgumentList $dest
        Copy-Item -Path $secretFile -Destination $dest -Force -ToSession $s
    }
}
