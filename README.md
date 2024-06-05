# Redesign and Optimization of the GIS Triage Process for Construction Permit Evaluations

## Introduction

The Bau- und Umweltschutzdirektion BL provides the convenience of electronic building application submissions. However, the software supporting this process has gradually become outdated and challenging to maintain. The main focus of this bachelor thesis is to redesign the GIS triage process within the software, aiming to tackle these issues and drive improvements.

## Result

The outcome of the project is a standalone .NET web service that offers seamless integration through HTTP requests. Results are returned to the requester in JSON format.
Notably, the new service is much faster compared to its predecessor, primarily attributed to its extensive utilization of parallelization techniques. Furthermore, the enhanced service greatly facilitates maintenance and expansion efforts. Leveraging the open source “Microsoft Rules Engine”, the implementation of triage rules is now simpler, especially when compared to the previous reliance on a database trigger.

## Folder Structure
