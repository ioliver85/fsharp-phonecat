﻿namespace PhoneCat.Web.Controllers

open System.Web
open System.Web.Mvc
open PhoneCat.Domain.Catalog
open System

type PhoneViewModel = 
  {
    Name : string
    Description : string
    Os : string
    Ui : string
    Flash : string
    Ram : string
    Weight : string
    ScreenResolution : string
    ScreenSize : string
    TouchScreen : string
    Primary : string
    Features : seq<string>
    Images : seq<string>    
  }

  with static member ToPhoneViewModel(phone : Phone) =
        let roundToDigits (value:float) = String.Format("{0:0.00}", value)
        let uomToString (measureValue: float<_>) measureName =
          let value = measureValue |> float |> roundToDigits  
          value + " " + measureName
        
        { 
          Name = phone.Name
          Description = phone.Description
          Os = phone.Android.OS
          Ui = phone.Android.UI
          Flash = phone.Storage.Flash |> int |> sprintf "%d MB"
          Ram =  phone.Storage.Ram |> int |> sprintf "%d MB"
          Weight = uomToString phone.Weight "grams"
          ScreenResolution = phone.Display.ScreenResolution
          ScreenSize = uomToString phone.Display.ScreenSize "inches"
          TouchScreen = if phone.Display.TouchScreen then "Yes" else "No"
          Primary = phone.Camera.Primary
          Features = phone.Camera.Features
          Images = phone.Images
        }

type PhoneController(phones : seq<Phone>) =
  inherit Controller()
  member this.Show (id : string) = 
    let phone = phones |> Seq.find (fun p -> p.Id = id) |> PhoneViewModel.ToPhoneViewModel 
    this.View(phone)

