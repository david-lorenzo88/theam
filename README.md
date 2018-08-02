# Theam CRM API

This is a project done for Theam's hiring process. We have created a simple CRM API using .NET Core Web API and some additional modules to fulfill all requirements requested by the team and explained in the next section.
I have also created a Continuous Deployment pipeline that pulls the code from this repo after each commit, launches a battery of tests, if all of them passes, then it creates a Docker image and push it to Azure Docker Container. Finally, it deploys that image to an Azure App Service so final users can access it.
All the architecture and details about constructing the pipeline are explained in the next sections.

## API Features

The API should be only accessible by a registered user by providing an authentication mechanism. 
- A user can only: 
	- List all customers in the database. 
	- Get full customer information, including a photo URL. 
	- Create a new customer: 
		- A customer should have at least name, surname, id and a photo field. 
		- Name, surname and id are required fields. 
		- Image uploads should be able to be managed. 
		- The customer should have a reference to the user who created it. 
	- Update an existing customer. 
		- The customer should hold a reference to the last user who modified it. 
	- Delete an existing customer. 
- An admin can also: 
	- Manage users: 
		- Create users. 
		- Delete users. 
		- Update users. 
		- List users. 
		- Change admin status.

## Requirements

In order to be able to execute the API and launch the battery of tests of this project, the following requirements would be needed:
 - Install .NET Core 2.1+:  https://www.microsoft.com/net/download (Required)
 - Install git: https://git-scm.com/book/en/v2/Getting-Started-Installing-Git (Optional but recommended)
 - Install Visual Studio 15.7+ (Optional but recommended)

If you want also be able to Dockerize the API, create the CI Pipeline and deploy the API to Azure, the following requirements would also be needed:
- Create an Azure Account: https://azure.microsoft.com/es-es/free/
- Create a Visual Studio Team Services Account: https://azure.microsoft.com/es-es/services/visual-studio-team-services/
- Create a Github Account: https://github.com/join
- Download Docker: https://store.docker.com/search?offering=community&type=edition


## How to install and run the API and Test Project

 1. Create a folder in your computer where the project will be downloaded.
 2. Open a terminal or command prompt **as Administrator** and ```cd``` into that folder.
 3. Clone or Download zip of this repo by executing the following command:
```git clone https://github.com/david-lorenzo88/theam```
 4. Edit the **Theam/appsettings.json** and **Theam.Tests/appsettings.json** and verify the location of the SQLite database is pointing to a correct folder in your computer.
 The appsettings.json file should look like this:
 
	 ```json
	 {
	  "ConnectionStrings": {
	    "SqliteConnection": "Data Source=D:\\sqlite\\theam.db"
	  },
	  "Logging": {
	    "LogLevel": {
	      "Default": "Warning"
	    }
	  },
	  "AllowedHosts": "*",
	  "Settings": {
	    "SecurityKey": "{KEY_HERE}",
	    "Domain": "localhost",
	    "ImagesBaseUrl": "http://localhost/",
	    "ImagesUploadPath": "uploads\\users",
	    "DatabaseDirectory": "D:\\sqlite",
	    "APIVersion": "v1"
	  }
	}
	```
 5. You should verify that **Data Source location** is a correct location in your computer (it does matter if it exists or not as the API will try to create it on the first run if it doesn't exists).
Also you should verify that the property **`Settings:DatabaseDirectory`** points to the directory where theam.db is located. 
 6. Run the API: You can run the API directly from Visual Studio, by opening the solution (.sln) and pressing F5 to run the project. 
 Or if you prefer to do it from the command line, you can run the following command from inside the Theam folder of the project:
 `dotnet run`
 You will see some stuff in the terminal and then finally will say something like:
	 ```
	 Now listening on: https://localhost:5001
	Now listening on: http://localhost:5000
	Application started. Press Ctrl+C to shut down.
	``` 
	 That means everything was OK and the API is listening on that ports for requests.
If you open the swagger page on the browser you should see the Swagger UI:
`https://localhost:5001/swagger`
You can also use [Postman](https://www.getpostman.com/) to make requests to the API
7. In order to run the test project and see the test results, you can do it also from inside Visual Studio, or from the command line where you should move to the Theam.Tests folder and run the following command:
`dotnet test`


