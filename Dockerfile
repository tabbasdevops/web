FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
EXPOSE 80/tcp
#exact source location (where dotnet publish did output) needs to be corrected for Jenkins
COPY ./bin/Release/netcoreapp3.1/publish/ .       
ENTRYPOINT ["dotnet", "web.dll"]