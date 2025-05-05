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
    timeout = "2m"
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