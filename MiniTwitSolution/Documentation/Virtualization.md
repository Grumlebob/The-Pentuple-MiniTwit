# Digital Ocean

Why use digital ocean?
A: The only reason is because we can use free credits via. the Github Education pack. This is the only motivator.

---

## Setup ssh key guide

### Step 1

Create a ssh key pair with a private and public key inside the directory ~/.ssh/.
Make sure that the name of the file containing the private key has the name **id_rsa**

Summary: Make sure that the private key is located at:
```txt
~/.ssh/id_rsa
```

### Step 2

Send the public key to @ChrBank.

@ChrBank will register your public key and assign you a with a SSH_KEY_NAME.

#### Local setup (Windows):

#### Local setup (macOS):

Depending on which shell you are using. Go to the file .bashrc or .zshrc.

Inside that file:
Add the following lines:

```bash
export DIGITAL_OCEAN_TOKEN="<your digital ocean token>"
export SSH_KEY_NAME="<your first name>"

#Example
export SSH_KEY_NAME="Christian"
```

**Note**: Ask @ChrBank to get access to the digital ocean token.

When these changes are added make sure to run:

```bash
#If you made changes inside .bashrc
source .bashrc

#If you made changes inside .zshrc
source .zshrc
```

### Step 3

Congrats!!!!
Everything with ssh should work now :)

---

## Create a droplet

Make sure these tools are installed:
* Vagrant

Make sure this plugin is installed:
```bash
vagrant plugin install vagrant-digitalocean
```

To create a droplet in digital ocean run the following command inside the directory where the Vagrantfile exist: 
The-Pentuple-MiniTwit/MiniTwitSolution

```bash
vagrant up
```

If this does not work try:

```bash
vagrant up --provider=digital_ocean
```
