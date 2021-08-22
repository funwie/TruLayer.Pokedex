## Pokedex with Fun Translation
A tool to get Pokemon information from external api and translate the pokemon description

### Requires C# and .Net 5
### Optional tools: Visual Studio or prefered IDE and Docker installed
### Tested using Nunit and XUnit frameworks

### Running Application
#### From Visual Studio or Rider
- Open the solution
- Set startup project to TruLayer.Pokedex.Api
- Set Debug option to IIS Express
- Debug the application.
- Use the Swagger UI to try out the api

### Terminal 
- Open the solution folder in a terminal
- Run `dotnet run --project src/TruLayer.Pokedex.Api/TruLayer.Pokedex.Api.csproj`

#### From Docker
- Open the solution folder in a terminal
- Run `docker build -f docker/Dockerfile -t pokedex .` 
- After image builds, run `docker run -dp 8000:80 --name pokedex-api pokedex`
- Access the pokedex api from localhost:8000 and explore the API with the Swagger UI
- Health enpoint is at /healthcheck

#### Run Tests
- Open the solution in VS and run all tests
- OR open the solution folder in terminal and run `dotnet test`

#### Improvements
- Add unit tests for more test cases
- Handle exceptions better depending on the requirements
- Additional logger
- Use a fake/mock api client for the integration tests
- Replace the many null returns with a Result return that can encapsulates success and failure cases
- Add persistence so that we can cache the data and not make the request to both apis every single time
