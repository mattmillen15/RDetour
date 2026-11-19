Import-Module RemoteDesktopServices  
Get-ChildItem RDS:\GatewayServer | Out-String | Out-File C:\cfgrdg_result.txt  
try {  
New-Item RDS:\GatewayServer\CAP -Name AllowAllCAP -UserGroups @(\"ecorp.local\Domain Users\") -AuthMethod 1  
} catch {  
$_.Exception.Message | Out-File C:\cfgrdg_result.txt -Append  
}  
