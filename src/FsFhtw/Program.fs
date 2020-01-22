[<EntryPoint>]
let main argv =
    printfn ""
    printfn """                                 |
                                 |
                                 |
                                _|_
                               /___\
                              /_____\
                             /oo   oo\
 \___________________________\       /___________________________/
  `-----------|------|--------\_____/--------|------|-----------´
             ( )    ( )     O|OOo|oOO|O     ( )    ( )"""
    printfn "                      _.~*               *~._"
    printfn "                  _.~* Flight Booking System *~._"
    printfn ""
    printfn "Please enter your commands to interact with the system."
    printfn "Press CTRL+C to stop the program."
    printf "> "

    let initialState = Domain.init ()
    Repl.loop initialState
    0 // return an integer exit code
