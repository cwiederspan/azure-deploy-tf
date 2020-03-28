using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Rest.Azure;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ContainerInstance.Fluent.Models;

using Dapr.Actors;
using Dapr.Actors.Runtime;
using Microsoft.Azure.Management.AppService.Fluent.Models;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace AzureTf.BuilderActor {

    interface IJobActor : IActor {

        Task<string> Execute(Payload payload);
    }

    [Actor(TypeName = "job")]
    public class JobActor : Actor, IJobActor {

        //private readonly string StateName = "locations";

        private readonly string RESOURCE_GROUP_NAME = "cdw-tftesting-20200327";

        private readonly string CONTAINER_GROUP_NAME = "cdw-container-20200327";

        private readonly string CONTAINER_NAME = "terraform";

        private readonly string CONTAINER_IMAGE = "hashicorp/terraform:latest";

        public JobActor(ActorService service, ActorId actorId) : base(service, actorId) {

            // Pass the args to the caller
            //this.Id.GetId();
            //Console.Write("ARM_CLIENT_ID = " + configuration.GetValue<string>("Azure:ARM_CLIENT_ID"));
        }

        public async Task<string> Execute(Payload payload) {

            //Console.WriteLine(JsonSerializer.Serialize(payload));

            var credentials = SdkContext.AzureCredentialsFactory
                .FromServicePrincipal(
                    Environment.GetEnvironmentVariable("ARM_CLIENT_ID"),
                    Environment.GetEnvironmentVariable("ARM_CLIENT_SECRET"),
                    Environment.GetEnvironmentVariable("ARM_TENANT_ID"),
                    AzureEnvironment.AzureGlobalCloud
                )
                .WithDefaultSubscription(Environment.GetEnvironmentVariable("ARM_SUBSCRIPTION_ID"));

            var azure = Azure
                .Configure()
                .Authenticate(credentials)
                .WithDefaultSubscription();

            // Get the resource group's region
            var resGroup = azure.ResourceGroups.GetByName(RESOURCE_GROUP_NAME);
            var azureRegion = resGroup.Region;

            // Create the container group
            var containerGroup = await azure.ContainerGroups
                .Define(CONTAINER_GROUP_NAME)
                .WithRegion(azureRegion)
                .WithExistingResourceGroup(RESOURCE_GROUP_NAME)
                .WithLinux()
                .WithPublicImageRegistryOnly()
                .WithoutVolume()
                .DefineContainerInstance(CONTAINER_NAME)
                    .WithImage(CONTAINER_IMAGE)
                    .WithoutPorts()
                    //.WithExternalTcpPort(80)
                    .WithCpuCoreCount(1.0)
                    .WithMemorySizeInGB(1)
                    .WithStartingCommandLine(this.GetCommand(payload))
                    .WithEnvironmentVariables(this.GetEnvironmentVariables(payload))
                    .Attach()
                //.WithDnsPrefix(containerGroupName)
                .WithRestartPolicy(ContainerGroupRestartPolicy.Never)
                .CreateAsync();

            return "Success";
        }

        private IDictionary<string, string> GetEnvironmentVariables(Payload payload) {

            // TODO: These should be the client's credentials, not ours
            return new Dictionary<string, string> {
                { "ARM_TENANT_ID",       payload.TenantId },
                { "ARM_SUBSCRIPTION_ID", payload.SubscriptionId },
                { "ARM_CLIENT_ID",       payload.ClientId },
                { "ARM_CLIENT_SECRET",   payload.Secret }
            };
        }

        private string GetCommand(Payload payload) {

            var variables = payload.Variables
                .Select(v => $"-var {v.Name}={v.Value}")
                .ToList();

            var varResult = String.Join(" ", variables);

            var builder = new StringBuilder();

            builder.Append($"/bin/sh -c '");
            builder.Append($"git clone {payload.Repo} src");
            builder.Append($" && cd /src{payload.Path}");
            builder.Append($" && terraform init");
            builder.Append($" && terraform apply --auto-approve {varResult}");
            builder.Append($"'");

            Console.WriteLine(builder.ToString());

            return builder.ToString();
        }

        /*
        private string GetCommand(Payload payload) {

            var variables = payload.Variables
                .Select(v => $"-var '{v.Name}={v.Value}'")
                .ToList();

            var varResult = String.Join(" ", variables);

            var builder = new StringBuilder();

            builder.Append($"git clone {payload.Repo} src");
            builder.Append($" && cd /src{payload.Path}");
            builder.Append($" && terraform init");
            builder.Append($" && terraform apply --auto-approve {varResult}");

            Console.WriteLine(builder.ToString());

            return builder.ToString();
        }
        */
    }

    public class Payload {

        [JsonPropertyName("tenantId")]
        public string TenantId { get; set; }

        [JsonPropertyName("subscriptionId")]
        public string SubscriptionId { get; set; }

        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }

        [JsonPropertyName("secret")]
        public string Secret { get; set; }


        [JsonPropertyName("repo")]
        public string Repo { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("variables")]
        public IList<Variable> Variables { get; set; }
    }

    public class Variable {

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
