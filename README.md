# Terraform Deploy to Azure

A project to speed up the deployment of Terraform resources to Azure from a GitHub repository.

## Azure Snippets

```bashrc

az ad sp create-for-rbac --role contributor --name cdw-tftesting-20200327-sp

```

## Dapr Snippets

```bash

dapr run --port 3500 --app-id job-actor --app-port 5000 dotnet run

```

## Postman Snippets

```json

{
	"tenantId": "72f988bf-xxxx-xxxx-xxxx-2d7cd011db47",
	"subscriptionId": "b9c770d1-xxxx-xxxx-xxxx-95ce1a4fac0c",
	"clientId": "19e2eeda-xxxx-xxxx-xxxx-7381043324cd",
	"secret": "8ea59f03-xxxx-xxxx-xxxx-c3844a3ec7ef",
	"repo": "https://github.com/ateamsw/learn-terraform.git",
	"path": "/challenges/02_create-resource-group/solution",
	"variables": [
		{
			"name": "resource_group_name",
			"value": "cdw-yyy-20200327"
		}	
	]
}

```

## Docker Snippets

```bash

docker run -it --rm --entrypoint=/bin/sh \
 --env ARM_TENANT_ID=72f988bf-xxxx-xxxx-xxxx-2d7cd011db47 \
 --env ARM_SUBSCRIPTION_ID=b9c770d1-xxxx-xxxx-xxxx-95ce1a4fac0c \
 --env ARM_CLIENT_ID=19e2eeda-xxxx-xxxx-xxxx-7381043324cd \
 --env ARM_CLIENT_SECRET=8ea59f03-xxxx-xxxx-xxxx-c3844a3ec7ef \
 hashicorp/terraform:latest

git clone https://github.com/ateamsw/learn-terraform.git src

cd /src/challenges/02_create-resource-group/solution

terraform init

terraform plan -var 'resource_group_name=cdw-tftesting-20200327'

terraform apply --auto-approve -var 'resource_group_name=cdw-tftesting-20200327'

```

## ACI Snippets

```bash

az container create \
 -n cdw-container2-20200327 \
 -g cdw-tftesting-20200327 \
 -l westus2 \
 ** Needs Environment Variables **
 --image hashicorp/terraform:latest \
 --command-line "git clone https://github.com/ateamsw/learn-terraform.git src && cd /src/challenges/02_create-resource-group/solution && terraform init && terraform apply --auto-approve -var 'resource_group_name=cdw-xxx-20200327'"

```
