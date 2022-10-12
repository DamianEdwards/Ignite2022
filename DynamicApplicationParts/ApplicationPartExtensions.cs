
using Microsoft.AspNetCore.Mvc.Infrastructure;

public static class ApplicationPartExtensions
{
    public static DynamicApplicationPart AddDynamicApplicationPart(this IMvcBuilder mvcBuilder)
    {
        var dynamicPart = new DynamicApplicationPart();
        // Tell the MVC that we want to notify it as changes
        mvcBuilder.Services.AddSingleton<IActionDescriptorChangeProvider>(dynamicPart);

        mvcBuilder.PartManager.ApplicationParts.Add(dynamicPart);

        return dynamicPart;
    }
}
