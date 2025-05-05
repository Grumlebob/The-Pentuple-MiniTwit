# credit: https://github.com/itu-devops/itu-minitwit-docker-swarm-teraform
resource "digitalocean_droplet" "db-droplet" {
  image = "docker-20-04" // ubuntu-22-04-x64
  name = "db"
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
      export MINITWIT_DB_USER="${var.minitwit_db_user}"
      export MINITWIT_DB_PASSWORD="${var.minitwit_db_password}"
    EOT

    destination = "/root/.bash_profile"
  }

  provisioner "remote-exec" {
    inline = [
      "mkdir -p /root/minitwit",
    ]
  }

  provisioner "file" {
    source = "remote_files/db/docker-compose.yml"
    destination = "/root/minitwit/docker-compose.yml"
  }
  
  provisioner "file" {
    source = "remote_files/db/deploy.sh"
    destination = "/root/minitwit/deploy.sh"
  }

  provisioner "remote-exec" {
    inline = [
      # SSH
      "ufw allow 22",

      # strip any CRLF that snuck in on the way
      "apt-get update && apt-get install -y dos2unix",
      "dos2unix /root/minitwit/deploy.sh",

      # deploy db application
      "cd /root/minitwit",
      "chmod +x deploy.sh",
      "bash -x deploy.sh",
    ]
  }
}

output "minitwit-db-ip-address" {
  value = digitalocean_droplet.db-droplet.ipv4_address
}