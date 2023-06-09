open System

[<EntryPoint>]
let rec main argv =
    printfn
        """
===================================        
Which example do you want to run?

1) Novel Creator
2) Question Answerer
q) Quit

Enter your input:"""

    let input = Console.ReadLine().Trim()

    printfn "==================================="
    
    match input with
    | "1" -> NovelCreator.createNovel ()
    | "2" -> QuestionAnswerer.answerQuestion ()
    | "q" ->
        printfn "Bye!"
        exit 0
    | s -> printfn "Invalid input '%s'" s

    main argv
