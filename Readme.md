# Basket API
[![Build Status](https://travis-ci.org/jhgann/Basket.API.svg?branch=master)](https://travis-ci.org/jhgann/Basket.API)

## Introduction
This is a prototype of a shopping basket microservice.  It demonstrates an outside client using HTTP requests to interact with a shopping basket.  It also demonstrates interservice asynchronous communication using a message broker.

## What's included
- **Basket.API** - The prototype microservice 
- **ClientApp** - A client that makes requests to the Basket API, and posts price changes to the message broker.
- **EndToEndTests** - A testing project that tests all endpoints of the API.
- **EventBusCore** - Holds some basic shared code for the event bus.
- **RabbitMQEventBus** - A rabbitmq event bus implementation.  Used by ClientApp to send price changes and by Basket.API to recieve them.

## Running the prototype
This solution was developed on Windows 10, using:
- Visual Studio 15.7.4
- .NET Core SDK 2.1.301
-  Docker 18.03.1 using Linux containers.
-  RabbitMQ 3.7.6 from Docker.

Loading the project in Visual Studio and debugging with Docker should load the Basket API swagger page.  From there the API can be interacted with.

#### Urls for the running services:
- Basket API - http://localhost:55311/swagger/index.html
- Client App - http://localhost:55322/swagger/index.html
- RabbitMQ - http://localhost:15672/#/
  - Management page for monitoring.
  - Username: guest
  - Password: guest

## Current issues
As I do not have a lot of experience with Docker and RabbitMQ, I encounted the following issues:
- HTTPS is not enabled for the service.  I could run all services using HTTPS but the ClientApp was unable to send requests to the Basket API.  It is very likely I simply have a docker compose issue.
- I need to restart Docker once after booting into Windows.
- On the first run, RabbitMQ fails to connect once or twice.  It succeeds after that.  I'm assuming it is simply not up and running yet.
- The testing project disables the event bus, as it is usually not running when I execute tests during development.  I would like to investigate running tests using docker so I can send test messages to the queue.

## Using the API
- The swagger page for the Basket API shows all of the actions available to users of the API.
- The swagger page for the ClientApp shows how a potential user could use the API.
- The Client App also can simulate a catalog or other service changing the price of a product.  This publishes a price change event to a message broker.  The Basket API is subscribed to this broker and will update the price of this product in any basket it happens to be in.
