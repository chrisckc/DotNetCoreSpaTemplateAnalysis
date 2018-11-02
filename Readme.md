# .Net Core Spa Template Analysis

Created using dotnet new angular and then stripped down, removing the Angular ClientApp.

Used to analyse the operation of the Spa middleware

ClientApp dir deleted and replace with ClientApp/dist containing some test html files.

app.UseStaticFiles(); commented out as not relevant for this purpose.

Removed "ASPNETCORE_ENVIRONMENT": "Development" entry from Properties/launchSettings.json to allow running in production mode by using env vars.

To see where the proxied requests are going, first run an instance of the Node Http server from the ClientApp dir:

First install the Node Http server globally if you need to:
```sh
npm install http-server -g
```

Then run it:

```sh
cd ClientApp
http-server dist --cors
```

This should run Node Http server on port 8080 serving files from the dist dir. The requests can be seen in the Node Http server console output

To run the .Net Core app in Development mode

```sh
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

To run in Production mode:

```sh
ASPNETCORE_ENVIRONMENT=Production dotnet run
```
