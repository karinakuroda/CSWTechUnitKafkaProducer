# CSW Tech Unit Kafka (Producer)

## Pre-Requirements


### 1 - Docker run (Kafka+Zookeper)


```bash
docker run -p 2181:2181 -p 9092:9092 --env ADVERTISED_HOST=127.0.0.1 --env ADVERTISED_PORT=9092 spotify/kafka
```

### 2 - Java enviroment
https://www.oracle.com/technetwork/java/javase/downloads/jre8-downloads-2133155.html

### 3 - Create Topics
> Use the kafka-topics.bat file to create:
```bash
kafka-topics.bat --create --bootstrap-server localhost:9092 --replication-factor 1 --partitions 1 --topic csw-topic
kafka-topics.bat --create --bootstrap-server localhost:9092 --replication-factor 1 --partitions 1 --topic csw-topic-portugal
kafka-topics.bat --create --bootstrap-server localhost:9092 --replication-factor 1 --partitions 1 --topic csw-topic-espanha

```


### Links
* https://bitbucket.critical.pt/projects/ALCHEMISTSTRAINING/repos/techunits/browse/techunits/kafka.md

