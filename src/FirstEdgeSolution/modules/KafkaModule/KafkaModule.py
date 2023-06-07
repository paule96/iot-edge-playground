import sys
from confluent_kafka import Consumer, KafkaError, KafkaException

import influxdb_client, os, time
from influxdb_client import InfluxDBClient, Point, WritePrecision
from influxdb_client.client.write_api import SYNCHRONOUS

# Kafka

BOOTSTRAP_SERVERS = "localhost:9092"
AUTO_OFFSET_RESET = "earliest"
ENABLE_AUTO_COMMIT = False
GROUP_ID = "test"
TOPICS = ["quickstart"]

# InfluxDB

TOKEN = "dLHMQ1UoMcx7RUcJmqWD4vJBAWzCnelYJkfuh_zMDq9sVpXiDb_LBB-ADGRqVHcZBewlfVz40ImfJzlo8pOfgw=="
ORG = "test"
URL = "http://localhost:8086"
BUCKET = "test"

def write_to_db(msg):
    print(msg.decode())
    client = influxdb_client.InfluxDBClient(url=URL, token=TOKEN, org=ORG)

    write_api = client.write_api(write_options=SYNCHRONOUS)
    
    point = (
        Point("measurement1")
        .tag("tagname1", "tagvalue1")
        .field("field1", msg.decode())
    )
    write_api.write(bucket=BUCKET, org=ORG, record=point)

def msg_process(msg):
    write_to_db(msg.value())

def connect_to_kafka():
    conf = {'bootstrap.servers': BOOTSTRAP_SERVERS,
            'auto.offset.reset': AUTO_OFFSET_RESET,
            'enable.auto.commit': ENABLE_AUTO_COMMIT,
            'group.id': GROUP_ID}
    consumer = Consumer(conf)
    topics = TOPICS
    try:
        consumer.subscribe(topics)
    except Exception as e:
        raise(e)
    return consumer

if __name__ == "__main__":
    consumer = connect_to_kafka()
    try:
        while True:
            msg = consumer.poll(1)
            if msg is None:
                continue
            if msg.error():
                if msg.error().code() == KafkaError._PARTITION_EOF:
                    sys.stderr.write('%% %s [%d] reached end at offset %d\n' %
                                     (msg.topic(), msg.partition(), msg.offset()))
                elif msg.error():
                    raise KafkaException(msg.error())
            else:
                print(msg.value())
                msg_process(msg)
    except KeyboardInterrupt:
        consumer.close()
    finally:
        consumer.close()
