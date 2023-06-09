open System

[<EntryPoint>]
let rec main argv =
    printfn
        """
===================================        
What do you want to do?

1) Run the helpful chatbot example
2) Run the novel generation example
q) Quit

Enter your input:"""

    let input = Console.ReadLine().Trim()

    printfn "==================================="
    
    match input with
    | "1" -> HelpfulChat.runHelpfulChatbot ()
    | "2" -> NovelCreator.createNovel ()
    | "q" ->
        printfn "Bye!"
        exit 0
    | s -> printfn "Invalid input '%s'" s

    main argv
