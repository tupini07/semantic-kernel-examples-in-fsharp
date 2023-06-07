module KernelBuilderExtensions

open Microsoft.SemanticKernel
open KernelSettings

type KernelBuilder with
    member x.WithCompletionService(settings: KernelSettings) =
        x.WithOpenAITextCompletionService(
            modelId = settings.DeploymentOrModelId,
            apiKey = settings.ApiKey,
            orgId = "",
            serviceId = settings.ServiceId
        )
// .WithOpenAIChatCompletionService(
//     modelId = settings.DeploymentOrModelId,
//     apiKey = settings.ApiKey,
//     orgId = "",
//     serviceId = settings.ServiceId
// )
