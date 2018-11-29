module SpanAlgebraTests
open NUnit.Framework
open SpanAlgebra


let intersectionChar = (char)0x2229
let unionChar = (char)0x222A
let plusChar = '+'

type Combine =
    | A = 1
    | B = 2
    | AnB = 3 // A ||| B

[<Test>]
let checkIntersection () =
    let spans1 = [ Span.create 0 1 Combine.A
                   Span.create 4 5 Combine.A
                   Span.create 7 10 Combine.A
                   Span.create 13 15 Combine.A
                   Span.create 17 20 Combine.A 
                   Span.create 25 200 Combine.A ]

    let spans2 = [ Span.create 2 3 Combine.B
                   Span.create 5 6 Combine.B
                   Span.create 8 11 Combine.B 
                   Span.create 12 14 Combine.B
                   Span.create 16 21 Combine.B 
                   Span.create 26 40 Combine.B
                   Span.create 45 50 Combine.B
                   Span.create 80 100 Combine.B ]

    let res = Span.intersect spans1 spans2 (|||) |> List.ofSeq
    printfn "input1:"
    spans1 |> List.iter (fun x -> printfn "  [%d, %d[ = %A" x.Start x.Stop x.Value)
    printfn ""
    printfn "input2:"
    spans2 |> List.iter (fun x -> printfn "  [%d, %d[ = %A" x.Start x.Stop x.Value)
    printfn ""
    printfn "result:"
    res |> List.iter (fun x -> printfn "  [%d, %d[ = %A" x.Start x.Stop x.Value)

    let expected = [ Span.create 8 10 Combine.AnB
                     Span.create 13 14 Combine.AnB
                     Span.create 17 20 Combine.AnB 
                     Span.create 26 40 Combine.AnB
                     Span.create 45 50 Combine.AnB
                     Span.create 80 100 Combine.AnB ]
    Assert.AreEqual(expected, res)


[<Test>]
let checkUnion () =
    let spans1 = [ Span.create 0 1 Combine.A
                   Span.create 4 5 Combine.A
                   Span.create 7 10 Combine.A 
                   Span.create 13 15 Combine.A
                   Span.create 17 20 Combine.A 
                   Span.create 25 200 Combine.A ]

    let spans2 = [ Span.create 2 3 Combine.B
                   Span.create 5 6 Combine.B
                   Span.create 8 11 Combine.B
                   Span.create 12 14 Combine.B
                   Span.create 16 21 Combine.B
                   Span.create 26 40 Combine.B
                   Span.create 45 50 Combine.B
                   Span.create 80 100 Combine.B ]

    printfn "input1:"
    spans1 |> List.iter (fun x -> printfn "  [%d, %d[ = %A" x.Start x.Stop x.Value)
    printfn ""
    printfn "input2:"
    spans2 |> List.iter (fun x -> printfn "  [%d, %d[ = %A" x.Start x.Stop x.Value)
    printfn ""
    printfn "result:"
    let res = Span.union spans1 spans2 (|||) |> List.ofSeq
    res |> List.iter (fun x -> printfn "  [%d, %d[ = %A" x.Start x.Stop x.Value)

    let expected = [ Span.create 0 1 Combine.A
                     Span.create 2 3 Combine.B
                     Span.create 4 5 Combine.A
                     Span.create 5 6 Combine.B
                     Span.create 7 8 Combine.A
                     Span.create 8 10 Combine.AnB
                     Span.create 10 11 Combine.B
                     Span.create 12 13 Combine.B
                     Span.create 13 14 Combine.AnB
                     Span.create 14 15 Combine.A
                     Span.create 16 17 Combine.B
                     Span.create 17 20 Combine.AnB
                     Span.create 20 21 Combine.B 
                     Span.create 25 26 Combine.A
                     Span.create 26 40 Combine.AnB
                     Span.create 40 45 Combine.A
                     Span.create 45 50 Combine.AnB
                     Span.create 50 80 Combine.A
                     Span.create 80 100 Combine.AnB
                     Span.create 100 200 Combine.A ]
    Assert.AreEqual(expected, res)

[<Test>]
let checkMerge () =
    let spans = [ Span.create 0 1 Combine.A
                  Span.create 2 3 Combine.A
                  Span.create 3 10 Combine.A
                  Span.create 13 15 Combine.B
                  Span.create 15 20 Combine.B ]

    printfn "input:"
    spans |> List.iter (fun x -> printfn "  [%d, %d[ = %A" x.Start x.Stop x.Value)
    let res = Span.merge spans |> List.ofSeq
    printfn ""
    printfn "result:"
    res |> List.iter (fun x -> printfn "  [%d, %d[ = %A" x.Start x.Stop x.Value)
    
    let expected = [ Span.create 0 1 Combine.A
                     Span.create 2 10 Combine.A
                     Span.create 13 20 Combine.B ]
    Assert.AreEqual(expected, res)

[<Test>]
let checkCreateNominal () =
    let int = Span.create 10 20 "toto"
    let expected = { Start = 10; Stop = 20; Value = "toto" }
    Assert.AreEqual(expected, int)

[<Test>]
let failureIfStartAndStopEqual () =
    try
        Span.create 10 10 "toto" |> ignore
        failwithf "Can't create interval with Start and Stop equal"
    with
        _ -> ()
    
[<Test>]
let failureIfStartGreaterThanStop () =
    try
        Span.create 10 8 "toto" |> ignore
        failwithf "Can't create interval with Start greater than Stop"
    with
        _ -> ()

[<Test>]
let checkValidateNominal () =
    let segs = [ Span.create 0 1 Combine.A
                 Span.create 2 3 Combine.A
                 Span.create 3 10 Combine.A
                 Span.create 13 15 Combine.B
                 Span.create 15 20 Combine.B ]
    segs |> Span.validate

[<Test>]
let checkValidateRejectsInvalidSpan () =
    let segs = [ Span.create 0 1 Combine.A
                 Span.create 2 3 Combine.A
                 { Span.Start = 10; Span.Stop = 3; Span.Value = Combine.A }
                 Span.create 13 15 Combine.B
                 Span.create 15 20 Combine.B ]
    try
        segs |> Span.validate
        failwithf "Validation should have detected invalid span"
    with
        _ -> ()

[<Test>]
let checkValidateRejectsUnorderedSpans () =
    let segs = [ Span.create 0 1 Combine.A
                 Span.create 3 10 Combine.A
                 Span.create 2 3 Combine.A
                 Span.create 13 15 Combine.B
                 Span.create 15 20 Combine.B ]
    try
        segs |> Span.validate
        failwithf "Validation should have detected unordered spans"
    with
        _ -> ()
