# TLS / HTTPS
## 04.04

### `ssh` into droplet (worker manager - `api`)

```bash
ssh -i C:\Users\adamh\.ssh\do_ssh_key root@157.245.26.8
```
 

`Links to tutorial`: <br>
* https://www.digitalocean.com/community/tutorials/how-to-secure-nginx-with-let-s-encrypt-on-ubuntu-22-04
* https://www.digitalocean.com/community/tutorials/how-to-configure-nginx-as-a-reverse-proxy-on-ubuntu-22-04

### Installing nginx
Installing nginx
```bash
sudo apt update
sudo apt install nginx
```

Allow Nginx through the firewall, and verify that it is running
```bash
sudo ufw allow 'Nginx Full'
sudo ufw delete allow 'Nginx HTTP'
systemctl status nginx
```

### Installing Certbot

Enable snap on droplet
```bash
sudo snap install core; sudo snap refresh core
```

Remove and install `Certbot`
```bash
sudo apt remove certbot # Remove Certbot
sudo snap install --classic certbot # Installing 
```

Create certificate 
```bashrc
sudo ln -s /snap/bin/certbot /usr/bin/certbot
```

### Configuring Server Block
Creating configuration file (example given for `api.thepentupledo.engineer`) 

```bash
sudo vim /etc/nginx/sites-available/api.thepentupledo.engineer
```

**Configuration file (`api.thepentupledo.engineer`)**

```conf
server {
    listen 443 ssl;
    server_name api.thepentupledo.engineer www.api.thepentupledo.engineer;

    ssl_certificate /etc/letsencrypt/live/thepentupledo.engineer/fullchain.pem; # managed by Certbot
    ssl_certificate_key /etc/letsencrypt/live/thepentupledo.engineer/privkey.pem; # managed by Certbot
    include /etc/letsencrypt/options-ssl-nginx.conf; # managed by Certbot
    ssl_dhparam /etc/letsencrypt/ssl-dhparams.pem; # managed by Certbot

    location / {
        proxy_pass http://localhost:5002; # Ensuring internal routing
        include proxy_params;
    }

    location /swagger/ {
        proxy_pass http://localhost:5002/swagger/;
        include proxy_params;
    }
}

# Redirect all HTTP traffic to HTTPS
server {
    listen 80;
    server_name api.thepentupledo.engineer www.api.thepentupledo.engineer;
    return 301 https://$host$request_uri;
}
```

This includes changes made by certBot when creating the certificate later in the tutorial.

Create settings file for adding support for HTTP requests to include headers (if does not exist by default, refer to code block below):

```bash
sudo nvim /etc/nginx/proxy_params
```

```nginx location="/etc/nginx/proxy_params"
proxy_set_header Host $http_host;
proxy_set_header X-Real-IP $remote_addr;
proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
proxy_set_header X-Forwarded-Proto $scheme;
```

Test whether your configuration file has syntax errors, and if no errors were detect apply all changes

```bash
sudo nginx -t
sudo systemctl restart nginx
```

### Obtaining SSL Certificate

```bash
sudo certbot --nginx -d api.thepentupledo.engineer -d www.api.thepentupledo.engineer
```

You will prompted to enter an email address, just skip this step by pressing `enter`.

Adding a timer that automatically renews any certificate that's within thirty days of expiration (when adding a certificate they are valid for ninety days)

Enable configuration file by creating a link (nginx reads from this at startup)

```bash
sudo ln -s /etc/nginx/sites-available/api.thepentupledo.engineer /etc/nginx/sites-enabled/
```

If you would like you can restart nginx like so:
```bash
sudo nginx -t
sudo systemctl restart nginx
```

```bash
sudo systemctl status snap.certbot.renew.service
sudo certbot renew --dry-run
```

### Creating a new DNS domain

Login to the appropriate `name.com` account. Go under `My Domains`, top right part of the website. Click on three dots on right of the domain name `thepentupledo.engineer`, and click `Manage DNS Records`. 

Add a type `A`, if u leave the host blank the domain wil be `thepentupledo.engineer`, but if you set the host to be `api`, the resulting domain name will be `api.thepentupledo.engineer`. THe answer is the IP address for the specific droplet. So for our API droplet that will be `157.245.26.8`. Then click `Add Record`.

Make sure that the nginx.conf file has the correct host name, under `server_name`.