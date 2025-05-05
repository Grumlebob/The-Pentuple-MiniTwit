# credit: https://github.com/itu-devops/itu-minitwit-docker-swarm-teraform

resource "digitalocean_droplet" "seq-droplet" {
  image = "docker-20-04" // ubuntu-22-04-x64
  name = "seq"
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
    EOT

    destination = "/root/.bash_profile"
  }

  provisioner "remote-exec" {
    inline = [
      "mkdir -p /root/minitwit",
    ]
  }

  provisioner "file" {
    source = "remote_files/seq/docker-compose.yml"
    destination = "/root/minitwit/docker-compose.yml"
  }
  
  provisioner "file" {
    source = "remote_files/seq/deploy.sh"
    destination = "/root/minitwit/deploy.sh"
  }

  provisioner "remote-exec" {
    inline = [
      # ports for apps
      "ufw allow 80",
      "ufw allow 8080",
      "ufw allow 8888",
      
      # SSH
      "ufw allow 22",

      # deploy seq application
      "cd /root/minitwit",
      "chmod +x deploy.sh",
      "bash -x deploy.sh",
    ]
  }
}

output "minitwit-seq-ip-address" {
  value = digitalocean_droplet.seq-droplet.ipv4_address
}