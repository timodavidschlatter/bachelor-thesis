# eBauGISTriage

Das Repository beinhaltet einen .NET web service. Dieser führt die GIS-Triage per request aus und liefert die Resultate als response zurück. Das Projekt ist Teil vom eBau-Projekt. 

## Dokumentation

TODO

## Building

Das Projekt kann mit den folgenden Befehlen in der Powershell gebuildet werden. Dafür muss man sich im Projektordner "eBauGISTriage" befinden.

``` Powershell
dotnet clean
dotnet build
```

## Testing

Das Projekt benutzt Unit Tests (Projekt eBauGISTriageUnitTests) und Integration Tests (Projekt eBauGISTriageIntegrationTests).

**Tests ausführen**

1. Powershell öffnen
2. Im Powershell in den Projektordner "eBauGISTriage" wechseln
3. Befehl ```dotnet test``` ausführen

## Deploying (CI/CD)

Das Repository benutzt das Gitlab CI/CD für das automatischen testen, builden und deployen auf die Test- und produktive Umgebung. 

Weitere Informationen zu CI/CD Variablen, JSON- und YAML-Files: https://git.bl.ch/bud-it/buildscriptlibrary

Die Pipeline wird nicht bei jedem commit ausgeführt, sondern bei jedem neuen Tag. Sie kennt drei stages: test, pack und deploy.

1. Neuen Tag erstellen (Naming convention: Major.Minor.Bug, example: 1.1.0)
2. Die Pipeline wird gestartet. 
3. Stage "test" wird gestartet. Es werden alle unit und integration tests ausgeführt. Schlägt ein Test fehl, bricht die Pipeline ab.
4. Stage "pack" wird gestartet. Hier werden zwei Jobs für die zwei Umgebung ausgeführt. Beide builden und paketieren die Applikation. 
5. Stage "deploy" muss manuell gestartet werden. Wenn es vorgängig keine Fehler gab, können die Applikationen auf die entsprechenden Server kopiert werden. 
