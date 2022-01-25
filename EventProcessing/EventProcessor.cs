using System;
using System.Text.Json;
using AutoMapper;
using UserService.Data;
using UserService.Dtos;
using Microsoft.Extensions.DependencyInjection;
using UserService.Models;

namespace user_service_refwebsoftware.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch(eventType)
            {
                // On souscrit à la méthode UpdateSpecialization() si la valeur retournée est bien EventType
                case EventType.SpecializationUpdated:
                    UpdateSpecialization(message);
                    break;
                case EventType.SpecializationPublished:
                    AddSpecialization(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining Event");

            // On déserialise les données pour retourner un objet (texte vers objet json)
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            Console.WriteLine($"--> Event Type: {eventType.Event}");

            switch(eventType.Event)
            {
                /* "Specialization_Updated" est la valeur attribuée dans le controller de SpecializationService
                lors de l'envoi de données 
                Dans le cas ou la valeur de notre attribue Event est bien "Specialization_Updated",
                nous retournons notre objet */
                case "Specialization_Updated":
                    Console.WriteLine("--> Platform Updated Event Detected");
                    return EventType.SpecializationUpdated;
                //Dans le cas ou la specialisation doit être crée
                case "Specialization_Published":
                    Console.WriteLine("--> Platform Updated Event Detected");
                    return EventType.SpecializationPublished;
                // Sinon nous retournons que l'objet est indeterminé
                default:
                    Console.WriteLine("-> Could not determine the event type");
                    return EventType.Undetermined;
            }
        }

        private void UpdateSpecialization(string SpecializationUpdatedMessage)
        {
            using(var scope = _scopeFactory.CreateScope())
            {
                // Recuperation du scope de meetRepo
                var repo = scope.ServiceProvider.GetRequiredService<IUserRepo>();

                //On deserialize le specializationUpdatedMessage
                var specializationUpdatedDto = JsonSerializer.Deserialize<UpdatedSpecializationDTO>(SpecializationUpdatedMessage);
                Console.WriteLine($"--> Specialization Updated: {specializationUpdatedDto}");

                try
                {

                    //Console.WriteLine(specializationUpdatedDto.Name);

                    var specializationRepo = repo.GetSpecializationById(specializationUpdatedDto.Id);
                    
                    _mapper.Map(specializationUpdatedDto, specializationRepo);
                    
                    // SI la specialization existe bien on l'update sinon rien
                    if(specializationRepo != null)
                    {
                        //Console.WriteLine(specializationRepo.Name);
                        repo.SaveChanges();
                        Console.WriteLine("--> Specialization mis à jour");
                    }
                    else{
                        Console.WriteLine("--> Specialization non existant");
                    }
                }
                // Si une erreur survient, on affiche un message
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not update Client to DB {ex.Message}");
                }
            }
        }

        private void AddSpecialization(string specializationPublishedMessage)
        {
            using(var scope = _scopeFactory.CreateScope())
            {
                // Recuperation du scope de meetRepo
                var repo = scope.ServiceProvider.GetRequiredService<IUserRepo>();

                //On deserialize le specializationPublishedMessage
                var specializationCreateDto = JsonSerializer.Deserialize<PublishedSpecializationDTO>(specializationPublishedMessage);
                Console.WriteLine($"--> Pecialization Created: {specializationCreateDto}");
                Console.WriteLine(specializationCreateDto.Id);
                try
                {
                    //Console.WriteLine("charal");
                    
                    
                    // SI la specialization n'existe pas on le crée sinon rien
                    if(!repo.IfSpecializationExist(specializationCreateDto.Id))
                    {
                        //Console.WriteLine("oukouk");
                        var NewSpec = _mapper.Map<Specialization>(specializationCreateDto);
                        //Console.WriteLine(specializationCreateDto.Id);
                        //Console.WriteLine(NewSpec.Name);
                        repo.CreateSpecialization(NewSpec);                  
                        repo.SaveChanges();
                        Console.WriteLine("--> Specialization crée");
                    }
                    else{
                        Console.WriteLine("--> La specialization existe déjà");
                    }
                }
                // Si une erreur survient, on affiche un message
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not create specialization to DB {ex.Message}");
                }
            }
        }
    }

    //Type d'event
    enum EventType
    {
        SpecializationUpdated,
        SpecializationPublished,
        Undetermined
    }
}