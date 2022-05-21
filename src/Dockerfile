FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 8001
EXPOSE 9001 

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /dest
COPY ["IOTCS.EdgeGateway.Application/IOTCS.EdgeGateway.Application.csproj", "IOTCS.EdgeGateway.Application/"]
COPY ["IOTCS.EdgeGateway.BaseDriver/IOTCS.EdgeGateway.BaseDriver.csproj", "IOTCS.EdgeGateway.BaseDriver/"]
COPY ["IOTCS.EdgeGateway.CmdHandler/IOTCS.EdgeGateway.CmdHandler.csproj", "IOTCS.EdgeGateway.CmdHandler/"]
COPY ["IOTCS.EdgeGateway.Commands/IOTCS.EdgeGateway.Commands.csproj", "IOTCS.EdgeGateway.Commands/"]
COPY ["IOTCS.EdgeGateway.ComResDriver/IOTCS.EdgeGateway.ComResDriver.csproj", "IOTCS.EdgeGateway.ComResDriver/"]
COPY ["IOTCS.EdgeGateway.Core/IOTCS.EdgeGateway.Core.csproj", "IOTCS.EdgeGateway.Core/"]
COPY ["IOTCS.EdgeGateway.Diagnostics/IOTCS.EdgeGateway.Diagnostics.csproj", "IOTCS.EdgeGateway.Diagnostics/"]
COPY ["IOTCS.EdgeGateway.Dispatch/IOTCS.EdgeGateway.Dispatch.csproj", "IOTCS.EdgeGateway.Dispatch/"]
COPY ["IOTCS.EdgeGateway.Domain/IOTCS.EdgeGateway.Domain.csproj", "IOTCS.EdgeGateway.Domain/"]
COPY ["IOTCS.EdgeGateway.Freesql/IOTCS.EdgeGateway.Freesql.csproj", "IOTCS.EdgeGateway.Freesql/"]
COPY ["IOTCS.EdgeGateway.Freesql.Helper/IOTCS.EdgeGateway.Freesql.Helper.csproj", "IOTCS.EdgeGateway.Freesql.Helper/"]
COPY ["IOTCS.EdgeGateway.HttpHandler/IOTCS.EdgeGateway.HttpHandler.csproj", "IOTCS.EdgeGateway.HttpHandler/"]
COPY ["IOTCS.EdgeGateway.Infrastructure/IOTCS.EdgeGateway.Infrastructure.csproj", "IOTCS.EdgeGateway.Infrastructure/"]
COPY ["IOTCS.EdgeGateway.Library/IOTCS.EdgeGateway.Library.csproj", "IOTCS.EdgeGateway.Library/"]
COPY ["IOTCS.EdgeGateway.Logging/IOTCS.EdgeGateway.Logging.csproj", "IOTCS.EdgeGateway.Logging/"]
COPY ["IOTCS.EdgeGateway.MqttHandler/IOTCS.EdgeGateway.MqttHandler.csproj", "IOTCS.EdgeGateway.MqttHandler/"]
COPY ["IOTCS.EdgeGateway.Plugins/IOTCS.EdgeGateway.Plugins.csproj", "IOTCS.EdgeGateway.Plugins/"]
COPY ["IOTCS.EdgeGateway.Reflection/IOTCS.EdgeGateway.Reflection.csproj", "IOTCS.EdgeGateway.Reflection/"]
COPY ["IOTCS.EdgeGateway.Repository/IOTCS.EdgeGateway.Repository.csproj", "IOTCS.EdgeGateway.Repository/"]
COPY ["IOTCS.EdgeGateway.ResDriver/IOTCS.EdgeGateway.ResDriver.csproj", "IOTCS.EdgeGateway.ResDriver/"]
COPY ["IOTCS.EdgeGateway.Server/IOTCS.EdgeGateway.Server.csproj", "IOTCS.EdgeGateway.Server/"]
RUN dotnet restore "IOTCS.EdgeGateway.Server/IOTCS.EdgeGateway.Server.csproj"
COPY . .
WORKDIR "/dest/IOTCS.EdgeGateway.Server"
RUN dotnet build "IOTCS.EdgeGateway.Server.csproj" -c Release -o /app


FROM build AS publish
RUN dotnet publish "IOTCS.EdgeGateway.Server.csproj" -c Release -o /app


FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IOTCS.EdgeGateway.Server.dll"]
