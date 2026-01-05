# Debian Package Installation Test

This pipeline template tests the installation and functionality of the log4tc Debian package.

## What it does

1. **Sets up test environment** with Docker Compose:
   - InfluxDB 1.8 container for log storage
   - Debian Bookworm container for package installation testing

2. **Installs log4tc** from the GitHub Pages APT repository:
   - Adds the log4tc repository to apt sources
   - Installs the `Mbc.Log4Tc.Service` package

3. **Configures log4tc** to connect to InfluxDB:
   - Creates appsettings.json with InfluxDB output configuration
   - Deploys configuration to `/etc/log4tc/config/`

4. **Validates the installation**:
   - Starts the log4tc service
   - Checks the internal log at `/var/log/log4tc/service.log`
   - Verifies InfluxDB connectivity
   - Checks for errors in the service log

## Requirements

- Docker and Docker Compose must be available on the build agent
- The log4tc Debian repository must be accessible at https://mbc-engineering.github.io/log4TC/deb

## Test Results

Test logs are published as build artifacts under `debian-test-logs`, including:
- `service.log` - The internal log4tc service log

## Future Enhancements

This test environment can be extended to support:
- End-to-end tests with TwinCAT Docker containers
- Integration tests with actual PLC log messages
- Performance and load testing
