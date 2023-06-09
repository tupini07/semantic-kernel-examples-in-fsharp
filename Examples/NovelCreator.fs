module NovelCreator

open Microsoft.SemanticKernel
open System.IO
open Microsoft.SemanticKernel
open Microsoft.SemanticKernel.Planning
open Microsoft.SemanticKernel.Planning.Sequential
open System
open FileSystemSkill
open Microsoft.SemanticKernel.CoreSkills
open KernelSettings
open Microsoft.Extensions.Logging
open KernelBuilderExtensions

let createNovel () =
    printfn "Running novel creator example ..."

    let loggerFactory =
        LoggerFactory.Create (fun builder ->
            builder
                .SetMinimumLevel(KernelSettings.LogLevel)
                .AddDebug()
            |> ignore)

    let kernel =
        KernelBuilder()
            .WithLogger(loggerFactory.CreateLogger<IKernel>())
            .WithCompletionService(KernelSettings)
            .Build()

    let skillsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "skills")

    // import skills into kernel so it's aware of them
    kernel.ImportSemanticSkillFromDirectory(skillsDirectory, "WriterSkill")
    |> fun s -> printfn "Imported skills %A" s.Keys

    kernel.ImportSemanticSkillFromDirectory(skillsDirectory, "MiscSkill")
    |> fun s -> printfn "Imported skills %A" s.Keys

    kernel.ImportSkill(FileSystemSkill(), "FileSystemSkill")
    |> fun s -> printfn "Imported skills %A" s.Keys

    let plannerConfig = SequentialPlannerConfig()
    plannerConfig.MaxTokens <- 2048

    printfn "Creating plan..."

    let plan =
        SequentialPlanner(kernel, plannerConfig)
        |> fun p ->
            p.CreatePlanAsync(
                "Create a book with 3 chapters about a T-REX trying to put on some shoes. As each chapter is written, save it the file for the story."
            )
        |> Async.AwaitTask
        |> Async.RunSynchronously

    Utils.printPlan plan

    Utils.proceedOrExit ()

    printfn "Running plan..."

    // Note another possibility is to use `kernel.RunAsync(plan)` which will run
    // the whole plan and report the output. But I wanted a custom run loop to
    // be able to log progress and see what's going on inside.
    Utils.runPlan kernel plan
    |> Async.RunSynchronously
