# IP Batch API

## Project Overview

This project implements a comprehensive IP information retrieval and management system as outlined in the [C# Project PDF](https://github.com/Theo011/IP-Batch-API/blob/main/C%23_Project.pdf). It consists of three main components:

1. Dynamic Link Library (DLL)
2. WebApi
3. WebApi Batch Request Job

## Components

### 1. Dynamic Link Library (DLL)

The DLL encapsulates the logic for communicating with the IPStack API.

Key Features:
- Implements the `IIPInfoProvider` interface
- Provides a `GetDetails(ip)` method to retrieve IP information
- Handles API communication exceptions
- Throws a custom `IPServiceNotAvailableException` when necessary

### 2. WebApi

A C# .NET WebApi that utilizes the DLL to serve IP details.

Key Features:
- Exposes an endpoint to get details for a specific IP
- Implements caching using .NET MemoryCache
  - Cache items expire after one minute
- Uses Entity Framework and Microsoft SQL Server for data persistence
- Optimizes performance by retrieving data from cache or database before calling the external API

### 3. WebApi Batch Request Job

An enhanced module of the WebApi that supports batch operations for updating IP details.

Key Features:
- Provides an endpoint for submitting batch update requests
  - Accepts an array of IP details to be updated
  - Returns a GUID for job tracking
- Implements a job progress tracking system
  - Allows checking job status using the provided GUID
- Processes submitted items in batches of 10
