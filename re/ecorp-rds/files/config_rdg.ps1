Import-Module RemoteDesktopServices  
Set-Item RDS:\GatewayServer\SSLCertificate\Thumbprint -Value 7b0a852b4e479136834fbcbabe5034fdec12214c  
New-Item -Path RDS:\GatewayServer\CAP -Name TestCAP -UserGroups @(\"ecorp.local\Domain Users\") -AuthMethod 1  
New-Item -Path RDS:\GatewayServer\RAP -Name TestRAP -UserGroups @(\"ecorp.local\Domain Users\") -ComputerGroupType 2  
Restart-Service TSGateway  
