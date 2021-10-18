# MQTTConsumer

## Docker
 - docker volume create --name sensor-data
 - Build: *docker build -t MqttConsumer -f Dockerfile*
 - Create: *docker create --name MqttConsumer-1 MqttConsumer*
 - Start: *docker start MqttConsumer-1*
 - Run interractivally: docker run -it -v sensor-data:/var/data
## References
 - [MQTTNet library](https://github.com/chkr1011/MQTTnet)
 - [Containerize .NET app](https://docs.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=linux)