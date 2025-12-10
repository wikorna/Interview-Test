docker build -f Interview-Test/Interview-Test.Api/Dockerfile -t interview-test.api .

docker run --rm -it -p 8000:8080 -e ASPNETCORE_ENVIRONMENT=Development interview-test.api

docker run --rm -it -p 8000:8080 interview-test.api


dotnet ef migrations add FixUserRoleMapping -p Interview-Test.Infrastructure -s Interview-Test.Api
dotnet ef migrations add AddMoreUserRole -p Interview-Test.Infrastructure -s Interview-Test.Api
dotnet ef database update -p Interview-Test.Infrastructure -s Interview-Test.Api
