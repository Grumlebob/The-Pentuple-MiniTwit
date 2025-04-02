# 31.01.2025

## Refactoring

### 2to3
ChatGPT was useful to understanding the commanded flags, as well as updating "Werkzeug.Security"

We also found a linux tool called "tldr", which was helpful in understanding linux commands, shorter than the tool "man"


## General usage notes


What does ./ means in front of file?: The ./ at the beginning refers to the current directory.


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
* We want to use tools that simplifies work. Such as generating appropriate gitignore through https://www.toptal.com/developers/gitignore/
* We want to set up an empty .net solution, with 3 projects.
* We want to use the same formatting tool, and Csharpier provides a working opinionated out-of-the-box solution, which can be added to GitHub actions as well later.


### First .net considerations

1 solution
5 projects
* Documentation
* Client (Wasm). Scales better than server, if we are to expect the simulation to perhaps give 100k concurrent users.
* API (Minimal API). Minimal API is the latest way to work with API's in .net. It is supposedly faster than old-school controllers
* Shared. For example DTO's, as both the visual presentation and the API should use same DTO's. Thus, we avoid code duplication
* Testing.

### Initial skeleton

We all worked on a single PC, to analyze the python version of MiniTwit, to decide vertical slices to be implemented in our C# version.
Thus, we all agreed on the same language and file structure for the initial slices, to increase expected consistency, when working in parallel.
But in order to be certain of consistency, we decided to follow a pull-request structure, where we as a group approve of the PR.


### Testing considerations

* Testcontainers for integration testing, as to avoid using in-memory testing.


## Database

### Vendor

Current vendor is SQLite, we will initially use SQLite as well.

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
The tests mostly server to see if we set up EF core relationships correctly,
such as the common tricky many-to-many cases, where we need to avoid cyclic dependencies.

## Docker
To run:
Go to root of director
```bash
docker-compose up --build
```
To stop:
```bash
docker-compose down
```

To open API:
http://localhost:5000/

to open Client:
http://localhost:5001/


## CodeQL

Added GitHub security scanning to pipeline called CodeQL Advanced.
Ensures we don't try to leak sensitive information, such as keys.

# 18.02.2025

## Authentication

We are currently not supporting proper authentication, so we can focus on deploying. 
We handle current user with a singleton service that relies on Blazored library localStorage.

## Ef core migrations

To make a migration, go to root (where .sln is) and run the following commands:
```bash
dotnet ef migrations add InitialPostgres --project MiniTwit.Api/MiniTwit.Api.csproj --startup-project MiniTwit.Api/MiniTwit.Api.csproj
```

To update the database, run the following command:
```bash
dotnet ef database update -p MiniTwit.Api/MiniTwit.Api.csproj -s MiniTwit.Api/MiniTwit.Api.csproj
```


# 19.02

## Struggling with migrations

Turns out the answer was: Ef Core Migration Bundles.
https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying

This was the correct command to run from root:
```bash
dotnet ef migrations bundle --project MiniTwit.Api/MiniTwit.Api.csproj --startup-project MiniTwit.Api/MiniTwit.Api.csproj --self-contained -r linux-x64 -o ef-migrations-bundle
```


## What a docker day

Docker compose up now works with migration. 
```bash
docker-compose up --build 
```


In docker-compose file:
Api has ports 5000:8080, where client has port 5001:80. 80 instead of 8080 might be some nginx thing.

In Api program.cs:
There are cors policies which allow client to connect to api.
It is currently hardcoded in appsettings.json as http//localhost:5001 (notice not https)

In Client program.cs:
It sets up the typed client again with a hardcoded baseurl to the api in appsettings.json http//:localhost:5000.

The migration happens in Dockerfile.migrator where it copies migrate.sh into the container and runs it.
The migrate.sh is a script that runs the migration. Perhaps this could be done directly.
We kept changing a "bug" where it said something like "could not find Migrations__History". 
But there is a chance that this was not a bug and would have worked regardless.

The simulator was activated afterward and we tried again. ConnectionErrors...
But the solution was just to use port 8080 instead of 80 in compose.

# 20.02

## Running docker compose
On macOS the docker compose up does not use "-" command is
```bash
docker compose up --build
```
and docker compose down is
```bash
docker compose down
```

## Problems with migrations
We had problems with outdated global tools for running the dotnet command for ef, therefore we needed to run the following command.
```bash
dotnet tool install --global dotnet-ef
```
We might need to add this to the Vagrant file...

We needed to delete the existing migrations file, as it was deprecated/incompatible with our version of dotnet. 

After deleting the file, we ran the following command:
```bash
dotnet ef migrations bundle --project MiniTwit.Api/MiniTwit.Api.csproj --startup-project MiniTwit.Api/MiniTwit.Api.csproj --self-contained -r linux-x64 -o ef-migrations-bundle
```
Which fixed the migratin issues.

## Problems with Ports/Docker
On Linux (fedoraOS):
There were several issues when docker compose. We had an error where port 5432 was used by Postgres and as such could not be used when building the project. And was subsequently changed to 5433. 

On Mac:
Another was that specifically on macOS, the port 5000 is occupied by the control center, which is a process that should not be killed.
For the client to work, we had to change its port from 8080 to 80.
Port 5432 was already occupied by postgress, this seems to be the port that postgress occupies on machines.
These were changes to (docker-compose.yml):
```yml 
...
api: 
    #...
    ports:
      - "5002:8080"

...

client:
    #...
    ports:
      - "5001:80"
...
db:
    ports:
      - "5433:5432"
```

## Problems with Vagrant

There is a problem when running vagrant up. That it get stuck 

There is a syntax error in one of the files when running vagrant up.

/.vagrant.d/gems/3.3.6/gems/vagrant-vbguest-0.32.0/lib/vagrant-vbguest/hosts/virtualbox.rb

To fix this we had to manually fix this file by removing an "s" from "exists" to "exist".

For some reason specifying the VM provider in the vagrant file does not seem to work. Instead, by explicitly specifying the provider when calling
```bash
vagrant up --provider=virtualbox
```
seems to work. We are not sure why.

We have a theory that the plugin that virtualBox uses aka. "vagrant-vbguest" when being freshly installed/or updated does not auto-update when vagrant up is being run afterward. Which is what we think is what vagrant keeps getting stuck on for some reason. 

This has been fixed by adding the following line to the vagrantFile: 
```ruby
if Vagrant.has_plugin?("vagrant-vbguest")
    config.vbguest.auto_update = false
end
```
(edit: this has solved the issue of running "vagrant up --provider=virtualbox")

Now running
```bash
vagrant up
```
works as it should.


There were also issues with syncing the project folders into the VM. As such the VM does not have the correct files for running the system. By adding the last parameter to the following command, seems to have fixed it:
```ruby 
config.vm.synced_folder ".", "/vagrant", type: "virtualbox"
```
(i.e. adding - type: "virtualbox").

Running with digital ocean
```bash
vagrant up --provider=digital_ocean
```

## Problems with Digital Ocean
There are issues with getting the configuration in our VagrantFile to work with digital ocean. As it cannot find the private ssh key. We are getting the following error:
```
DigitalOcean Provider:
* SSH private key path is required
```

We have tried several "fixes", none of them work.

# 21.02

## Problem with ssh keys with Digital ocean when running vagrant up

There is a problem that the SSH_KEY_NAME from the vagrant file cannot recognize the ID of the ssh key owner inside the ~/.zshrc file.
For other developers this might be in the ~/.bashrc file, depending on which shell you are using.

We are currently getting the ID of the users ssh key by running this command.
```bash
curl -X GET "https://api.digitalocean.com/v2/account/keys" -H "Authorization: Bearer $DIGITAL_OCEAN_TOKEN"

{"ssh_keys":[{"id":45641586,"public_key":"ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIN/CFG4HkRiyZqtCv6/cQldFE6Tavt6SoFL +ScBnUE9A adamhadoutemsamani@fedora","name":"Adam","fingerprint":"77:0e:e3:50:b2:d2:cb:81:f7:46:f3:04:c6:69:d8:70"},{"id":45641584,"public_key":"ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQCzgvxcDmnyYFhoZTcO7uvYGWaGpadYJ+1rdmSkhpTKMvqSck7gCIak5a1qh7/PPbm3GU2RqpSRaSQzKQJauiLr9zM+bGukIm9d9yHIcx+vMlrJ5Z0lddHuPb4m8BVoMN0JQLFceSooLsQ9RHhTiCxVvlybKAs0WpzKk7TfOh230fPx+yZasnnTp0rVVM0kWRfDTf+lTHX9W6339t11N3hZB2C85B6FAGNe/GwMnCx0frh47h7uhw8lcxycqWqUEfqH6geGTHWSL1iQ1G3HtIEY/BfcUWebDrOeyLSR455+vYLXvWBGU8ucQWGFttEeN/XTcra8lUbqXENDcFenM1VxuJ1vQWQRMOAzixg7mXzYtrYBgWqDSvNHqXd+e53GSiAr4g6hsafTVq7irqfa9A2WnhoIh8UihItpKOnz9ruMg5TK/GIOEH2Lqd7/xcCNGcTUXn3ldNINZxlWvG5XdtaRuvKX9yWqbhrIeJFuuW07+EsDoGrz3ZjbF2FmBaPMbRaewpFarQlZARpNszAzLiVykMJgyCXEN7OI0E3iigRwUrpNAV0Lbf9Z9ambTZOD9yWU3jk7+kZUn5eZBJNdGCWPrNYoGG7ZPrTXtF34qykiMil6tnPGUYuv7RDcA0xBYZNSjJ7pv2j2Q+dxjXGUkuxIC+tKJ5Sp80bYTIxViHEqsQ== chbl@itu.dk\n","name":"Christian Lauridsen","fingerprint":"91:cd:2f:4d:f6:8f:e6:95:b1:a1:d2:05:be:ad:0b:c5"}],"links":{"pages":{}},"meta":{"total":2}}
```

**FIX**: Instead of writing the ID of the ssh key owner, give it the assigned name of the ssh key from Digital ocean in the ~/.zshrc file.

Changes made in local file ~/.zshrc
```bash
#old
export SSH_KEY_NAME="45641584"

#updated
export SSH_KEY_NAME="Christian Lauridsen"
```

## Considerations with ef migrations bundle

To generate the ef migrations bundle the following command needs to run inside shell provisioning setup in MiniTwitSolution/Vagrantfile:

```bash
dotnet ef migrations bundle --project MiniTwit.Api/MiniTwit.Api.csproj \\
        --startup-project MiniTwit.Api/MiniTwit.Api.csproj --self-contained -r linux-x64 -o ef-migrations-bundle
```

Instead of having to run this command inside the Vagrantfile, we want this command to be executed in another Docker container (either using the API or migrations docker container, using API makes sense since it contains the Migrations folder).

## Problem with droplets in digital ocean

Each time running the following command:
```bash
vagrant up --provider=digital_ocean
```
A new droplet is created

Using the command:
```bash
vagrant destroy
```
Does not destroy the droplet in digital ocean.

Fix. Using this command destroys the droplet in digital ocean:
```bash
vagrant destroy -f
```

# 28.02"

## Secure database

Changed user and password for the database (before was "postgres" "postgres").
To implement it a vagrant update was needed. 

In the vagrant file 4 environment variables are added to the droplet. 2 docker and 2 postgres

# 04.03

## disabled simulator

We disabled our own simulator now that the course simulator is running.
Enable it by going to the simulator Dockerfile and uncommenting the last line, that runs the simulator script.

# 07.03

## Add automated releases with GitHub actions

In the pull request template there are the steps you need to check when creating a release.

The latest commit **must** contain the commit message:

```txt
Release: x.y (replace x and y with version numbers)
```

Update the changelog to make sure that the release gets the correct tag and description

# 18.03

## Run shellcheck on all .sh files in solution.
The only unresolved line is 
```bash
source ~/.bash_profile
```
It is a warning that the shellcheck cannot check it because it cannot resolve the path.

## Docker linter

Used hadolint by pasting docker files into the page https://hadolint.github.io/hadolint/

Used ```docker-compose config``` to lint the docker-compose file.
Then used dclint (https://github.com/zavoloklom/docker-compose-linter)
to look for further suggestions. It wants a certain order on fields 
and says that services like api, client etc. should be alphabetically ordered.
However, this doesn't seem intuitive. Instead, we ordered them based on dependencies.
It also complains about ports being exposed without specifying an IP-address. 
But this would require hard-coded IP's all over.

## Vagrant validate

This command found nothing wrong with our script.

## Versions

Replaced ":latest" with ":<version>" in docker and docker-compose files.
The migrator uses dos2unix. To get the latest version you can open the image it uses:
```bash
docker run --rm -it mcr.microsoft.com/dotnet/runtime-deps:9.0 bash
```
Then check the latest version like this:
```bash
apt-get update
apt-cache policy dos2unix
```

Here we also discovered that bash was already installed in the image.
And therefore the bash is now not getting installed in our script.

Our own docker images currently does not use versioning. 
We just give them the tag latest. 
This is the easiest since we have continuous deployment.
To change this we need to edit the deploy.yaml script in GitHub workflows.

# 02.04

The migration dockerfile has been updated to run once and then terminate. Meaning we cannot do health checks on it. 
The local docker-compose has been updated to reflect this. However it doesn't guarantee that a migration is run before the api. This needs to be looked at. 
