#!/bin/bash

# from https://github.com/itu-devops/itu-minitwit-docker-swarm-teraform/blob/master/bootstrap.sh

set -e

echo -e "\n--> Bootstrapping Minitwit\n"

echo -e "\n--> Loading environment variables from secrets file\n"
source secrets

echo -e "\n--> Checking that environment variables are set\n"
# check that all variables are set
[ -z "$TF_VAR_do_token" ] && echo "TF_VAR_do_token is not set" && exit
[ -z "$TF_VAR_docker_username" ] && echo "TF_VAR_docker_username is not set" && exit
[ -z "$TF_VAR_minitwit_db_user" ] && echo "TF_VAR_minitwit_db_user is not set" && exit
[ -z "$TF_VAR_minitwit_db_password" ] && echo "TF_VAR_minitwit_db_password is not set" && exit
[ -z "$SPACE_NAME" ] && echo "SPACE_NAME is not set" && exit
[ -z "$STATE_FILE" ] && echo "STATE_FILE is not set" && exit
[ -z "$DO_ACCESS_KEY_ID" ] && echo "DO_ACCESS_KEY_ID is not set" && exit
[ -z "$DO_SECRET_ACCESS_KEY" ] && echo "DO_SECRET_ACCESS_KEY is not set" && exit

echo -e "\n--> Initializing terraform\n"
# initialize terraform
terraform init \
    -backend-config "bucket=$SPACE_NAME" \
    -backend-config "key=$STATE_FILE" \
    -backend-config "access_key=$DO_ACCESS_KEY_ID" \
    -backend-config "secret_key=$DO_SECRET_ACCESS_KEY"

# check that everything looks good
echo -e "\n--> Validating terraform configuration\n"
terraform validate

# create infrastructure
echo -e "\n--> Creating Infrastructure\n"
terraform apply -auto-approve

echo -e "\n--> Done bootstrapping Minitwit"
echo -e "--> The dbs will need a moment to initialize, this can take up to a couple of minutes..."
echo -e "--> Site will be avilable @ http://$(terraform output -raw public_ip)"
echo -e "--> You can check the status of swarm cluster @ http://$(terraform output -raw minitwit-swarm-leader-ip-address):8888"
echo -e "--> ssh to swarm leader with 'ssh root@\$(terraform output -raw minitwit-swarm-leader-ip-address) -i ssh_key/terraform'"
echo -e "--> To remove the infrastructure run: terraform destroy -auto-approve"
