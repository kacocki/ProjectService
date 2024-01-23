# Project API Service

## Environment 

- .NET version: 6.0

## Data:  
Example of a project model JSON object:
```
{
    id: 5,
    name: "test name",
    IsAvailable : true,
    addedDate: 1573843210
}   
```

Example of a user model JSON object:
```
{
    id: 5,
    name: "test user name",
    projectId: 5,
    addedDate: 1573843210
}  
```

## Functionality Requirements

The following API needs to be implemented:

- `POST` request to `api/projects/{projectId}/users`:
    - Add the user to the given projectId. 
    - The HTTP response code should be 201 on success.
    - For the body of the request, please use the JSON example of the user model given above.
    - If a project with {projectId} does not exist, return 404.

- `GET` request to `api/project/{projectId}/users`:
    - Return the entire list of users for the project with given {projectId}
    - The HTTP response code should be 200.
    - If a project with {projectId} does not exist, return 404.
 
- `DELETE` request to `api/projects/{projectId}` :
    - Delete the project with projectId. 
    - The HTTP response code should be 204 on success.
    - If a project with {projectId} does not exist, return 404.
 

- NOTE: You need to add support for Dependency Injection for internal services (UsersService and ProjectsService) in the project Startup.cs file.

## Example requests and responses with headers

**Request 1:**

`POST` request to `api/projects/1/users`

```
{
    id: 5,
    name: "Alex",
    addedDate: 1573843210,
    projectId: 1,
}
```
The response code will be 201 and this user is added to the project with id 1.

**Request 2:**

`GET` request to `api/projects/10/users`

The response code is 200, and when converted to JSON, the response body (assuming that the below objects are all objects in the collection) is as follows:

```
[
   {
      id: 5,
      name: "Alex",
      addedDate: 1573843210,
      projectId: 10,
   },
   {
      id: 6,
      name: "John",
      addedDate: 1593943210,
      projectId: 10,
   }
]
```

**Request 3:**

`DELETE` request to `api/projects/10`

Assuming that the project with id 10 exists, then the response code is 204 and there are no particular requirements for the response body. This causes the project with id 10 to be removed from the collection. When a project with id 10 doesn't exist, then the response code is 404 and there are no particular requirements for the response body.

## Project Specifications

**Read Only Files**
- ProjectService.Tests/IntegrationTests.cs

**Commands**  
- run:  
```
run: dotnet build && dotnet run --project ProjectService.WebAPI
```
- install: 
```
dotnet build
```
- test: 
```
rm -rf reports && dotnet build && dotnet test --logger xunit --results-directory ./reports/
```
