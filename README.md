# LBH Repairs API

The Repairs API is part of the Hackney API ecosystem and is responsible for all endpoints related to repairs of homes owned and managed by the London Borough of Hackney.

## Stack

- .NET Core as a web framework.
- xUnit as a test framework.

## Dependencies

- Universal Housing
- NCC

## Development practices

We employ a variant of Clean Architecture, borrowing from [Made Tech Flavoured Clean Architecture][mt-ca] and Made Tech's [.NET Clean Architecture library][dotnet-ca].

## Contributing

### Setup

1. Install [Docker][docker-download].
2. Clone this repository.
3. Open it in your IDE.

### Development

To serve the application, run it using your IDE of choice. We've Visual Studio CE on Mac.

The test suite depends on a local version of Universal Housing, to simulate the legacy data source. To bring this up, run the following. You can then run the tests using the test explorer in your IDE.

```sh
$ docker run -it --rm repairs-api dotnet HackneyRepairs.dll
```
To run on particular port,the command is as below.
```sh
$ docker run -p 3000:80 -it --rm repairs-api dotnet HackneyRepairs.dll
```
### Release

![Circle CI Workflow Example](docs/circle_ci_workflow.png)

We use a pull request workflow, where changes are made on a branch and approved by one or more other maintainers before the developer can merge into `central`.

Then we have an automated four step deployment process, which runs in CircleCI.

1. Automated tests (xUnit) are run to ensure the release is of good quality.
2. The app is deployed to staging automatically, where we check our latest changes work well.
3. We manually confirm a production deployment in the CircleCI workflow once we're happy with our changes in staging.
4. The app is deployed to production.

## Testing

### Universal Housing tests

We use the [Universal Housing Simulator][universal-housing-simulator] to test the UHT and UH Warehouse repositories.

You must have it running locally on port 1433 for these tests to run, otherwise they'll be automatically skipped.

To build and run the simulator, follow these steps:

```sh
$ git clone https://github.com/LBHackney-IT/lbh-universal-housing-simulator
$ cd lbh-universal-housing-simulator
$ docker build -t uhsimulator .
$ docker run -ti -p 1433:1433 --rm uhsimulator
```
## Endpoints
 [Endpoints](docs/endpoints.md)
 
## Contacts

### Active Maintainers

- **Rashmi Shetty**, Development Manager at London Borough of Hackney (rashmi.shetty@hackney.gov.uk)
- **Vlad Atamanyuk**, Junior Developer at London Borough of Hackney (vladyslav.atamanyuk@hackney.gov.uk)
- **Jeff Pinkham**, Engineer at [Made Tech][made-tech] (jeff@madetech.com)
- **Mark Rosel**, Lead Engineer at [Made Tech][made-tech] (mark.rosel@madetech.com)
- **Steven Leighton**, Engineer at [Made Tech][made-tech] (steven@madetech.com)
- **Cormac Brady**, Engineer at [Made Tech][made-tech] (cormac@madetech.com)
- **Elena Vilimaite**, Engineer at [Made Tech][made-tech] (elena@madetech.com)

### Other Contacts

- **Richard Foster**, Lead Engineer at [Made Tech][made-tech] (richard@madetech.com)
- **Luke Morton**, Director at [Made Tech][made-tech] (luke@madetech.com)
- **Dennis Robinson**, Delivery Lead at London Borough of Hackney (dennis.robinson@hackney.gov.uk)
- **Soraya Clarke**, Delivery Manager at London Borough of Hackney (soraya.clarke@hackney.gov.uk)

[docker-download]: https://www.docker.com/products/docker-desktop
[mt-ca]: https://github.com/madetech/clean-architecture
[made-tech]: https://madetech.com/
[dotnet-ca]: https://github.com/madetech/dotnet-ca
[Endpoints]: docs/endpoints.md

