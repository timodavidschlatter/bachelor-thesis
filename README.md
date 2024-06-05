# Redesign and Optimization of the GIS Triage Process for Construction Permit Evaluations

## Introduction

The Bau- und Umweltschutzdirektion BL provides the convenience of electronic building application submissions. However, the software supporting this process has gradually become outdated and challenging to maintain. The main focus of this bachelor thesis is to redesign the GIS triage process within the software, aiming to tackle these issues and drive improvements.

## Result

The outcome of the project is a standalone .NET web service that offers seamless integration through HTTP requests. Results are returned to the requester in JSON format.
Notably, the new service is much faster compared to its predecessor, primarily attributed to its extensive utilization of parallelization techniques. Furthermore, the enhanced service greatly facilitates maintenance and expansion efforts. Leveraging the open source “Microsoft Rules Engine”, the implementation of triage rules is now simpler, especially when compared to the previous reliance on a database trigger.

## Folder Structure

- [doc](https://github.com/timodavidschlatter/bachelor-thesis/tree/main/doc): Contains the documentation and the abstract of the thesis
- [eBauGISTriage](https://github.com/timodavidschlatter/bachelor-thesis/tree/main/eBauGISTriage): Contains all the written code in C#
  - [eBauGISTriageApi](https://github.com/timodavidschlatter/bachelor-thesis/tree/main/eBauGISTriage/eBauGISTriageApi)
    - [Controllers](https://github.com/timodavidschlatter/bachelor-thesis/tree/main/eBauGISTriage/eBauGISTriageApi/Controllers): The TriageController to handle the HTTP requests
    - [Data](https://github.com/timodavidschlatter/bachelor-thesis/tree/main/eBauGISTriage/eBauGISTriageApi/Data): The GIS queries and the rules engine workflows
    - [Helper](https://github.com/timodavidschlatter/bachelor-thesis/tree/main/eBauGISTriage/eBauGISTriageApi/Helper): DTOs, exceptions, functions, OpenAPI request and response examples
    - [Models](https://github.com/timodavidschlatter/bachelor-thesis/tree/main/eBauGISTriage/eBauGISTriageApi/Models)
    - [Persistence](https://github.com/timodavidschlatter/bachelor-thesis/tree/main/eBauGISTriage/eBauGISTriageApi/Persistence): Database context and repositories (Repository Pattern)
    - [Services](https://github.com/timodavidschlatter/bachelor-thesis/tree/main/eBauGISTriage/eBauGISTriageApi/Services): The service classes (QueriesService and TriageService) with the main workload
  - [eBauGISTriageIntegrationTests](https://github.com/timodavidschlatter/bachelor-thesis/tree/main/eBauGISTriage/eBauGISTriageIntegrationTests)
  - [eBauGISTriageTests](https://github.com/timodavidschlatter/bachelor-thesis/tree/main/eBauGISTriage/eBauGISTriageTests)