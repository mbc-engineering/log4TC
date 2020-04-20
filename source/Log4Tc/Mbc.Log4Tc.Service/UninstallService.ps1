$serviceName = "Mbc.Log4Tc.Service"
$servicePath = Get-Location

# Stop
Stop-Service -Name $serviceName

# Remove
sc.exe delete $serviceName
# version 6: Remove-Service -Name $serviceName