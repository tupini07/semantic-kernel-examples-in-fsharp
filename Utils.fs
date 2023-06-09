module Utils

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

#nowarn "3391"

let printStep (step: Plan) =
    let paramsS =
        step.Parameters
        |> Seq.map (fun kvp -> sprintf "%s='%s'" kvp.Key kvp.Value)
        |> String.concat ", "

    let outputs =
        if step.Outputs.Count > 0 then
            "=> " + (step.Outputs |> String.concat ", ")
        else
            ""

    printfn "> %s.%s [%s] %s" step.SkillName step.Name paramsS outputs


let printPlan (plan: Plan) =
    printfn ""
    printfn "Original plan:\n%s" plan.Description

    printfn ""
    printfn "Plan steps:"

    for step in plan.Steps do
        printStep step

    printfn ""

let rec runPlan (kernel: IKernel) (plan: Plan) =
    async {
        printfn "-----------------------------------"

        if not plan.HasNextStep then
            printfn "-----------------------------------"

            printfn "Plan execution completed!"

            printfn "\nFinal plan outputs:"

            for outp in plan.Outputs do
                printfn "\t - %s => %s" outp plan.State[outp]

            printfn "-----------------------------------"
            printfn "-----------------------------------"

        else
            printfn "Starting step execution..."

            let currentStep = plan.Steps.[plan.NextStepIndex]

            do!
                kernel.StepAsync(plan)
                |> Async.AwaitTask
                |> Async.Ignore

            printfn "Finished step"
            printStep currentStep

            for stepOutName in currentStep.Outputs do
                printfn "\t - %s => %s" stepOutName plan.State[stepOutName]

            return! runPlan kernel plan
    }

let proceedOrExit () =
    // ask user if we should proceed with plan
    printfn "Proceed with plan? (y/n)"
    let proceed = Console.ReadLine() = "y"

    if not proceed then
        printfn "Stopping execution..."
        exit 0
