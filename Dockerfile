# Etapa base: Configuração do runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.19 AS base
WORKDIR /app

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

SHELL ["/bin/ash", "-eo", "pipefail", "-c"]

RUN apk update && \
    apk add --no-cache icu-libs=74.1-r0 icu-data-full=74.1-r0 && \
    LIBSSL_VERSION=$(apk search --no-cache libssl3 | grep '^libssl3-[0-9]\+\.[0-9]\+\.[0-9]\+-r[0-9]\+' | tail -n 1 | sed 's/libssl3-//') && \
    apk add --no-cache "libssl3=$LIBSSL_VERSION"

# Etapa de build: Construção da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine3.19 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["API/API.csproj", "API/"]
RUN dotnet restore "API/API.csproj"

# Copiar o restante dos arquivos e publicar a aplicação
COPY . .
RUN dotnet publish "API/API.csproj" -c Release -o /app/publish

# Etapa final: Configuração do contêiner final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expor portas necessárias
EXPOSE 8080

# Definir o comando de entrada
ENTRYPOINT ["dotnet", "API.dll"]