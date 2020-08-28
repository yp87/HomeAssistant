Import-Module Microsoft.PowerShell.SecretManagement

$s = New-PSSession -HostName (Get-Secret HomeAssistantHost -AsPlainText) -UserName (Get-Secret HomeAssistantUsername -AsPlainText)#(New-Object PSCredential "$(Get-Secret HomeAssistantUsername -AsPlainText),$(Get-Secret HomeAssistantPassword -AsPlainText)")

$SourcePath = Get-Location;
Get-Childitem -Path "$($SourcePath)/**/secret*.yaml" -Recurse |
ForEach-Object {
  $childPath = "$_".substring("$($sourcePath)".length+1)
  $dest = "$(Get-Secret HomeAssistantConfigPath -AsPlainText)\$($childPath)"
  Invoke-Command -Session $s -ScriptBlock { New-Item -Path $args[0] -type File -Force } -ArgumentList $dest
  Copy-Item -Path $_ -Destination $dest -Force -ToSession $s
}
