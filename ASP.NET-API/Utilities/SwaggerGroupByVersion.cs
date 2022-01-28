using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ASP.NET_API.Utilities
{
    public class SwaggerGroupByVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var namespaceController = controller.ControllerType.Namespace; // Controllers.V1
            var versionAPI = namespaceController.Split('.').Last().ToLower(); // v1
            controller.ApiExplorer.GroupName = versionAPI;
        }
    }
}
