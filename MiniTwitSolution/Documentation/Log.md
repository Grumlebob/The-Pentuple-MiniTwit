# 31.01.2025

## Refactoring

### 2to3
ChatGPT was useful to understanding the commanded flags, as well as updating "Werkzeug.Security"

We also found a linux tool called "tldr", which was helpful in understanding linux commands, shorter than the tool "man"


## General usage notes


What does ./ means in front of file: The ./ at the beginning refers to the current directory.


Activate python environment:

    # Cd ind I itu-minitwit
    . venv/bin/activate
    # or
    . venv/bin/activate

Compile flag_tool file:

    gcc flag_tool.c -o flag_tool -lsqlite3

Linter for shell scripts:

    shellcheck control.sh

Connection to miniTwit with SSH:

    ssh student@104.248.24.1
    pw is: uiuiui

Flagging a tweet:

    ./flag_tool <tweet_id>

Stop server:

    ./control.sh stop


# 07.02.2025


## Refactor to C#


### First considerations

* We want to use a language, everyone in the group is familiar with. This is also the technology we want to work with professionally, and therefore to increase our skills in this technology, is valuable for us.
* We want to make it simple to work with different database vendors, therefore we use EF Core, as everyone in the group is familiar.
* We want to use tools that simplies work. Such as generating appropiate gitignores through https://www.toptal.com/developers/gitignore/
* We want to setup an empty .net solution, with 3 projects.
* We want to use the same formatting tool, and Csharpier provides a working opinionated out-of-the-box solution, which can be added to github actions aswell later.


### First .net considerations

1 solution
5 projects
* Documentation
* Client (Wasm). Scales better than server, if we are to expect the simulation to perhaps give 100k concurrent users.
* API (Minimal API). Minimal API is the latest way to work with API's in .net. It is supposedly faster than old-school controllers
* Shared. For example DTO's, as both the visual presentation and the API should use same DTO's. Thus we avoid code duplication
* Testing.

### Initial skeleton

We all worked on a single PC, to analyze the python version of MiniTwit, to decide vertical slices to be implemented in our C# version.
Thus we all agreed on the same language and file structure for the initial slices, to increase expected consistency, when working in parallel.
But in order to be certain of consistency, we decided to follow a pull-request structure, where we as a group approve of the PR.


### Testing considerations

* Testcontainers for integration testing, as to avoid using in-memory testing.


## Database

### Vendor

Current vendor is SQLite, we will initially use SQLite aswell.

But we use EF Core, which enables us to easily change vendor at a later point.

### Database scheme

As we have an existing database, we are going to use EFCore scaffolding, to get the same scheme in C#

This enables the group, to quickly have a shared "language" for this project.

### First migration

We have made a "baseline migration", which is simply empty, so we can use the existing database, but have EF Core start tracking changes.

## Github

### Kanban

We decided to use a Kanban board to organize issues, for ease of overview and monitoring progress and bottlenecks.

### Templates

For consistency, and ease of development flow.
Both for creating issues, and for pull-requests.

### Releases

First version of our release scheme:
We started from v1.0 which was the initial project we got through scp.
* Small changes increment the right digit. (For example using 2to3 on the python files went from v1.0 to v1.1).
* Large changes increment the left digit and sets the right to 0. (For example changing from python to C# took us from v1.1 to v2.0)
Whether a change is small or large is agreed upon by the group before a release.


# 10.02.2025

Adding dependencies agreed upon above:
* Configures EFCore models
* Setup for Testcontainers using Postgres
* Setup Respawn together with Testcontainers
* When running program normally, it will use SQLite
* Adds CSharpier


## Testing

The added tests are not complete. We plan to rely on the already given python tests, after
the initial first versions of the C# is being done.
The tests mostly server to see if we setup EF core relationships correctly,
such as the common tricky many-to-many cases, where we need to avoid cyclic dependencies.
