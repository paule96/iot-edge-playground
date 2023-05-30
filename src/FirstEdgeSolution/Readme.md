# Sample 1

## Build and Run

1. Start the simulator in a new terminal

```powershell
iotedgedev start --setup --file config/deployment.amd64.json
```

2. Build your solution

```powershell
iotedgedev solution build
```

3. Now go to the azure portal and create an iot hub
4. under the devices section in the iot hub create a new device
5. after the device is created, open it in the azure portal and copy the connection string
6. Run your solution in the simulator

```powershell
$env:DEVICE_CONNECTION_STRING=(Read-Host -MaskInput)
iotedgedev simulator setup   
iotedgedev simulator start --file ./config/deployment.amd64.json
```

7. with docker cli you can check that everything is up and running

```powershell
docker ps
```