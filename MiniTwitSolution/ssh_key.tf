# this file ensures that droplets has all group members' ssh keys (credit to chatgpt)
variable "team_key_names" {
  type    = list(string)
  # names taken from digital ocean ssh keys
  default = ["Rasmus", "JacobGrum", "Jacob", "Thor", "Adam", "Christian Lauridsen"]
}

data "digitalocean_ssh_key" "team" {
  for_each = toset(var.team_key_names)
  name     = each.value
}