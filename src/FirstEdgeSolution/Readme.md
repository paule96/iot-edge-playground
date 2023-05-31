# Sample 1

## Build and Run

1. Build your solution

```powershell
iotedgedev solution build
```

2. Now go to the azure portal and create an iot hub
3. under the devices section in the iot hub create a new device
4. after the device is created, open it in the azure portal and copy the connection string
5. Run your solution in the simulator

```powershell
$env:DEVICE_CONNECTION_STRING=(Read-Host -MaskInput)
iotedgedev simulator setup   
iotedgedev simulator start --file ./config/deployment.amd64.json
```

6. with docker cli you can check that everything is up and running

```powershell
docker ps
```

## Monitor you application

> You need for the next step your iot hub connection string.

```powershell
$env:IOTHUB_CONNECTION_STRING=(Read-Host -MaskInput)
iotedgedev iothub monitor
```

## Access the database

```
Server=tcp:localhost,1433;Initial Catalog=master;Persist Security Info=False;Encrypt=True;TrustServerCertificate=False;User Id=sa;Password=<Default_MSSQL_SA_Password>;
```