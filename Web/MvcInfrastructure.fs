﻿namespace PhoneCat.Web

open PhoneCat.DataAccess
open PhoneCat.Domain
open PhoneCat.Web.Controllers
open System
open System.Web.Mvc
open Hubs

module MvcInfrastructure =    

  type CompositionRoot(phones : seq<PhoneTypeProvider.Root>) =          
    inherit DefaultControllerFactory() with
      override this.GetControllerInstance(requestContext, controllerType) = 
        if controllerType = typeof<HomeController> then 
          let homeController = new HomeController()
          homeController :> IController
        else if controllerType = typeof<PhoneController> then          
          let phones' = phones |> Seq.map TypeProviders.ToCatalogPhone
          let observer = PhoneViewTracker.observePhonesViewed requestContext.HttpContext.Session
          let phoneController = new PhoneController(phones')
          phoneController.Subscribe observer |> ignore                    
          phoneController :> IController
        else if controllerType = typeof<ManufacturerController> then
          let getPhonesByManufactuerName = phones |> Seq.map TypeProviders.ToPhone |> Phones.getPhonesOfManufacturer
          let manufacturerController = new ManufacturerController(getPhonesByManufactuerName)
          manufacturerController :> IController
        else
          raise <| ArgumentException((sprintf "Unknown controller type requested: %A" controllerType))

  let initializeRecommendation httpContext phones = 
    let phones' = phones |> Seq.map TypeProviders.ToPhone
    let getPhoneById = Phones.getPhoneById phones'
    let recommendationHub = new RecommendationHub(httpContext, getPhoneById)
    Recommendation.RecommendationPipe.Subscribe recommendationHub
