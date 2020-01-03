module Domain

// Optional:
// OverbookingRate
// Compensation

type Airport =
    | IATA of string
    | ICAO of string
    | FAA of string

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
      DepartureTime: System.DateTime
      Duration: System.TimeSpan
      DepartureAirport: Airport
      ArrivalAirport: Airport }

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

type State = Booking list

let Flights = [
    { Flight.DepartureAirport = IATA "VIE"
      Flight.ArrivalAirport = IATA "FRA"
      Flight.DepartureTime = System.DateTime.Now
      Flight.Duration = System.TimeSpan.FromHours(1.0)
      Flight.Designator =
          { FlightDesignator.AirlineDesignator = "OE"
            FlightDesignator.Number = 123 }}
    { Flight.DepartureAirport = IATA "VIE"
      Flight.ArrivalAirport = IATA "DAL"
      Flight.DepartureTime = System.DateTime.Now
      Flight.Duration = System.TimeSpan.FromHours(10.0)
      Flight.Designator =
          { FlightDesignator.AirlineDesignator = "OE"
            FlightDesignator.Number = 887 }}
    { Flight.DepartureAirport = IATA "VIE"
      Flight.ArrivalAirport = IATA "BOM"
      Flight.DepartureTime = System.DateTime.Now
      Flight.Duration = System.TimeSpan.FromHours(12.0)
      Flight.Designator =
          { FlightDesignator.AirlineDesignator = "OE"
            FlightDesignator.Number = 334 }} ]

type Message =
    | ListBookings
    | ListFlights
    | CreateBooking

let init () : State = List.empty

let update (msg : Message) (model : State) : State =
    match msg with
    | ListBookings ->
        printfn "Bookings:"
        model |> List.iter (fun f -> printfn "%A" f)
        model
    | ListFlights ->
        Flights |> List.iter (fun a -> printfn "%A" a)
        model
