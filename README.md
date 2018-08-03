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
## Create Continuous Deployment Pipeline
In order to create the CI Pipeline we first need to meet requirements indicated in the section above, that I will repeat here:
- Create an Azure Account: https://azure.microsoft.com/es-es/free/
- Create a Visual Studio Team Services Account: https://azure.microsoft.com/es-es/services/visual-studio-team-services/
- Create a Github Account: https://github.com/join
- Download Docker: https://store.docker.com/search?offering=community&type=edition

After having all this requirements met. We need to do some more steps:

 1. Create an Azure Resource Manager Service Endpoint that will allow us to connect the different artifacts from Visual Studio Team Services with our Azure Account. For this step you can follow this blog post that explains in detail how to do it: http://www.donovanbrown.com/post/Creating-an-Azure-Resource-Manager-Service-Endpoint
 When finished, we will have configured a Service Endpoint in Visual Studio Team Services pointing to our Azure subscription.
 2. Next step is to create Azure Container Registry (ACR), for what you can follow this Microsoft instructions: https://docs.microsoft.com/en-us/azure/container-registry/container-registry-get-started-portal
 And you should collect the following data from the ACR resource in Azure:
 - Username and password generated in Access Keys
 - Login Server: [your-acr-name].azurecr.io
 3. Next Step will be install Docker (in my case Docker for Windows) that you can grab from here: https://store.docker.com/search?offering=community&type=edition
 4. Then you should add Docker support to the project. This step is not needed if you have cloned this repo and you are using .NET Core 2.1. In other case is better to add a correct DockerFile, just in case the one provided here doesn't work. 
 To do this, using Visual Studio just right-click on the Theam.API Project and do Add -> Docker Support. It will add all required	 files to the project.
 Then modify the file to look like this one (be aware of your version of .NET Core and change it accordingly)
	 ```
	 FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
	WORKDIR /app
	EXPOSE 80
	EXPOSE 443

	FROM microsoft/dotnet:2.1-sdk AS build
	WORKDIR /src
	COPY Theam/Theam.API.csproj Theam/
	RUN dotnet restore Theam/Theam.API.csproj
	COPY . .
	WORKDIR /src/Theam
	RUN dotnet build Theam.API.csproj -c Release -o /app

	FROM build AS publish
	RUN dotnet publish Theam.API.csproj -c Release -o /app

	FROM base AS final
	WORKDIR /app
	COPY --from=publish /app .
	ENTRYPOINT ["dotnet", "Theam.API.dll"]
	 ```
 5. Create Web App for Container in Azure:
- Go to Azure Portal
- Create New Web App for Container with the following settings:
- Image Source: Azure Container Registry
- Repo Access: Public
- Image and Tag: Select the image and latest tag
- Give it an app name, select the Azure subscription, create a new resource group, and select service plan according to your needs.
6. Go to VSTS and create a new Build Pipeline:
- Give it a name and select Hosted Linux Preview as Agent queue.
- This will create a phase called Phase 1. Click on that phase and change the name to Test Phase.
- Change the Agent Queue to Hosted VS2017
- Click on the + button right on the phase item, and Add a dotnet build task. 
In the Path to project put this: `**/*.csproj` this will tell the task to search all .csproj files in all subfolders and build it.
- Click again on the + of the Test phase and Add a Visual Studio Test task. The default configuration will work, but you can check "Code coverage enabled" checkbox just in case you want this information.
- Now we have to create another phase, to build the docker image and push it to Azure. So click on the 3 dots button on the right of the Process item and click on Add agent phase.
- Click on the new phase and rename it to Build Phase, in the Agent Queue select "inherit from pipeline", so it will take Linux Hosted Agent we selected when created the pipeline.
- Click on the + button of Build Phase and add a .NET Core Tool Installer phase. In the Version field, put `2.1.300`. This task will install this version of .NET Core in the Agent Virtual Machine, this is needed just now because they don't have it installed by default, so if you don't do this, it will fail the build. Maybe in the future this task won't be needed.
- Add a Docker task and call it Build API Image, with this configuration:
	- Container Registry Type: Azure Container Registry
	- Azure subscription: Select the Azure Resource Manager you created some steps before.
	- Azure Container Registry: select the one you created just before.
	- Action: Build an image
	- Dockerfile: Theam/Dockerfile
	- Build arguments: ` --pull` (be aware of the blank space just before the 2 semicolons)
	- Build Context: $(Build.Repository.LocalPath)
	- Check include latest tag
- Creates another Docker task and call it Push API Image
	- The configuration is the same than before, just changes the Action to: Push an image.

	We have finished to create the Build Pipeline. Now click on the arrow next to Save & queue and click Save, to only save the build pipeline.
7.  Next step is to create a Release Pipeline for this Build. Click on the Releases menu item and click on the + button to Create a new Release Pipeline. 
- In the template select Empty Process
- Click on Add an Artifact, select Source Type = Build, Project = your VSTS project, and Source = Your build pipeline created just before, and click on Add button.
- On the right side is the Environment, we will deploy the Docker imaged created in the Build Phase to an Azure Web App for Container. 
- Click on the Environment 1 and rename it if you wish.
- Now click below the name where is the link 1 phase, 1 task. This will open the pipeline editor. 
- Click on the Agent phase and select Hosted VS 2017 as the Agent Queue.
- Click on the + button to add a task, and add Azure App Service Deploy.
- In the task configuration you have to select your Azure Resource Manager like before.
- App type = Web App
- App service name = select the one you created some steps before.
- Image source = Container Registry
- Registry = the login server address of your container registry
- Image = your image name
- Leave the rest as default and save the Release pipeline.
8. Return to the Build pipeline, click on Edit and go to Triggers Tab. Enable continuous integration, enable Batch changes while a build is in progress and select the branch or branches from where you want to pull the code.
This step will activate the build and deploy every time a commit arrives to one of the branches selected here.

## Video

In this video I show and explain better the CI/CD process and configuration:

[![Alt text](https://img.youtube.com/vi/sy0VXbE0yqA/0.jpg)](https://www.youtube.com/watch?v=sy0VXbE0yqA)