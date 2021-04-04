dotnet build Pokerhands.csproj
Start-Process powershell -ArgumentList "dotnet run PokerHands.csproj"
Start-Process "https://localhost:5001/"