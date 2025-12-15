FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

# เดิม EXPOSE 8080,443 → เปลี่ยนเป็น 44307 (จะ EXPOSE 443 เพิ่มด้วยก็ได้ถ้าต้องใช้)
EXPOSE 44307

# ให้ Kestrel ฟังที่ port 44307 ภายใน container
ENV ASPNETCORE_URLS=http://0.0.0.0:44307
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# ระวัง path ตรงนี้ ต้องตรงกับ structure จริง
COPY ["Interview-Test/Interview-Test.Api/Interview-Test.Api.csproj", "Interview-Test.Api/"]
COPY ["Interview-Test/Interview-Test.Infrastructure/Interview-Test.Infrastructure.csproj", "Interview-Test.Infrastructure/"]

RUN dotnet restore "Interview-Test.Api/Interview-Test.Api.csproj"
COPY . .
WORKDIR "/src/Interview-Test/Interview-Test.Api"
RUN dotnet build "Interview-Test.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Interview-Test.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Interview-Test.Api.dll"]

# ตัวอย่างคำสั่ง build / run:
# docker build -f Interview-Test/Interview-Test.Api/Dockerfile -t interview-test.api .
# docker run --rm -it -p 44307:44307 --name interview-test-api interview-test.api
