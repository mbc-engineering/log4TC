# Contributing to log4TC
We love your input! We want to make contributing to this project as easy and transparent as possible, whether it's:

- Reporting a bug
- Discussing the current state of the code
- Submitting a fix
- Proposing new features
- Becoming a maintainer

## We Develop with Github
We use github to host code, to track issues and feature requests, as well as accept pull requests.

## We Use Pull Requests for Code changes
Pull requests are the best way to propose changes to the codebase. We actively welcome your pull requests:

1. Fork the repo and create your branch from `master`.
2. If you've changed APIs, include the API change in the Pull Request so that the official homepage can be updated.
4. Link your Issue for that pull request!

## Report bugs using Github's [issues](https://github.com/mbc-engineering/log4TC/issues)
We use GitHub issues to track public bugs. Report a bug by [opening a new issue](https://github.com/mbc-engineering/log4TC/issues/new); it's that easy!

## Write bug reports with detail, background, and sample code

**Great Bug Reports** tend to have:

- A quick summary and/or background
- Steps to reproduce
  - Be specific!
  - Give sample code if you can.
- What you expected would happen
- What actually happens
- Notes (possibly including why you think this might be happening, or stuff you tried that didn't work)

## Use a Consistent Coding Style
* Make sure to edit the project with the same version of Visual Studio as the master branch. All Twincat software has been developed using Visual Studio 2017 Shell. Note that VS2017 shell (provided with TwinCAT 3.1.4024.x) can't open the .net solution (as it's a .NET/C# project). Instead it's recommended to use [VS2019 community edition](https://visualstudio.microsoft.com). 
* The prefixes of naming of function blocks/variables/etc from the [Beckhoff TwinCAT3 identifier/name conventions](https://infosys.beckhoff.com/english.php?content=../content/1033/tc3_plc_intro/18014401873267083.html&id=) 
* Make sure to set your TwinCAT development environment to use Separate LineIDs. This is available in the TwinCAT XAE under **Tools→Options→TwinCAT→PLC Environment→Write options→Separate LineIDs** (set this to TRUE, more information is available [here](https://infosys.beckhoff.com/english.php?content=../content/1033/tc3_userinterface/18014403202147467.html&id=))

## License
By contributing, you agree that your contributions will be licensed under its Apache 2.

## References
This document was adapted from briandk's excellent [contribution guidelines template](https://gist.github.com/briandk/3d2e8b3ec8daf5a27a62).

## publish deb packages

with aptly is it possible to create a deb repository to host the deb packages. This can then be published to the static website of github pages.

```bash
# navigate to the aptly configuration directory
cd source/aptly/

# Create a local aptly repository (only needs to be done once)
aptly repo create -config=aptly.conf -component=main -distribution=stable log4tc

# add deb packages to the repository
aptly repo add -config=aptly.conf log4tc *.deb

# Generate GPG key for signing (one-time setup)
cat >gpg-batch <<EOF
%echo Generating GPG key for log4TC repository
Key-Type: RSA
Key-Length: 4096
Subkey-Type: RSA
Subkey-Length: 4096
Name-Real: log4TC Package Repository
Name-Email: packages@log4tc.mbc-engineering.com
Expire-Date: 0
%no-protection
%commit
%echo done
EOF
gpg --batch --generate-key gpg-batch

# Export the public key
gpg --armor --export packages@log4tc.mbc-engineering.com > /root/.aptly/public/log4tc-archive-keyring.gpg

# Get the GPG key ID
export GPG_KEY_ID=$(gpg --list-keys --with-colons packages@log4tc.mbc-engineering.com | grep '^pub' | cut -d':' -f5)

# publish the repository to a local directory with GPG signing
aptly publish repo -config=aptly.conf -architectures="amd64,arm64" -gpg-key="$GPG_KEY_ID" log4tc

# The contents of the public directory can then be copied to the gh-pages branch of the github repository
cp -r /root/.aptly/public/* /tmp/deb/

# To use the repository, first download and install the GPG key:
# wget -qO- https://mbc-engineering.github.io/log4TC/deb/log4tc-archive-keyring.gpg | sudo tee /etc/apt/trusted.gpg.d/log4tc-archive-keyring.gpg > /dev/null

# Then add the repository to apt /etc/apt/sources.list.d/log4tc.list:
# deb [signed-by=/etc/apt/trusted.gpg.d/log4tc-archive-keyring.gpg] https://mbc-engineering.github.io/log4TC/deb/ stable main

# or in the new format /etc/apt/sources.list.d/log4tc.sources:
# Types: deb
# URIs: https://mbc-engineering.github.io/log4TC/deb
# Suites: stable
# Components: main
# Signed-By: /etc/apt/trusted.gpg.d/log4tc-archive-keyring.gpg
```
