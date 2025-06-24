# Tell terraform to use remote bucket for backend
terraform {
    # It is empty because the bootstrap script fills in the values
    backend "s3" {}
}