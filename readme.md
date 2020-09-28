# Using AutoRest and OpenAPI Tools to Generate SDKs

This repository has some sample Swagger files, representing currently-deployed APIs that will function as expected.

--- 

## Prerequisites

You will need the following dependencies on your machine to make the contents of this repository work properly. 

* Node (LTS is fine)
* AutoRest - Once you have Node installed, install AutoRest via:

    ```
    npm install -g autorest
    ```

    Then, validate the installation worked by running the `--help` command to see AutoRest's in-product documentation.

        ```
        autorest --help
        ```

* .NET Core - Install from https://dotnet.microsoft.com/download

* Open API Tools

    ```
    dotnet tool install --global Microsoft.dotnet-openapi --version 3.1.8
    ```
---

## API Explanations

There are two APIs deployed. These APIs are used in a demo COVID-19 screening application. There are two versions of the API deployed, demonstrating the variance in Open API structure when the APIs lack the `operationId` attribute for each operation.

### API with operationId

This API includes the `operationId` attribute. This is the ideal scenario, as the `operationId` attribute is used by the OpenAPI document owner to specify how the generated code *should* look.

> Note: Operation ID isn't **inherently** tied to code-generation, though it is frequently used by code-generation framework authors as the operation name the API author intended, and thus,  resepct the author's intent.

* Swagger UI: https://covidscreeningapi-withoperationid.azurewebsites.net/swagger/index.html

* Open API Doc: https://covidscreeningapi-withoperationid.azurewebsites.net/swagger/v1/swagger.json

### API without operationId

This API omits the `operationId` attribute. This is less ideal, as it forces code generators to "guess" how to create the SDK. Most generators can be tricked into generating something useful using switches, but the best option is to tackle operationId-inclusive APIs.

* Swagger UI: https://covidscreeningapis-nooperationid.azurewebsites.net/swagger/index.html

* Open API Doc: https://covidscreeningapis-nooperationid.azurewebsites.net/swagger/v1/swagger.json

--- 

### Download the APIs locally

Download the Open API **containing operation id attributes** spec using either the PowerShell or Linux/Mac flavor below. Whilst the generation sample scripts below presume the Open API files are local, you can aim most generators at URLs, too.

```powershell
#powershell
iwr https://covidscreeningapi-withoperationid.azurewebsites.net/swagger/v1/swagger.json -o covidapp-with-operationids.json
```

```bash
#bash
curl https://covidscreeningapi-withoperationid.azurewebsites.net/swagger/v1/swagger.json -o covidapp-with-operationids.json
```

Download the Open API spec **lacking operation id attributes** using either the PowerShell or Linux/Mac flavor below.

```powershell
#powershell
iwr https://covidscreeningapis-nooperationid.azurewebsites.net/swagger/v1/swagger.json -o covidapp-without-operationids.json
```

```bash
#bash
curl https://covidscreeningapis-nooperationid.azurewebsites.net/swagger/v1/swagger.json -o covidapp-without-operationids.json
```

--- 

## Generating C# client code using AutoRest

This section will show you how to use AutoRest to try both the API versions - with and without `operationId` to see the generated code quality.

### Generating clients using AutoRest for APIs with operationId

Execute AutoRest against the downloaded file.

```
autorest --input-file=covidapp-with-operationids.json --csharp --output-folder=Generated/WithOperationId/CovidScreeningApi --namespace=CovidScreeningApi
```

### Generating clients using AutoRest for APIs without operationId

Execute AutoRest against the downloaded file. 

```
autorest --input-file=covidapp-without-operationids.json --csharp --output-folder=Generated/WithOutOperationId/CovidScreeningApi --namespace=CovidScreeningApi
```

> Note: This will NOT work, as AutoRest will, by default, throw when it can't properly process the API. There are hacks one could implement to make AutoRest work here, but usually, the output will be sub-optimal in contrast to the SDK generated inclusive of `operationId` in the incoming Open API document. This set of steps is only here for documentation purposes.

---

## Generating clients using dotnet openapi tools 

This section will show you how to use the .NET global tool `dotnet openapi` to generate clients from Open API docs - both with and without `operationId`. This way you can see a contrast in the feature set of `dotnet openapi` to AutoRest. 

### Generating clients using dotnet openapi with operationid

This section will show you how to create a Console Application project that will use the [Microsoft.dotnet-openapi .NET global tool](https://www.nuget.org/packages/Microsoft.dotnet-openapi/3.1.8). Since the `dotnet openapi` tool operates on an existing .NET project, you'll first want to create a project for it to operate on. 

```
dotnet new console -o SwaggerClientConsoleWithOperationId
cd SwaggerClientConsoleWithOperationId
```

Now you should be in the directory of the new project. Once in there, you'll want to run the following `dotnet openapi` command to add the Open API as a connected service to the .NET project file.

```
dotnet openapi add -p SwaggerClientConsoleWithOperationId.csproj url https://covidscreeningapi-withoperationid.azurewebsites.net/swagger/v1/swagger.json
```

Once the client is created you can add code to the `Program.cs` file to make a call to the API.

```csharp
var apiClient = new swaggerClient(new System.Net.Http.HttpClient());
```

Now, use IntelliSense to see the various API calls you can make. You'll see method names that hang off of the `swaggerClient` named according to what was identified in the `operationId` attribute for each method. When aimed at the `operationId`-inclusive version of the Open API document, the generated method names are very friendly:

* `CreatePortOfEntryAsync`
* `CreateRepresentativeAsync`
* `CreateScreeningAsync`
* `GetPortOfEntryAsync`
* `GetRepresentativeByIdAsync`
* `UpdateScreeningAsync`

Think of using the `PortsOfEntry`-specific APIs to get all, get one, create, and then update a port of entry model:

```csharp
apiClient.PortsOfEntryAllAsync();               // gets all
apiClient.PortsOfEntry2Async(Guid.Empty);       // gets one
apiClient.PortsOfEntryAsync(null);              // create
apiClient.PortsOfEntry3Async(Guid.Empty, null); // update
```

### Generating clients using dotnet openapi tools for APIs without operationId

This section is similar to the previous one, yet in this, you'll use the `dotnet openapi` tool on an API that lacks the `operationId` attribute. The result is that the SDK generation will work fine, but the generated code isn't nearly as clean, since the generator has to "guess" what to name the methods.

```
dotnet new console -o SwaggerClientConsoleNoOperationId
cd SwaggerClientConsoleNoOperationId
```

Now you should be in the directory of the new project. Once in there, you'll want to run the following `dotnet openapi` command to add the Open API as a connected service to the .NET project file.

```
dotnet openapi add -p SwaggerClientConsoleNoOperationId.csproj url https://covidscreeningapis-nooperationid.azurewebsites.net/swagger/v1/swagger.json
```

Once the client is created you can add code to the `Program.cs` file to make a call to the API.

```csharp
var apiClient = new swaggerClient(httpClient);
```

When aimed at the `operationId`-lacking version of the Open API document, the generated method names aren't nearly as friendly. A developer would have to pause as each expands in the IDE to guess what method to call based on the parameterization: 

```csharp
apiClient.PortsOfEntryAllAsync();               // gets all
apiClient.PortsOfEntry2Async(Guid.Empty);       // gets one
apiClient.PortsOfEntryAsync(null);              // create
apiClient.PortsOfEntry3Async(Guid.Empty, null); // update
```
## Summary

From this exercise you can basically see the reliance of both the `dotnet openapi` tool and AutoRest on the presence of `operationId` in the Open API operation document. This table summarized the resulting experience for developers who will use the generated code. 

|Generator      |Works without operationId  |Code is clean without operationId  |Code is clean with operationId |Built into VS/VS for Mac|
|---------------|--------|--------|--------|--------|
|dotnet openapi |Yes     |No      |Yes     |Yes     |
|AutoRest       |No      |No      |Yes     |No      |