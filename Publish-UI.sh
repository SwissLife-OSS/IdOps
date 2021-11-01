rm -rf  ./src/UI/dist/
yarn --cwd ./src/UI
yarn --cwd ./src/UI build
rm -rf ./src/Server/src/AspNet/UI
mkdir ./src/Server/src/AspNet/UI/
cp -rf ./src/UI/dist/* ./src/Server/src/AspNet/UI
