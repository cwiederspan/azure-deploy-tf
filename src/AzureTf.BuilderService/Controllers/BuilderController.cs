using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AzureTf.BuilderService.Controllers {
    
    [ApiController]
    //[Route("[controller]")]
    public class BuilderController : ControllerBase {

        private readonly ILogger<BuilderController> Logger;

        public BuilderController(ILogger<BuilderController> logger) {
            this.Logger = logger;
        }

        [HttpGet("deploy")]
        public void DeployAsync() {

            this.Logger.LogInformation("BuilderController.DeployAsync()...");
        }
    }
}
