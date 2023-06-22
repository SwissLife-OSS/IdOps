# Developing in DevContainers

## In VsCode
1) Open vscode
2) Execute the command: `>Dev Containers: Open Folder in Container...`
3) Select the Folder containing the service you want to work on. 

## UI:
The docker image contains the frontend dependencies.
Make sure to rebuild the docker image with `docker compose up --build` when dependencies change. 