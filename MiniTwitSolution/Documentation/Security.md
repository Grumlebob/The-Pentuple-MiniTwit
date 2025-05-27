# Security

## Guide - How to get an overview of vulnabilites and exposures of our docker images

On your local machine run the following command:

```bash
docker compose up --build
```

This will build and start our containers.
To get an overview of the names and tags of our docker images run the following command:

```bash
docker images
```

To get an overview of a containers vulnabilites we use ```docker scout``` (<https://docs.docker.com/scout/explore/analysis/>)

We can use two commands ```docker scout quickview``` or ```docker scout cves```. ```quickview``` gives you a summary of a specified image, and ```cves```gives you a complete local analysis of the specified image.

### How to use docker scout quickview

To use ```docker scout quickview```, choose which docker image you want to get an overview of together with a tag.

Example:

```bash
docker scout quickview minitwitsolution-client:latest
✓ SBOM of image already cached, 82 packages indexed

 Target                 │  minitwitsolution-client:latest  │    0C     7H     0M     1L
 digest                 │  315733f0ee24                    │
 Base image             │  nginx:1-alpine                  │    0C     7H     0M     1L
 Updated base image     │  nginx:1-alpine-slim             │    0C     0H     0M     0L
                        │                                  │           -7            -1
```

### How to use docker scout cves

To use ```docker scout cves```, choose which docker image you want to get a complete analysis of together with a tag.

Example:

```bash
docker scout cves minitwitsolution-client:latest

✓ SBOM of image already cached, 82 packages indexed
✗ Detected 5 vulnerable packages with a total of 8 vulnerabilities

## Overview

                    │          Analyzed Image
────────────────────┼───────────────────────────────────
  Target            │  minitwitsolution-client:latest
    digest          │  315733f0ee24
    platform        │  linux/amd64
    vulnerabilities │    0C     7H     0M     1L
    size            │ 34 MB
    packages        │ 82

## Packages and Vulnerabilities

   0C     2H     0M     1L  libxml2 2.13.4-r3
pkg:apk/alpine/libxml2@2.13.4-r3?os_name=alpine&os_version=3.21

    ✗ HIGH CVE-2025-24928
      https://scout.docker.com/v/CVE-2025-24928
      Affected range : <2.13.4-r4
      Fixed version  : 2.13.4-r4

    ✗ HIGH CVE-2024-56171
      https://scout.docker.com/v/CVE-2024-56171
      Affected range : <2.13.4-r4
      Fixed version  : 2.13.4-r4

    ✗ LOW CVE-2025-27113
      https://scout.docker.com/v/CVE-2025-27113
      Affected range : <2.13.4-r5
      Fixed version  : 2.13.4-r5


   0C     2H     0M     0L  libxslt 1.1.42-r1
pkg:apk/alpine/libxslt@1.1.42-r1?os_name=alpine&os_version=3.21

    ✗ HIGH CVE-2025-24855
      https://scout.docker.com/v/CVE-2025-24855
      Affected range : <1.1.42-r2
      Fixed version  : 1.1.42-r2

    ✗ HIGH CVE-2024-55549
      https://scout.docker.com/v/CVE-2024-55549
      Affected range : <1.1.42-r2
      Fixed version  : 1.1.42-r2


   0C     1H     0M     0L  expat 2.6.4-r0
pkg:apk/alpine/expat@2.6.4-r0?os_name=alpine&os_version=3.21

    ✗ HIGH CVE-2024-8176
      https://scout.docker.com/v/CVE-2024-8176
      Affected range : <2.7.0-r0
      Fixed version  : 2.7.0-r0


   0C     1H     0M     0L  xz 5.6.3-r0
pkg:apk/alpine/xz@5.6.3-r0?os_name=alpine&os_version=3.21

    ✗ HIGH CVE-2025-31115
      https://scout.docker.com/v/CVE-2025-31115
      Affected range : <5.6.3-r1
      Fixed version  : 5.6.3-r1


   0C     1H     0M     0L  c-ares 1.34.3-r0
pkg:apk/alpine/c-ares@1.34.3-r0?os_name=alpine&os_version=3.21

    ✗ HIGH CVE-2025-31498
      https://scout.docker.com/v/CVE-2025-31498
      Affected range : <1.34.5-r0
      Fixed version  : 1.34.5-r0



8 vulnerabilities found in 5 packages
  CRITICAL  0
  HIGH      7
  MEDIUM    0
  LOW       1
```

### Vulnerability severity assessment

Docker scout shows a severity rating consisting of four levels: Critical (C), High (H), Medium (M), and Low (L).

### Using  docker scout recommendations on image

We can use ```docker scout``` to give us recommendations to fix vulnerabilites.

Example:

```bash
docker scout recommendations minitwitsolution-client:latest

✓ SBOM of image already cached, 82 packages indexed

## Recommended fixes

  Base image is  nginx:1-alpine

  Name            │  1-alpine
  Digest          │  sha256:a71e0884a7f1192ecf5decf062b67d46b54ad63f0cc1b8aa7e705f739a97c2fc
  Vulnerabilities │    0C     7H     0M     1L
  Pushed          │ 2 months ago
  Size            │ 21 MB
  Packages        │ 82
  Flavor          │ alpine
  Runtime         │ 1.27.4


  Refresh base image
  Rebuild the image using a newer base image version. Updating this may result in breaking changes.

  ✓ This image version is up to date.


Change base image
  The list displays new recommended tags in descending order, where the top results are rated as most suitable.


              Tag              │                        Details                        │    Pushed    │       Vulnerabilities
───────────────────────────────┼───────────────────────────────────────────────────────┼──────────────┼──────────────────────────────
   1-alpine-slim               │ Benefits:                                             │ 2 months ago │    0C     0H     0M     0L
  Minor runtime version update │ • Same OS detected                                    │              │           -7            -1
  Also known as:               │ • Minor runtime version update                        │              │
  • 1.27.4-alpine-slim         │ • Image is smaller by 15 MB                           │              │
  • 1.27-alpine-slim           │ • Image contains 58 fewer packages                    │              │
  • alpine-slim                │ • Image introduces no new vulnerability but removes 8 │              │
  • alpine3.21-slim            │ • Tag is using slim variant                           │              │
  • 1-alpine3.21-slim          │ • 1-alpine-slim was pulled 215K times last month      │              │
  • mainline-alpine-slim       │                                                       │              │
  • 1.27-alpine3.21-slim       │ Image details:                                        │              │
  • 1.27.4-alpine3.21-slim     │ • Size: 5.4 MB                                        │              │
  • mainline-alpine3.21-slim   │ • Flavor: alpine                                      │              │
                               │ • Runtime: 1.27.4                                     │              │
                               │                                                       │              │
                               │                                                       │              │
                               │                                                       │              │
```
