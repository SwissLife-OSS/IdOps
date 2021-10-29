Remove-Item  .\src\UI\dist\ -Recurse -ErrorAction Ignore
yarn --cwd .\src\UI
yarn --cwd .\src\UI build
Remove-Item .\src\Server\src\AspNet\UI -Recurse -ErrorAction Ignore
New-Item -ErrorAction Ignore -ItemType "directory" -Path .\src\Server\src\AspNet\UI\
Copy-Item .\src\UI\dist\* .\src\Server\src\AspNet\UI -Recurse -Force
