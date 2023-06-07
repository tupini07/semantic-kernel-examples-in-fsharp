module FileSystemSkill

open Microsoft.SemanticKernel
open Microsoft.SemanticKernel.SkillDefinition
open Microsoft.SemanticKernel.Orchestration
open System.IO
open System
open FSharpPlus

type FileSystemSkill() =
    static member GetFullFilePath filePath =
        Environment.CurrentDirectory
        + "/workdir/"
        + filePath

    [<SKFunction("Read all text from a document")>]
    [<SKFunctionInput(Description = "Path to the file to read")>]
    member this.ReadFromFile filePath (context: SKContext) =
        printfn "Reading text from %s" filePath

        let fullFilePath = FileSystemSkill.GetFullFilePath filePath

        if not (File.Exists(fullFilePath)) then
            failwith $"File {filePath} does not exist"

        File.ReadAllText(fullFilePath)

    [<SKFunction("Append text to a document. If the document doesn't exist, it will be created.")>]
    [<SKFunctionInput(Description = "Text to append")>]
    [<SKFunctionContextParameter(Name = "filePath", Description = "Destination file path")>]

    member this.AppendToFile text (context: SKContext) =
        let mutable filePath = Unchecked.defaultof<string>

        let isThereAFilePath = context.Variables.TryGetValue("filePath", &filePath)

        match isThereAFilePath with
        | false -> failwith "filePath variable not found"
        | true ->
            printfn "Appending text to %s" filePath

            let fullFilePath = FileSystemSkill.GetFullFilePath filePath
            File.AppendAllText(fullFilePath, text)
