module Repl

open System
open Parser

type Message =
    | DomainMessage of Domain.Message
    | HelpRequested
    | NotParsable of string

type State = Domain.State

let read (input : string) =
    match input with
    | Help -> HelpRequested
    | ListBookings -> Domain.ListBookings |> DomainMessage
    | ListFlights -> Domain.ListFlights |> DomainMessage
    | SearchFlights f -> Domain.SearchFlights f |> DomainMessage
    | CreateBooking b -> Domain.CreateBooking b |> DomainMessage
    | Login p -> Domain.Login p |> DomainMessage
    | Logout -> Domain.Logout |> DomainMessage
    | ParseFailed  -> NotParsable input

// TODO: Needs update for split command display capability (e.g. `List Flights`)
let createHelpText () : string =
    let h = Map.empty
            |> Map.add "List Flights" "Prints a list of all the available flights."
            |> Map.add "List Bookings" "Prints a list of all the bookings."
            |> Map.add "Create Booking" """Creates a new booking."""
            |> Map.add "Search Flights" """Searches for a flight using the given departure and arrival IATA codes (e.g. "search flight VIE DAL")."""
    Map.fold (fun s k v -> sprintf "%s%s\t%s%s" s k v Environment.NewLine) "" h

let evaluate (update : Domain.Message -> State -> State) (state : State) (msg : Message) =
    match msg with
    | DomainMessage msg ->
        let newState = update msg state
        let message = sprintf "The message was %A. New state is %A" msg newState
        (newState, message)
    | HelpRequested ->
        let message = createHelpText ()
        (state, message)
    | NotParsable originalInput ->
        let message =
            sprintf """"%s" was not parsable. %s"""  originalInput "You can get information about known commands by typing \"Help\""
        (state, message)

let print (state : State, outputToPrint : string) =
    printfn "%s\n" outputToPrint
    printf "> "

    state

let rec loop (state : State) =
    Console.ReadLine()
    |> read
    |> evaluate Domain.update state
    |> print
    |> loop
