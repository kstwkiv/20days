# Sprint workspace documentation

## Overview
This folder contains the codebase for the NoCap Eats sprint project. The primary application lives in the nested NoCapEats directory and represents a .NET microservices platform for a restaurant ordering and delivery workflow.

The solution includes:

- API Gateway for centralized routing and authentication enforcement
- Identity service for users, JWTs, and refresh tokens
- Catalog service for restaurants, menu items, categories, and image storage
- Order service for placing and tracking orders
- Delivery service for assigning and updating deliveries
- Notification service for event-driven communication and logs
- Shared event contracts used for cross-service messaging
- Automated tests for key service behaviors

## Folder layout

- NoCapEats/: main source code for the application and its testing projects
- docs/: project documentation index and supporting notes

## Project summary
The application follows a modular, service-oriented architecture. Each service owns a database, application logic, and API surface. A shared infrastructure layer enables local development through Docker and the use of SQL Server, RabbitMQ, and Azurite.

## Service boundaries

### Gateway
The gateway exposes a single entry point and forwards requests to downstream microservices while validating JWT tokens.

### Identity
The Identity service handles account creation, login, token issuance, refresh operations, and revocation.

### Catalog
The Catalog service manages restaurants, menu categories, and menu items, along with image storage integration.

### Order
The Order service manages order placement, status transitions, and query operations for the customer and restaurant workflows.

### Delivery
The Delivery service manages assignment and progress tracking for delivery agents.

### Notification
The Notification service reacts to events from other services and records or emits notifications.

## Development lifecycle

1. Infrastructure is started through Docker Compose.
2. Each service configures its own database, JWT settings, and RabbitMQ connection.
3. APIs expose endpoint contracts for business operations.
4. Application handlers enforce business rules and validation.
5. Domain entities protect state invariants and acceptable transitions.
6. Events are published to RabbitMQ for downstream consumers.

## Documentation note
This workspace is documented in detail in docs/FILE_INDEX.md, which captures a file-by-file map of the codebase and gives each group of files a concise explanation.
