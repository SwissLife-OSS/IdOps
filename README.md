IdOps is a multi-tenant Identity Server resource management platform.

## Getting Started
A local environment can be started using the `tye.yaml` configuration from the repository root.

- first install all node_modules needed for the UI
```powershell
yarn --cwd .\src\UI\
```
- then start the environment
```powershell
tye run
```
This will build and start the followings:
- IdOps UI & Server
- 2 x Identity Servers
- MongoDB
- RabbitMq
## Features
Work in Progress
## Community

This project has adopted the code of conduct defined by the [Contributor Covenant](https://contributor-covenant.org/)
to clarify expected behavior in our community. For more information, see the [Swiss Life OSS Code of Conduct](https://swisslife-oss.github.io/coc).
