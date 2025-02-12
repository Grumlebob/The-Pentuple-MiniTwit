# Testing

## 12.05.2025

### Testcontainers

ISO25000 https://iso25000.com/index.php/en/iso-25000-standards/iso-25010/57-maintainability, 
under maintainability refers to testability, "Degree of effectiveness and efficiency with which test criteria can be established for a system, product or component and tests can be performed to determine whether those criteria have been met."

One of the common issues, is that some integrations tests uses mocking. Here there might be a difference between the mocked result and the actual result comming from the endpoint reaching a real database.
That is why we use Testcontainers https://testcontainers.com/.
This allows us to easily setup integration tests that uses real dependencies, like an actual database.
Testcontainers behind the scenes uses docker, which is why it can easily run through integration pipelines such as Github Actions,
which is mentioned later in the course, and why we would like to prepare for it already.

But right now there is 1 caveat.
Python uses SQLite, while Testcontainers uses Postgres.
As such we have added to program.cs

builder.Environment.EnvironmentName = "Testing";

This makes test use Postgres.
If line is outcommented, we use SQLite.

### Respawn

This helps us with dealing with concurrency and performance issues, when running tests.
In short, it allows us to re-use the same created testing database, and it will then intelligently
reset the table to a desired state between each tests. It does it so fast, that we can then
just run all the tests sequentially without performance issues, while avoiding concurrency issues.

This is also why a global 5 minutes test timer has been added.
Serving as a warning, that if very slow tests are added, they will initially fail due to the timer.
This will serve as a reminder for the person, to make sure that their own test is optimal.


### Bogus

While not implemented. We discussed a way, to generate random but valid data for our tests.
This library can do such a thing, and will most likely be implemented later.

### Using python tests on C# code, during refactoring.

skal stå noget om hvordan vi også brugte html siderne, til at ramme vores nye endpoints





## ny.dato.2025

### Ny interessant ting