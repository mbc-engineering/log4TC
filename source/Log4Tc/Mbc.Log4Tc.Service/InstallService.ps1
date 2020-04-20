$serviceName = "Mbc.Log4Tc.Service"
$servicePath = $PSScriptRoot

<# Permission for user execution -> add -Credential $serviceUser
$serviceUser = "$env:ComputerName\$env:UserName"
$acl = Get-Acl "./"
$aclRuleArgs = $serviceUser, "Read,Write,ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($aclRuleArgs)
$acl.SetAccessRule($accessRule)
$acl | Set-Acl "./"
#>

# Install
New-Service -Name $serviceName -BinaryPathName "$servicePath\$serviceName.exe --service" -Description "mbc Log4Tc service" -DisplayName $serviceName -StartupType Automatic -DependsOn "TcSysSrv"

# Start
Start-Service -Name $serviceName
Get-Service -Name $serviceName