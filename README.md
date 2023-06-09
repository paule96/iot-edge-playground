# iot-edge-playground
just some fun with IOT edge

## Sample 1

### Build and Run

0. init git submodules

```powershell
git submodule update --init --recursive
```

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

> Note: for the first start you must provision the database. see database access

### Monitor you application

> You need for the next step your iot hub connection string.

```powershell
$env:IOTHUB_CONNECTION_STRING=(Read-Host -MaskInput)
iotedgedev iothub monitor
```

### Access the database

To access the database you can use the following connection string on your development machine.

```
Server=tcp:localhost,1433;Initial Catalog=master;Persist Security Info=False;Encrypt=True;TrustServerCertificate=True;User Id=sa;Password=ThisIsAStrongPassword1!!11;
```

This can be used on the left side with the database project after starting the solution. There you will find a SQL script that will setup a database table for you.
After this the application is up and running.

### Access Grafana

> Note: Right now grafana needs to setup after every restart of the solution. This will be fixed later.

To access grafanan search in the `Ports` section of you VSCode for an exposed port with the label `Grafana`. Then you can just open the browser on the vscode generate external port.
After that you can login with `admin`as username and password.

A sample query for an Dashboard is the following:

```sql
Select [Timestamp], [Temperature], [Type] AS metric From [dbo].[Temperature] ORDER BY [Timestamp] DESC
```

## Start Krafana and influx

To start Krafana and influx run:

```powershell
docker compose -f docker-compose-kafka.yml up -d
```

To stop run

```powershell
docker compose -f docker-compose-kafka.yml down -d
```

To connect to it, use localhost as DNS name.

### Access Kafka Broker

You can access the broker conainer with `docker exec -it broker bash`

Use VS Code's Kafka extension to browse brokers or to list and create topics.

You can use the Kafka-CLI inside the broker container to produce messages. 

`kafka-console-producer --bootstrap-server=localhost:9092 --topic=quickstart`

### Acess InfluxDB 

You can access the GUI via localhost:8086