module Parser

open System
open Domain

let (=~) (it : string) (theOther : string) =
    String.Equals(it, theOther, StringComparison.OrdinalIgnoreCase)

[<Literal>]
let HelpLabel = "Help"

[<Literal>]
let ListVerb = "List"

[<Literal>]
let CreateVerb = "Create"

[<Literal>]
let SearchVerb = "Search"

let (|Help|ParseFailed|ListBookings|ListFlights|CreateBooking|SearchFlights|) (input : string) =
    let parts = input.Split(' ') |> List.ofArray
    match parts with
    | [ verb ] when verb =~ HelpLabel -> Help
    | [ verb; arg ] when verb =~ ListVerb && arg =~ "Flights" -> ListFlights
    | [ verb; arg ] when verb =~ ListVerb && arg =~ "Bookings" -> ListBookings
    | [ verb; arg; flightDesignator; luggage ] when verb =~ CreateVerb && arg =~ "Booking" -> 
        CreateBooking (flightDesignator, Boolean.Parse(luggage))
    | [ verb; arg; departureAirport; arrivalAirport ] when verb =~ SearchVerb && arg =~ "Flights" -> 
        SearchFlights ({ Airport.IATA = departureAirport.ToUpper() }, { Airport.IATA = arrivalAirport.ToUpper() })
    | _ -> ParseFailed
