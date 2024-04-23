
# Example WebApi

An example API showing how to use the `RbacAuthorization` library.

## Walkthrough
The library is enabled using the `Services.AddRbacAuthorization()` in `Program.cs`.

## Usage
Starting the API will launch the Swagger UI and show the available endpoints.

To generate a JWT bearer token to use with the API, run the following command from
the ExampleWebApi folder. When entering the token into the Swagger UI you must prefix it
with "Bearer ".

### Admin
This token has the Admin role which is assigned all permissions giving it full access to the Users, 
Organizations, Projects and Tasks APIs.
```
dotnet user-jwts create --role "Admin"
```

### User + OrganizationAdmin
This token has the OrganizationAdmin role scoped to organization #1 giving it the same access as the above
Admin role but only within organization #1. To allow the token access to the Users API it has the User role
scoped to the Me API so that it can't search and view other users.
```
dotnet user-jwts create --role "User:/users/me" --role "OrganizationAdmin:/organizations/1"
```

### User + OrganizationUser + ProjectAdmin
This token has the ProjectAdmin role scoped to project #2 in organization #1 to restrict the admin access further.
This token only has admin permissions within project #2. It also has the OrganizationUser role to give it basic
read access to the other Projects and Tasks APIs in organization #1. To allow the token access to the Users API
it has the User role scoped to the Me API so that it can't search and view other users.
```
dotnet user-jwts create --role "User:/users/me" --role "OrganizationUser:/organizations/1" --role "ProjectAdmin:/organizations/1/projects/2"
```