# CI/CD

## 26.02.2025

We followed this guide: https://github.com/itu-devops/itu-minitwit-ci

### Vagrant

The vagrantfile handles creating a droplet on digital ocean with our application synced to a "vagrant" folder
and our "remote-files" folder synced to a "minitwit" folder, both in the droplet.
It is run using vagrant up. But this should not be necessary in the future once the droplet is running.

### Docker compose files

We have a "local" docker-compose file, which is allows us to run docker compose up on our local machine.
We have another docker-compose file in the remote-files folder which our droplet uses when run github actions.
It gets the newest images from Jacob's docker hub and runs the application.


### Github Actions

Other than code scanning and analysis we have a build and test which runs on pull request and push to main.
Then we have a deploy action which runs after the build and test if it is a push to main.
The deploy action updates our docker images on Jacob's docker hub and then ssh into the droplet and runs deploy.sh
which uses the docker-compose file in the remote-files folder to update the application.
The github actions uses github secrets that has information about the host (droplet ip), user (root) 
and Jacobs private do_ssh_key.


### Migrations

Currently handled in the Dockerfile.migrator, which creates a migration-bundle. 
It then runs the migrate.sh file, which runs the migration. 
(For future clean up we could handle everything in the Dockerfile, and discard the .sh file.)


### GitHub Actions

We took earlier github actions made in a previous project, for a fast head-start on the setup.
So we are prepared for future lecture, where github actions is mentioned

## 18.02.2025

### Static tools

Added codefactor, github code scan.

Mostly for gaining experience, as to prepare ourselves for the future lecture about static tools.


## 14.03.2025

### Automated code formatting

The codeql GitHub action now runs Csharpier on code in pull requests and 
pushes the changes, so we don't have to remember to do it. 
This means it happens behind the scenes, and we assume it does not do
something crazy that we wouldn't want on our main branch.

## 25.04.2025

### Terraform

We started on the terraform setup using this guide:
https://github.com/itu-devops/itu-minitwit-docker-swarm-teraform?tab=readme-ov-file
We did it on our own repository.

Instead of 1 leader 2 managers and 3 workers 
we use 1 leader/manager and 2 workers.

The leader droplet is the one calling ```docker swarm init```.
The leader then calls ```docker swarm join-token worker``` 
which creates a token that worker droplets can call ```docker swarm join``` with.
To add more managers to the swarm the leader can make a manager token
```docker swarm join-token worker``` but since we use the leader as manager 
we haven't used this.

there is a script for the droplet swarm (minitwit-api-swarm.tf), 
one for terraform vars (minitwit.auto.tfvars)
and a provider file (provider.tf) where variables are published.



