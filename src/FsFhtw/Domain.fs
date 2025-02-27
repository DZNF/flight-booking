module Domain

open System

type Airport =
    { IATA: string }

type Luggage = bool

type Person =
    { FirstName: string
      LastName: string
      Birthday: string }

type Passenger =
    { Person: Person
      FrequentFlyerProgramId: string option }

type FlightDesignator =
    { AirlineDesignator: string
      Number: int }

type Flight =
    { Designator: FlightDesignator
      DepartureTime: DateTime
      Duration: TimeSpan
      DepartureAirport: Airport
      ArrivalAirport: Airport }
    override this.ToString() =
        sprintf "* Flight %2s/%-7i %s%s" this.Designator.AirlineDesignator this.Designator.Number (this.DepartureTime.ToString("dd.MM.yy  hh:mm")) Environment.NewLine +
        sprintf "  %-5s -> %-8s Duration: %s" this.DepartureAirport.IATA this.ArrivalAirport.IATA (this.Duration.ToString("hh\:mm"))

type PaymentState =
    | Error of string
    | Success

type PaymentMethod =
    | Cash
    | PayPal of PaymentState
    | CreditCard of PaymentState

type Price =
    { Amount: decimal
      Currency: string }

type Payment =
    { Method: PaymentMethod
      Price: Price }

type BoardingInfo =
    { Gate: string option
      Seat: string option }

type Booking =
    { Passenger: Passenger
      PaymentInfo: Payment
      Flight: Flight
      BoardingInfo: BoardingInfo option
      Luggage: Luggage }

type FSBSStates =
    | NotLoggedIn
    | LoggedIn of Passenger

type State = { Bookings: Booking list
               State: FSBSStates }

let Flights = [
    { Flight.DepartureAirport = { IATA = "VIE" }
      Flight.ArrivalAirport = { IATA = "FRA"}
      Flight.DepartureTime = DateTime.Now
      Flight.Duration = TimeSpan.FromHours(1.0)
      Flight.Designator =
          { FlightDesignator.AirlineDesignator = "OE"
            FlightDesignator.Number = 123 }}
    { Flight.DepartureAirport = { IATA = "VIE" }
      Flight.ArrivalAirport = { IATA = "DAL" }
      Flight.DepartureTime = DateTime.Now
      Flight.Duration = TimeSpan.FromHours(10.0)
      Flight.Designator =
          { FlightDesignator.AirlineDesignator = "OE"
            FlightDesignator.Number = 887 }}
    { Flight.DepartureAirport = { IATA = "VIE" }
      Flight.ArrivalAirport = { IATA = "BOM" }
      Flight.DepartureTime = DateTime.Now
      Flight.Duration = TimeSpan.FromHours(12.0)
      Flight.Designator =
          { FlightDesignator.AirlineDesignator = "OE"
            FlightDesignator.Number = 334 }} ]

type Message =
    | ListBookings
    | ListFlights
    | CreateBooking of string * bool
    | SearchFlights of Airport * Airport
    | Login of Passenger
    | Logout

let searchFlights (departureAirport : Airport) (arrivalAirport : Airport) : Flight list =
    List.filter (fun flight -> flight.DepartureAirport = departureAirport && flight.ArrivalAirport = arrivalAirport) Flights

let getFlight (designator : FlightDesignator) : Flight =
    List.find (fun flight -> flight.Designator = designator) Flights

let parseDesignator (designator : string) : FlightDesignator =
    let splitDesignator = designator.Split('/') |> List.ofArray
    let (_, parseNumber) = Int32.TryParse (List.item 1 splitDesignator)
    { FlightDesignator.AirlineDesignator = (List.head splitDesignator).ToUpper()
      FlightDesignator.Number = parseNumber }

let init () : State = { Bookings = List.empty
                        State = NotLoggedIn }

let update (msg : Message) (model : State) : State =
    match msg with
    | ListBookings ->
        match model.Bookings with 
        | [] -> printfn "No Bookings."
        | _ -> 
            printfn "Bookings:"
            model.Bookings |> List.iter (fun f -> printfn "%A" f)
        model
    | ListFlights ->
        Flights |> List.iter (fun a -> printfn "%O" a)
        model
    | SearchFlights (departure, arrival) ->
        printfn "Results for %A -> %A:" departure.IATA arrival.IATA
        searchFlights departure arrival |> List.iter (fun a -> printfn "%A" a)
        model
    | CreateBooking (flightDesignator, luggage) ->
        let designator = parseDesignator flightDesignator
        let flight = getFlight designator
        let passenger =
            match model.State with
            | LoggedIn p -> p
            | _ -> failwith "Should not happen :>"
        let booking = { Booking.Flight = flight
                        Booking.Luggage = luggage
                        Booking.Passenger = passenger
                        Booking.PaymentInfo = { Payment.Method = Cash;
                                                Payment.Price = { Price.Amount = 200m; Price.Currency = "EUR"}}
                        Booking.BoardingInfo = None }
        printfn "Booking created:%s%A" Environment.NewLine booking
        let updatedBookings = List.append model.Bookings [booking]
        { model with Bookings = updatedBookings }
    | Login passenger ->
        { model with State = LoggedIn passenger }
    | Logout ->
        { model with State = NotLoggedIn }
