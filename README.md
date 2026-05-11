# E-commerce Database Benchmark: SQL Server vs MongoDB

Exam project for the 2025 **Database for Developers** course.

This project benchmarks **SQL Server** and **MongoDB** in a simple e-commerce order system with focus on **performance** and **data modeling**. The purpose of the project is to investigate how the two database technologies differ across realistic use cases, rather than assuming that one database is always faster than the other.

## Benchmark Results

![Benchmark results comparing SQL Server and MongoDB across the four use cases](docs/images/benchmark-results.png)

*Figure 1. Average response time in milliseconds for SQL Server and MongoDB across UC1-UC4.*

## Problem Statement

How do SQL Server and MongoDB differ when benchmarking central use cases in a simple e-commerce order system, with focus on performance and data modeling?

The project investigates these questions:

- How do SQL Server and MongoDB perform when creating an order with multiple order items?
- How does performance differ when retrieving an order with related data?
- How do paging and indexing affect performance when retrieving a customer’s order history?
- How well do SQL Server and MongoDB handle aggregating queries for analysis of top-selling products?

## Project Overview

The benchmark is based on four realistic use cases:

- **UC1 - Place Order**  
  Measures write performance when creating an order with 1, 3, and 10 order items.

- **UC2 - Get Order Details**  
  Measures read performance when retrieving a single order together with customer data, order items, and product information.

- **UC3 - Customer Order History**  
  Measures paged reads for a customer’s order history, including the effect of paging depth and indexing.

- **UC4 - Top-selling Products**  
  Measures aggregating queries used for analytical reporting.

The project uses a shared application layer and a **ports-and-adapters architecture**, so the same business logic is used for both databases. This makes the comparison more fair, since differences in results are mainly related to the database choice, data model, and query patterns.

## Method

The project was carried out using a practical and empirical approach.

- A controlled benchmark environment was implemented
- SQL Server and MongoDB were tested against the same use cases
- Data was generated using a deterministic seeder with a fixed seed value
- Both databases were populated with equivalent datasets
- Benchmarks were executed on a **medium profile** with:
  - **1,000 customers**
  - **1,000 products**
  - **10,000 orders**
- Relevant indexes were created in both databases
- Each use case was benchmarked with **30 iterations per run**
- **3 separate runs** were performed for each database
- Response times were measured in milliseconds
- Results were summarized using **average response time** and **standard deviation**
- A warm-up phase was performed before each benchmark to reduce startup effects

## Benchmark Environment

Benchmarks were executed on a local development machine with:

- **Intel Core i7-11700**
- **32 GB DDR4-3200 RAM**
- **Windows 11**

Both **SQL Server** and **MongoDB** were run locally in **Docker** to ensure isolated and comparable environments.

## Key Findings

### UC1 - Place Order
MongoDB performed better than SQL Server when creating orders with multiple order items.

This is mainly explained by the difference in data modeling:

- In **SQL Server**, orders and order items are stored in separate tables with foreign keys, constraints, indexes, and transactions
- In **MongoDB**, the order and its order items are stored together as a single document

As the number of order items increased, SQL Server showed higher write overhead, while MongoDB remained more efficient and generally more stable.

### UC2 - Get Order Details
SQL Server performed better than MongoDB when retrieving an order with related data.

This use case benefits from SQL Server’s relational model:

- SQL Server can retrieve related data efficiently using joins
- MongoDB required additional lookups and more assembly work in the application layer

This made SQL Server faster in a read-heavy scenario with more complex relationships.

### UC3 - Customer Order History
Both databases performed well for shallow paging, but SQL Server showed a tendency toward higher response times at deeper paging levels.

This is likely related to paging strategy:

- SQL Server used offset-based paging with `Skip`/`Take`
- MongoDB used `skip`/`limit`

At higher paging depth, SQL Server appeared more affected, while MongoDB maintained a more stable profile in this setup.

### UC4 - Top-selling Products
Both databases handled the analytical query effectively, but MongoDB achieved lower average response times.

This can be explained by MongoDB’s aggregation pipeline working directly on embedded order items, while SQL Server had to aggregate across normalized tables and joins.

## Conclusion

The project shows that **neither SQL Server nor MongoDB is universally superior across all use cases**.

The benchmark results indicate that performance depends heavily on:

- the specific use case
- the chosen data model
- the query pattern
- consistency requirements

In this project:

- **MongoDB** performed better in the write-heavy and some analytical scenarios
- **SQL Server** performed better in the relational read-heavy scenario
- Paging performance depended strongly on implementation strategy and indexing

Overall, the project demonstrates that database selection should be based on **concrete system requirements**, not on general assumptions about speed.

## Technologies Used

- **C#**
- **.NET**
- **SQL Server**
- **MongoDB**
- **Docker**
- **Mongo Express**

## Architecture

The repository follows **Clean Architecture** and is structured as a layered .NET solution with separate folders for:

- `Api`
- `Application`
- `Domain`
- `Infrastructure`
- `Migrations`

This structure supports separation of concerns and makes it easier to benchmark the same use cases across different database implementations.

## Getting Started

This project uses:

- Docker Compose for SQL Server and MongoDB containers
- .NET user secrets for application credentials
- Postman to seed data and run benchmarks

### 1. Configure Docker Compose
Create a `.env` file in the repository root.

Example:

```env
# Mongo
MONGO_ROOT_USERNAME=your-mongo-username
MONGO_ROOT_PASSWORD=your-chosen-password
MONGO_DATABASE=EcommerceBenchmark

# SQL Server
SA_USER=sa
SA_PASSWORD=your-chosen-password
```

### 2. Configure application secrets
Set the secrets used by the .NET application

Example:
```
dotnet user-secrets set "SA_PASSWORD" "your-chosen-password"
dotnet user-secrets set "MONGO_USERNAME" "your-mongo-username"
dotnet user-secrets set "MONGO_PASSWORD" "your-chosen-password"
```
   
### 3. Start the databases
`docker compose up -d --build`
  
### 4. Run the API
`dotnet run`

### 5. Apply SQL Server migrations
`dotnet ef database update`
   
### 6. Use Postman
Use Postman to call the four endpoints:

1. Seed SQL Server
2. Run SQL Server benchmark
3. Seed MongoDB
4. Run MongoDB benchmark

Example:

```
http://localhost:5178/benchmark/seed?provider=sql&profile=medium&seed=42
http://localhost:5178/benchmark/run?provider=sql&iterations=30&customerId=195
http://localhost:5178/benchmark/seed?provider=mongo&profile=medium&seed=42
http://localhost:5178/benchmark/run?provider=mongo&iterations=30&customerId=195
```
