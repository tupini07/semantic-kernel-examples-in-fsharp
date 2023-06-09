module KernelSettings

open FSharp.Data
open System
open Microsoft.Extensions.Logging

type private KernelSettingsType = JsonProvider<"config/appsettings.example.json">

type EndpointType =
    | TextCompletion
    | ChatCompletion

type KernelSettings =
    { EndpointType: EndpointType
      ServiceId: String
      DeploymentOrModelId: String
      ApiKey: String
      BingApiKey: String
      LogLevel: LogLevel }

let KernelSettings =
    let rawSettings =
        KernelSettingsType.Load(
            Environment.CurrentDirectory
            + "/config/appsettings.json"
        )

    let endpointType =
        match rawSettings.EndpointType with
        | "text-completion" -> TextCompletion
        | "chat-completion" -> ChatCompletion
        | et -> failwith $"Invalid endpoint type {et}"

    { EndpointType = endpointType
      ServiceId = rawSettings.ServiceId
      DeploymentOrModelId = rawSettings.DeploymentOrModelId
      ApiKey = rawSettings.ApiKey
      BingApiKey = rawSettings.BingApiKey
      LogLevel = LogLevel.Parse(rawSettings.LogLevel) }
