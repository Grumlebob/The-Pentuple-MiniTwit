# credit: https://github.com/itu-devops/itu-minitwit-docker-swarm-teraform
resource "digitalocean_droplet" "minitwit-swarm-leader" {
  image = "docker-20-04" // ubuntu-22-04-x64
  name = "api-swarm-leader"
  region = var.region
  size = "s-1vcpu-1gb"
  ssh_keys = [
    for key in data.digitalocean_ssh_key.team : key.fingerprint
  ]

  # specify a ssh connection
  connection {
    user = "root"
    host = self.ipv4_address
    type = "ssh"
    private_key = file(var.pvt_key)
    timeout = "5m"
  }

  # ensure Terraform waits long enough for the droplet to be "created"
  timeouts {
    create = "10m"
  }

  provisioner "file" {
    content = <<-EOT
      export DOCKER_USERNAME=${var.docker_username}
      export MINITWIT_DB_USER=${var.minitwit_db_user}
      export MINITWIT_DB_PASSWORD=${var.minitwit_db_password}
    EOT

    destination = "/root/.bash_profile"
  }

  provisioner "remote-exec" {
    inline = [
      "mkdir -p /root/minitwit",
      "mkdir -p /root/migrator",
    ]
  }

  provisioner "file" {
    source = "remote_files/api-swarm/docker-compose.yml"
    destination = "/root/minitwit/docker-compose.yml"
  }
  
  provisioner "file" {
    source = "remote_files/api-swarm/deploy.sh"
    destination = "/root/minitwit/deploy.sh"
  }
  
  # the leader node is also responsible for doing migrations
  provisioner "file" {
    source = "remote_files/migrator/docker-compose.yml"
    destination = "/root/migrator/docker-compose.yml"
  }
  
  provisioner "file" {
    source = "remote_files/migrator/doMigration.sh"
    destination = "/root/migrator/doMigration.sh"
  }

  provisioner "remote-exec" {
    inline = [
      # allow ports for docker swarm
      "ufw allow 2377/tcp",
      "ufw allow 7946",
      "ufw allow 4789/udp",
      # ports for apps
      "ufw allow 80",
      "ufw allow 8080",
      "ufw allow 8888",
      # SSH
      "ufw allow 22",

      # initialize docker swarm cluster
      "docker swarm init --advertise-addr ${self.ipv4_address}",
    ]
  }
}

resource "null_resource" "swarm-worker-token" {
  depends_on = [digitalocean_droplet.minitwit-swarm-leader]

  # save the worker join token
  provisioner "local-exec" {
    command = <<EOS
      ssh -o 'StrictHostKeyChecking no' 
        root@${digitalocean_droplet.minitwit-swarm-leader.ipv4_address} 
        -i ${var.pvt_key} 
        'docker swarm join-token worker -q' > temp/worker_token
      EOS
  }
}

#                     _
# __      _____  _ __| | _____ _ __
# \ \ /\ / / _ \| '__| |/ / _ \ '__|
#  \ V  V / (_) | |  |   <  __/ |
#   \_/\_/ \___/|_|  |_|\_\___|_|
#
# create cloud vm
resource "digitalocean_droplet" "minitwit-swarm-worker" {
  # create workers after the leader
  depends_on = [null_resource.swarm-worker-token]

  # number of vms to create
  count = 2

  image = "docker-20-04"
  name = "api-swarm-worker-${count.index}"
  region = var.region
  size = "s-1vcpu-1gb"
  # add public ssh key so we can access the machine
  ssh_keys = [
    for key in data.digitalocean_ssh_key.team : key.fingerprint
  ]

  # specify a ssh connection
  connection {
    user = "root"
    host = self.ipv4_address
    type = "ssh"
    private_key = file(var.pvt_key)
    timeout = "5m"
  }

  # ensure Terraform waits long enough for the droplet to be "created"
  timeouts {
    create = "10m"
  }

  provisioner "file" {
    source = "temp/worker_token"
    destination = "/root/worker_token"
  }

  provisioner "remote-exec" {
    inline = [
      # allow ports for docker swarm
      "ufw allow 2377/tcp",
      "ufw allow 7946",
      "ufw allow 4789/udp",
      # ports for apps
      "ufw allow 80",
      "ufw allow 8080",
      "ufw allow 8888",
      # SSH
      "ufw allow 22",

      # join swarm cluster as workers
      "docker swarm join --token $(cat worker_token) ${digitalocean_droplet.minitwit-swarm-leader.ipv4_address}"
    ]
  }
}

# Deploy swarm application after droplets are created

resource "null_resource" "swarm-deploy" {
  # wait for leader, workers and db to be created
  depends_on = [digitalocean_droplet.minitwit-swarm-leader, 
  digitalocean_droplet.minitwit-swarm-worker, 
  digitalocean_droplet.db-droplet]
  
  # this trigger will re-run this if you we change the count/addresses of workers (gpt credit)
  triggers = {
    worker_ips = join(",", digitalocean_droplet.minitwit-swarm-worker.*.ipv4_address)
  }

  connection {
    type        = "ssh"
    user        = "root"
    host        = digitalocean_droplet.minitwit-swarm-leader.ipv4_address
    private_key = file(var.pvt_key)
    timeout     = "5m"
  }

  provisioner "remote-exec" {
    inline = [
      # strip any CRLF that snuck in on the way
      "apt-get update && apt-get install -y dos2unix",
      "dos2unix /root/migrator/doMigration.sh",
      "dos2unix /root/minitwit/deploy.sh",

      # run migration
      "cd /root/migrator",
      "chmod +x doMigration.sh",
      "bash -x doMigration.sh",
      
      # run api swarm
      "cd /root/minitwit",
      "chmod +x deploy.sh",
      "bash -x deploy.sh"
    ]
  }
}


output "minitwit-swarm-leader-ip-address" {
  value = digitalocean_droplet.minitwit-swarm-leader.ipv4_address
}

output "minitwit-swarm-worker-ip-address" {
  value = digitalocean_droplet.minitwit-swarm-worker.*.ipv4_address
}