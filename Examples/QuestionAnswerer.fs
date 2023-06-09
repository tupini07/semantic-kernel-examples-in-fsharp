module QuestionAnswerer

open Microsoft.SemanticKernel
open KernelSettings
open KernelBuilderExtensions
open Microsoft.SemanticKernel.Memory
open Microsoft.SemanticKernel.Skills.Web
open Microsoft.SemanticKernel.Planning
open Microsoft.SemanticKernel.Skills.Web.Bing
open System
open Microsoft.SemanticKernel.CoreSkills

let private createKernel () =
    let kernel =
        KernelBuilder()
            .WithCompletionService(KernelSettings)
            // save memory in RAM
            .WithOpenAITextEmbeddingGenerationService(
                modelId = "text-embedding-ada-002",
                apiKey = KernelSettings.ApiKey,
                serviceId = "helpful-chatbot-embedding-service"
            )
            .WithMemoryStorage(new VolatileMemoryStore())
            .Build()

    kernel.ImportSkill(TextMemorySkill()) |> ignore

    kernel.ImportSkill(SearchUrlSkill(), "search_url")
    |> ignore


    kernel.ImportSkill(WebSearchEngineSkill(new BingConnector(KernelSettings.BingApiKey)), "search")
    |> ignore

    let skillsDir = Environment.CurrentDirectory + "/skills"

    kernel.ImportSemanticSkillFromDirectory(skillsDir, "MiscSkill")
    |> ignore

    kernel


let answerQuestion () =
    printfn "Running question answerer example ..."

    let kernel = createKernel ()

    let planner = SequentialPlanner(kernel)

    // Tell the planner to look for something on the web and report back
    let plan =
        planner.CreatePlanAsync("Who is the girlfriend of Elon Musk? What is her age and height?")
        |> Async.AwaitTask
        |> Async.RunSynchronously

    Utils.printPlan plan

    Utils.proceedOrExit ()

    Utils.runPlan kernel plan
    |> Async.RunSynchronously


// let result =
//     kernel.RunAsync(plan)
//     |> Async.AwaitTask
//     |> Async.RunSynchronously

// printfn "Output"
// printfn "%s" result.Result
