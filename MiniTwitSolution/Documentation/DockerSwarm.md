# Docker swarm

## 28.03

### Setup docker swarm

Summary: The first part of the guide basically just sets up three droplets. A manager and two workers. This could have been done on digital oceans ui. The last part registers the manager as manager and joins the workers to the manager.

We followed the following the guide: https://github.com/itu-devops/lecture_notes/blob/master/sessions/session_09/README_EXERCISE.md
to create manager droplets and workers.
There was a default droplet limit of 3, so we requested an increase by going to setting on digital ocean and scrolling down to droplet limit. This only worked with the team owner (Christian).

We already setup the variable DIGITAL_OCEAN_TOKEN, so this could be skipped.

For the CONFIG varibale where we have to insert our own fingerprint:
go to our digital ocean project on the web. Go to settings -> security. Here there is a list of ssh keys and their fingerprints. Use your own.

## 01.04

### Droplets manual setup

We decided to use a swarm for our API. It consists of three droplets: swarm-manager, worker1 and worker2.
Each api droplet should also get the migrator. 
Then there is a separate droplet for each of: the database, client and seq.
So 6 droplets in total.

After making the droplets. from root
add env varibalbes that are used in the docker compose for that droplet. (e.g. DOCKER_USERNAME)
The values are in the discord resources chat.
We added them by editing the root/.bashrc file with 
```bash
export VAR="value"
export VAR2="value2"
#...
```

Then after saving and exiting the file
```bash
source ./.bashrc
```

we copy the relevant remote files to the droplets with scp
```bash
scp -i ~/.ssh/do_ssh_key -r "path\to\file" root@DROPLET_IP:~/minitwit
# the path works on powershell. On other you might need backslashes or without quotation or whatever
```

The files are **setup.sh** in the remote files folder. It installs docker and docker-compose. 
and **deploy.sh** + **docker-compose.yml** in the relevant droplet folder.

run the setup file with 
```bash
chmod +x setup.sh # execute permission
./setup.sh
```
then run the deploy file with
```bash
bash -x deploy.sh
```





(we plan to automate this for ci/cd)
