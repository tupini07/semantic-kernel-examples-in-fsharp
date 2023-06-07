open Microsoft.Extensions.Logging
open Microsoft.SemanticKernel
open Microsoft.SemanticKernel.AI.ChatCompletion
open Microsoft.SemanticKernel.Orchestration

open KernelBuilderExtensions
open KernelSettings
open System.IO

let kernelSettings = KernelSettings

let loggerFactory =
    LoggerFactory.Create (fun builder ->
        builder
            .SetMinimumLevel(kernelSettings.LogLevel)
            .AddDebug()
        |> ignore)

let kernel =
    KernelBuilder()
        .WithLogger(loggerFactory.CreateLogger<IKernel>())
        .WithCompletionService(kernelSettings)
        .Build()

NovelCreator.createNovel kernel

// if kernelSettings.EndpointType = TextCompletion then
//     printfn "Running text completion example..."

//     let skillsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "skills")

//     let skill = kernel.ImportSemanticSkillFromDirectory(skillsDirectory, "FunSkill")

//     let context = ContextVariables()
//     context.Set("input", "Time travel to dinosaur age")
//     context.Set("style", "Dank")

//     let result =
//         kernel.RunAsync(context, skill.["Joke"])
//         |> Async.AwaitTask
//         |> Async.RunSynchronously

//     printfn "%A" result


// elif kernelSettings.EndpointType = ChatCompletion then
//     printfn "Running chat completion example..."

//     let chatCompletionService = kernel.GetService<IChatCompletion>()

//     let chat =
//         chatCompletionService.CreateNewChat("You are an AI assistant that helps people find information.")

//     chat.AddMessage(ChatHistory.AuthorRoles.User, "Hi, what information can yo provide for me?")

//     let response =
//         chatCompletionService.GenerateMessageAsync(chat, new ChatRequestSettings())
//         |> Async.AwaitTask
//         |> Async.RunSynchronously

//     printfn "%s" response
