dotnet tool restore

dotnet CodeFileSanity
dotnet jb inspectcode "Circle.Desktop.slnf" --output="inspectcodereport.xml" --caches-home="inspectcode" --verbosity=WARN
dotnet nvika parsereport "inspectcodereport.xml" --treatwarningsaserrors

exit $LASTEXITCODE