# credit: https://github.com/itu-devops/itu-minitwit-docker-swarm-teraform
resource "digitalocean_ssh_key" "minitwit" {
  name = "minitwit"
  public_key = file(var.pub_key)
}

resource "digitalocean_droplet" "db-droplet" {
  image = "docker-20-04" // ubuntu-22-04-x64
  name = "db"
  region = var.region
  size = "s-1vcpu-1gb"
  ssh_keys = [digitalocean_ssh_key.minitwit.fingerprint]

  # specify a ssh connection
  connection {
    user = "root"
    host = self.ipv4_address
    type = "ssh"
    private_key = file(var.pvt_key)
    timeout = "2m"
  }

  provisioner "file" {
    source = "remote_files/db/docker-compose.yml"
    destination = "~/minitwit/docker-compose.yml"
  }
  
  provisioner "file" {
    source = "remote_files/db/deploy.sh"
    destination = "~/minitwit/deploy.sh"
  }

  provisioner "remote-exec" {
    inline = [
      # SSH
      "ufw allow 22",

      # deploy db application
      "cd ~/minitwit",
      "chmod +x deploy.sh",
      "bash -x deploy.sh",
    ]
  }
}

output "minitwit-db-ip-address" {
  value = digitalocean_droplet.db-droplet.ipv4_address
}