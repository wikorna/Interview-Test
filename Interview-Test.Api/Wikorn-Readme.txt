DOCKER
======
docker build -f Interview-Test/Interview-Test.Api/Dockerfile -t interview-test.api .

docker run --rm -it -p 44307:44307 -e ASPNETCORE_ENVIRONMENT=Development interview-test.api

docker run --rm -it -p 44307:44307 interview-test.api


dotnet ef migrations add InitializeDb -p Interview-Test.Infrastructure -s Interview-Test.Api

//MacOS
dotnet ef migrations add InitializeDb -s ./Interview-Test.Api -p ./Interview-Test.Infrastructure
dotnet ef database update -s ./Interview-Test.Api -p ./Interview-Test.Infrastructure

//Windows
dotnet ef migrations add InitializeDb -s .\Interview-Test.Api -p .\Interview-Test.Infrastructure
dotnet ef database update -s .\Interview-Test.Api -p .\Interview-Test.Infrastructure


dotnet ef migrations remove -s ./Interview-Test.Api -p ./Interview-Test.Infrastructure


dotnet ef migrations add AddMoreUserRole -p Interview-Test.Infrastructure -s Interview-Test.Api
dotnet ef database update -p Interview-Test.Infrastructure -s Interview-Test.Api
