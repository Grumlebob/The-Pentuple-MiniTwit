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
