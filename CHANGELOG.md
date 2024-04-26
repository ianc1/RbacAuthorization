# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2024-04-26

### Changed

- The role scope support introduced a breaking change with how roles are defined and registered using the `AddRbacAuthorization(options)` extension
  method. Refer to README.md for details.

### Added

- Support for scoping roles to the request path. For example a user with the role `OrganizationAdmin:/organizations/1` will have the `OrganizationAdmin`
  permissions restricted to the `/organizations/1` path. Role scopes are optional and can be parametrized to avoid duplicating role definitions. Refer
  to README.md for details.
- Support for .Net 8.


## [1.1.0] - 2023-10-21

### Added

- Support for retrieving role permissions and user roles from a database or API.


## [1.0.1] - 2023-04-30

### Fixed 

- Library did not target the .Net 6.0 Long Term Support release.

## [1.0.0] - 2023-04-30

- Initial release