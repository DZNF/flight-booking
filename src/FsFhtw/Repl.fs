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
            |> Map.add "Login" """Logs in given user (e.g. "login freddy kruger 24.12.1957 598234")."""
            |> Map.add "Logout" "Logs out the user."
    Map.fold (fun s k v -> sprintf "%s%s\t%s%s" s k v Environment.NewLine) "" h

let isMessageValidForState (state : State) (msg : Domain.Message) : bool =
    match (state.State, msg) with
    | (Domain.FSBSStates.NotLoggedIn, Domain.Message.Login _)
    | (Domain.FSBSStates.NotLoggedIn, Domain.Message.ListFlights _) -> true
    | (Domain.FSBSStates.NotLoggedIn, _) -> false
    | (Domain.FSBSStates.LoggedIn _, Domain.Message.Login _) -> false
    | (Domain.FSBSStates.LoggedIn _, _) -> true

let evaluate (update : Domain.Message -> State -> State) (state : State) (msg : Message) =
    match msg with
    | DomainMessage msg when isMessageValidForState state msg ->    
        let newState = update msg state
        let message = 
            match newState.State with
            | Domain.FSBSStates.LoggedIn p -> sprintf "Logged in as %s, %s." p.Person.LastName p.Person.FirstName
            | Domain.FSBSStates.NotLoggedIn -> "Not logged in."
        (newState, message)
    | DomainMessage _ ->
        (state, "Message is not valid for current state.")
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
