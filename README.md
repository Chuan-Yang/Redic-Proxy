# Redis-Proxy

## Overview
![image](https://user-images.githubusercontent.com/22019482/164131043-ac081232-181f-492e-a195-218e35f8f3f0.png)

This project builds a redis proxy with cache. The high-level architecure looks like the diagram above. When a request comes in, the server will:
  1. Check if we have anything in the cache. If so, we will directly use the value stored under cache. If not, we will continue with step 2
  2. Talk with Redis database to get the value using the key in the request. If we didn't find the corresponding value, return 404 Not Found to the client. Otherwise, return the value to the client, and continue with step3
  3. Update the value we find in Redis database into our cache
  4. Finally, we will return the value back to the service consumer, either a string value with 200 ok or a 404 not found bad request.

## Code explanation

### RedisProxy.Service

This is our main service project, which includes the functionality of the project. It consists of three main parts:
  - Setup(Program.cs)
    - This sets up all the needed services, including cache and connection with redis
    - It stores the configs from our config files (appsettings) into singletons so that it can be used in the code easily 

  - Controller(DataController)
    - This is the controller which contains the only GET endpoint `GET /data/{key}` in this project
    - It uses the strategy introduced in the overview section to get the value for the passed in key
  
  - Services(Cache)
    - This is the place storing the useful services for the project. For this one, it only contains the implementation for the cache class 
    - It contains the interface `ICache`, its implementation `LRUCache`, and all the needed elements to implement the LRU cache. For the LRU cache:
      - Double linked list + Hashmap are used to implement this
      - **Time Complexity**: O(1) for both get and put operations

### RedisProxy.Service.Integation

This is the integration test for the main project. It connects to the same redis database as the main service connects to, which will generata some initial data for test purpose. This contains three main test cases:
  - Check if the 404 status is returned properly when there is no corresponding value for the key in database
  - Check if the value is returned properly when there is a value for the key
  - Check if concurrent requests are handled properly by the service.

## Run

To run the project, docker and docker-compose are required to be installed under the test environment.

### Run serivce + test

```
docker-compose up --build
```

### Run service only

```
docker-compose up --build redis-proxy-api
```

## Timeline
| Actions | Time | Details |
| --- | --- | --- |
| HTTP web service + Single backing instance | 2 hours | 1. Learnt and set up redis in the local environment 2. Created and implemented the template for the project |
| Cached GET + Global expiry + LRU eviction + Fixed key size + Configuration | 2 hours 30 mins | 1. Implemented the cache system 2. Implemented the config setup for the project |
| Concurrent processing | 3 hours | 1. Spent lots of time looking into the sequential concurrent processing using queue over API, which didn’t end really well 2. Switched to parallel concurrent processing using asynchronous operations |
| System tests | 3 hours | 1. Implemented the test cases 2. Spent lots of time figuring out a project referencing issue 3. Wasted some time setting up the project because of the editor referencing tool issue |
| Platform + Single-click build and test | 3 hours | 1. Introduced Docker and Docker Compose into the project 2. Spent some time figuring out the host network and domain issue when running the server and test in docker |
| Documentation | 1 hour |  |

## Requirements not implemented

- Sequential concurrent processing: implemented using parallel concurrent processing. But without setting any locks, so that the service will follow the FIFO rule.


