# IP Batch API

Implementation of the [C# Project PDF](https://github.com/Theo011/IP-Batch-API/blob/main/C%23_Project.pdf).

This C# project consists of three parts: (1) a Dynamic Link Library (DLL) that interfaces with the IPStack API, sending requests, parsing responses, and returning IP details while managing exceptions and issuing a custom "IPServiceNotAvailableException" as needed; (2) a C# .Net WebApi utilizing the DLL to serve IP details, featuring an optimization layer of caching (with .Net MemoryCache) and a repository (using Entity Framework and Microsoft SQL Server) for storing and retrieving IP details, minimizing redundant API calls, and managing cache expiration every minute; (3) an enhanced WebApi module that supports batch updates of IP details, processing in batches of 10, with a post request API for submitting IP details and a tracking mechanism using a returned GUID, offering flexibility in its implementation.
