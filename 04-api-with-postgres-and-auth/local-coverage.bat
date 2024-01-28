 dotnet test --settings .\tests.runsettings
 
 reportgenerator -reports:".\**\TestResults\**\coverage.cobertura.xml" -targetdir:"coverage" -reporttypes:Html
 
 .\coverage\index.htm