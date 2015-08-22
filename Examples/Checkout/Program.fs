open Library

open System
open System.Globalization

let countries =
    CultureInfo.GetCultures CultureTypes.SpecificCultures 
    |> Seq.map (fun culture -> RegionInfo(culture.LCID).EnglishName)
    |> Seq.distinct
    |> Seq.sort

let isRequired (answer:string) = 
    if String.IsNullOrEmpty answer then "Required" else null

let validateCardNo (s:string) =
    if s.Length = 16 && s |> Seq.forall Char.IsDigit 
    then null else "Incorrect card"

do  Form.Title <- "Checkout"
    
    Form.Group("Delivery Address")
    Form.Question("Full Name", isRequired)   
    Form.Question("Address Line 1", isRequired)
    Form.Question("Address Line 2")
    Form.Question("Town/City", isRequired)
    Form.Question("County")
    Form.Question("Postcode", isRequired)
    Form.Options("Country", countries, "United Kingdom")
    Form.Question("Phone")

    Form.Group("Card")    
    Form.NumericalQuestion("Card number", validateCardNo)
    Form.Question("Name on card", isRequired)
       
    Form.Information("Expiration date")
    Form.Options("Month", [1..12], Clock.Month)
    let year = Clock.Year
    Form.Options("Year", [year..year+20])
    
    Form.Group("Terms & Conditions")
    Form.Choice("I agree to the terms and conditions")
    
    Form.OnValidate(fun _ -> true)
    Form.OnSubmit(fun answers -> 
        answers.ForEach TextWindow.WriteLine
        Form.Clear()
    )

