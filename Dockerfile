# 1. Usa la imagen oficial de .NET 8 SDK para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia el archivo .csproj y restaura las dependencias
COPY *.sln ./
COPY src/FDLM.Application/FDLM.Application.csproj src/FDLM.Application/
COPY src/FDLM.Domain/FDLM.Domain.csproj src/FDLM.Domain/
COPY src/FDLM.Infrastructure.EntrypointsAdapters/FDLM.Infrastructure.EntrypointsAdapters.csproj src/FDLM.Infrastructure.EntrypointsAdapters/
COPY src/FDLM.Infrastructure.OutpointsAdapters/FDLM.Infrastructure.OutpointsAdapters.csproj src/FDLM.Infrastructure.OutpointsAdapters/
COPY src/FDLM.Utilities/FDLM.Utilities.csproj src/FDLM.Utilities/
COPY src/FDLM.Runner/FDLM.Runner.csproj src/FDLM.Runner/
COPY tests/FDLM.Test/FDLM.Test.csproj tests/FDLM.Test/

RUN dotnet restore

# Copia el resto de los archivos y compila la aplicación
COPY . .
RUN dotnet publish -c Release -o /app/out

# 2. Usa la imagen de runtime de .NET 8 para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Elimina los binarios de las pruenas unitarias
RUN rm FDLM.Test*
RUN rm xunit*
RUN rm Moq.*

ENV ASPNETCORE_ENVIRONMENT=local

# Expone el puerto 8080
EXPOSE 8080

# Define el punto de entrada del contenedor
ENTRYPOINT ["dotnet", "FDLM.Runner.dll"]