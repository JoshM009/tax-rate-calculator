# tax-rate-calculator
## Run and Test
The incomeTax project provides an API endpoint for a user to calculate their total taxes for a given year and annual income.

It requires a .NET Core 5.0.4+ SDK and the provided "ptsdocker16/interview-test-server" Docker container to run. The SDK can be downloaded at: https://dotnet.microsoft.com/en-us/download/dotnet/5.0

The docker container can be run with the following commands:

`docker pull ptsdocker16/interview-test-server`

`docker run --init -p 5000:5000 -it ptsdocker16/interview-test-server`

The project can be unit tested by cloning the project and running the command:

`dotnet test`

The project can be run using the command:

`dotnet run`

This will start the server listening on port 5050. The api is located at:

http://localhost:5050/api/tax/total/marginal

The swagger documentation can be viewed at:

http://localhost:5050/swagger/index.html

The API can be queried using Swagger or with a POST request like below:

```
curl --location --request POST 'localhost:5050/api/tax/total/marginal' \
--header 'Content-Type: application/json' \
--data-raw '{
    "TaxYear": 2021,
    "Income": 70000
}'
```

Alternatively the integration Test class TaxServiceIntegrationTest can be used to query the API by updating the input parameters as required, uncommenting the [Fact] attribute and then running the test.

# Project Details
The main project, incomeTax, is located in the incomeTax folder. It contains the classes that enable the Marginal Tax api to calculate the total income tax for a given tax year and annual income.

The tests for this project are located in the test folder. There is a set of unit tests and a set of integration tests.