module NovelCreator

open Microsoft.SemanticKernel
open System.IO
open Microsoft.SemanticKernel
open Microsoft.SemanticKernel.Planning
open Microsoft.SemanticKernel.Planning.Sequential
open System
open FileSystemSkill

let rec private runPlan (kernel: IKernel) (plan: Plan) =
    async {
        if not plan.HasNextStep then
            printfn "Plan execution completed!"

        else
            printfn "-----------------------------------"
            printfn "Starting step execution..."

            do!
                kernel.StepAsync(plan)
                |> Async.AwaitTask
                |> Async.Ignore

            printfn "Finished step"
            printfn "%s" (plan.State.ToString())

            return! runPlan kernel plan
    }

let private printPlan (plan: Plan) =
    printfn ""
    printfn "Original plan:\n%s" plan.Description

    printfn ""
    printfn "Plan steps:"

    for step in plan.Steps do
        let paramsS =
            step.Parameters
            |> Seq.map (fun kvp -> sprintf "%s='%s'" kvp.Key kvp.Value)
            |> String.concat ", "

        let outputs = step.Outputs |> String.concat ", "

        printfn "> %s.%s [%s] => %s" step.SkillName step.Name paramsS outputs

    printfn ""


let createNovel (kernel: IKernel) =
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
                "Create a book with 3 chapters about a llama trying to order a coffee from a turtle in the forest. After every chapter is generated, save it to a file. Use the same file for all chapters. Name the file something that is relevant to the story, and indicate where each chapter starts."
            )
        |> Async.AwaitTask
        |> Async.RunSynchronously

    printPlan plan

    printfn "Running plan..."
    runPlan kernel plan |> Async.RunSynchronously

//     printfn "Running text completion example..."


//     let context = ContextVariables()
//     context.Set("input", "Time travel to dinosaur age")
//     context.Set("style", "Dank")

//     let result =
//         kernel.RunAsync(context, skill.["Joke"])
//         |> Async.AwaitTask
//         |> Async.RunSynchronously

//     printfn "%A" result
