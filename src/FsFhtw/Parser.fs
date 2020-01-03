module Parser

open System

let (=~) (it : string) (theOther : string) =
    String.Equals(it, theOther, StringComparison.OrdinalIgnoreCase)

[<Literal>]
let HelpLabel = "Help"

[<Literal>]
let ListVerb = "List"

let (|Help|ParseFailed|ListFlights|) (input : string) =
    let parts = input.Split(' ') |> List.ofArray
    match parts with
    | [ verb ] when verb =~ HelpLabel -> Help
    | [ verb; arg ] when verb =~ ListVerb && arg =~ "Flights" ->
        ListFlights
    | _ -> ParseFailed
