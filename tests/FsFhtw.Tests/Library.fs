module FsFhtw.Tests

open Xunit
open FsUnit

[<Fact>]
let ``That the setup is correct`` () =
    let actual = 1
    let expected = 1
    
    actual |> should equal expected
