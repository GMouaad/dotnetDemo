FROM mcr.microsoft.com/dotnet/aspnet:5.0
# mcr.microsoft.com/dotnet/runtime:5.0 # can be used instead

COPY bin/Release/net5.0/publish/ App/

ENV BROKER_HOSTNAME=localhost
ENV BROKER_PORT=9883
ENV PUBLISH_INTERVAL=2000

WORKDIR /App
ENTRYPOINT ["dotnet", "MqttProducer.dll"]