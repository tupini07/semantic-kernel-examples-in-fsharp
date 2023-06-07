module KernelBuilderExtensions

open Microsoft.SemanticKernel
open KernelSettings

type KernelBuilder with
    member x.WithCompletionService(settings: KernelSettings) =
        ignore
        <| match settings.EndpointType with
           | TextCompletion ->
               x.WithOpenAITextCompletionService(
                   modelId = settings.DeploymentOrModelId,
                   apiKey = settings.ApiKey,
                   orgId = "",
                   serviceId = settings.ServiceId
               )
           | ChatCompletion ->
               x.WithOpenAIChatCompletionService(
                   modelId = settings.DeploymentOrModelId,
                   apiKey = settings.ApiKey,
                   orgId = "",
                   serviceId = settings.ServiceId
               )

        x
