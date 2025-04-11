# Changelog

## v2.8

### Changed

- Added domain names for:
  - client, api, seq
- Added tls encryption and https support
- Add sexy graphs to seq 

## v2.7

### Changed

- New sexy program.cs

- Our system has been deployed on different droplets:
  - db, seq and client each has their own droplet
  - api is run on a swarm of three droplets. One being the manager.
  - The GitHub workflow has been updated to run a migration on the manager droplet. Then deploy api and client separately.

## v2.6

### Changed

- Fixed requests and response body to be showed on Seq

## v2.5

### Changed

- Add automatic code formatting on pull requests
- Fixed seq not receiving logs
- Fix dependabot using an outdated repository

## v2.4

### Changed

- Add file for automated releases
- Add Changelog markdown file
- Fix [#46](https://github.com/Grumlebob/The-Pentuple-MiniTwit/issues/46)
- Performed an auto release to test if it works

## v2.3

### Changed

(Changelog was used after, this subheader was added to demonstrate format)
