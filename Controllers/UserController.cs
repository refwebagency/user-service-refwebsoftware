using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserService.Models;
using UserService.Data;
using UserService.Dtos;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace UserService.Controllers
{
    // L'attribut ApiController permet de bénéficier de certaines conventions
    // et évite la duplication de code concernant la validation des données, il la gère automatiquement.
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        // L'attribut booléen readlony rend l'élément non mutable, l'utilisateur ne peut pas le modifier.
        private readonly IUserRepo _repository;
        private readonly IMapper _mapper;
        private readonly HttpClient _HttpClient;

        public UserController(IUserRepo repository, IMapper mapper, HttpClient HttpClient)
        {
            _repository = repository;
            _mapper = mapper;
            _HttpClient = HttpClient;
        }

        [HttpGet]

        // On appelle la classe abstraite ActionResult pour avoir un retour
        // puis la classe UserReadDto pour suivre le modèle du dto
        // et on crée la fonction GetAllUsers().
        public ActionResult<IEnumerable<UserReadDto>> GetAllUsers()
        {
            // On applique la méthode GetAllUsers() de la classe UserRepo 
            // et on stocke le résultat dans la variable userItem.
            var userItem = _repository.GetAllUsers();

            // La méthode Ok retourne un statut 200 et la liste de tous les users.
            return Ok(_mapper.Map<IEnumerable<UserReadDto>>(userItem));
        }


        [HttpGet("{id}", Name = "GetUserById")]

        // On appelle la classe abstraite ActionResult pour avoir un retour
        // puis la classe UserReadDto pour suivre le modèle du dto
        // et on crée la fonction GetUserById().
        public ActionResult<UserReadDto> GetUserById(int id)
        {
            // On applique la méthode GetUserById() de la classe UserRepo 
            // et on stocke le résultat dans la variable userItem.
            var userItem = _repository.GetUserById(id);

            // On vérifie que userItem ne soit pas vide.
            if (userItem == null)
            {
                return NotFound();
            }

            // La méthode Ok retourne un statut 200 et l'user avec l'id demandée.
            return Ok(_mapper.Map<UserReadDto>(userItem));
        }

        [HttpGet("meet/id", Name = "GetUserByMeetId")]

        // On appelle la classe abstraite ActionResult pour avoir un retour
        // puis la classe UserReadDto pour suivre le modèle du dto
        // et on crée la fonction GetUserById().
        public ActionResult<IEnumerable<UserReadDto>> GetUserByMeetId(int id)
        {
            // On applique la méthode GetUserById() de la classe UserRepo 
            // et on stocke le résultat dans la variable userItem.
            var userItem = _repository.GetUserByMeetId(id);

            // On vérifie que userItem ne soit pas vide.
            if (userItem == null)
            {
                return NotFound();
            }

            // La méthode Ok retourne un statut 200 et l'user avec l'id demandée.
            return Ok(_mapper.Map<IEnumerable<UserReadDto>>(userItem));
        }


        [HttpGet("{Xp}/{SpecId}", Name = "GetUserByExpIdAndSpecId")]

        // On appelle la classe abstraite ActionResult pour avoir un retour
        // puis la classe UserReadDto pour suivre le modèle du dto
        // et on crée la fonction GetUserById().
        public ActionResult<IEnumerable<UserReadDto>> GetUserByExpIdAndSpecId(int Xp, int SpecId)
        {
            var userItems = _repository.GetUserByExpIdAndSpecId(Xp, SpecId);

            while (userItems.Count() == 0)
            {
                Xp -= 1;
                userItems = _repository.GetUserByExpIdAndSpecId(Xp, SpecId);

                if (Xp == -1)
                {
                    return NotFound();
                }
            }

            return Ok(_mapper.Map<IEnumerable<UserReadDto>>(userItems));
        }


        [HttpPut("{id}", Name = "UpdateUserById")]

        // On appelle la classe abstraite ActionResult pour avoir un retour
        // puis la classe UserUpdateDto pour suivre le modèle du dto
        // et on crée la fonction UpdateUserById().
        public ActionResult<UserUpdateDto> UpdateUserById(int id, UserUpdateDto updateUser)
        {
            // On applique la méthode GetUserById() de la classe UserRepo 
            // et on stocke le résultat dans la variable userItem.
            var userItem = _repository.GetUserById(id);

            // On lui donne la route a suivre pour l'update qui passera par le dto
            // et qui prendra la variable userItem en paramètre.
            _mapper.Map(updateUser, userItem);

            // On vérifie que userItem ne soit pas vide.
            if (userItem == null)
            {
                return NotFound();
            }

            // On applique la méthode UpdateUserById de la classe UserRepo.
            _repository.UpdateUserById(id);

            // On sauvegarde les changements.
            _repository.SaveChanges();

            // La CreatedAtRoute crée une route.
            // Cette méthode est destinée à renvoyer un URI
            // à la ressource nouvellement créée lorsqu'on appelle une méthode POST ou PUT pour stocker un nouvel objet.
            return CreatedAtRoute(nameof(GetUserById), new { id = updateUser.Id }, updateUser);
        }


        [HttpPost]
        public async Task<ActionResult<UserReadDto>> CreateUser(UserCreateDto createUserDto)
        {
            var userModel = _mapper.Map<User>(createUserDto);

            // stocke le mail et soubscription à la méthode qui verifie si l'email déjà existant
            var mail = _repository.VerifyUserByEmail(userModel.Email);

            //si email déjà existant retourne NotFound avec message d'erreur
            if (mail == true)
            {
                Console.WriteLine("Un compte contenant cet émail est déjà éxistant");
                return NotFound();
            }
            else
            {
                var getSpecialization = await _HttpClient.GetAsync("https://localhost:2222/Specialization/" + userModel.SpecializationId);

                var deserializeSpecialization = JsonConvert.DeserializeObject<CreateSpecializationDTO>(
                    await getSpecialization.Content.ReadAsStringAsync());

                var SpecializationDTO = _mapper.Map<Specialization>(deserializeSpecialization);
                
                var specialization = _repository.GetSpecializationById(SpecializationDTO.Id);

                if (specialization == null) userModel.Specialization = SpecializationDTO; else userModel.Specialization = specialization;

                //Sinon crée un nouvel utilisateur        
                _repository.CreateUser(userModel);
                _repository.SaveChanges();

                // On stocke le résultat de l'user nouvellement crée dans la variable readUser.

                var readUser = _mapper.Map<UserReadDto>(userModel);

                // La CreatedAtRoute crée une route.
                // Cette méthode est destinée à renvoyer un URI
                // à la ressource nouvellement créée lorsqu'on appelle une méthode POST ou PUT pour stocker un nouvel objet.
                return CreatedAtRoute(nameof(GetUserById), new { id = readUser.Id }, readUser);

            }
        }


        [HttpDelete("{id}")]

        // On appelle la classe abstraite ActionResult pour avoir un retour
        // puis la classe ReadUser pour suivre le modèle du dto
        // et on crée la fonction DeleByUserId().
        public ActionResult DeleteUserById(int id)
        {
            // On applique la méthode GetUserById() de la classe UserRepo 
            // et on stocke le résultat dans la variable userItem.
            var userItem = _repository.GetUserById(id);

            // On vérifie que userItem ne soit pas vide.
            if (userItem == null)
            {
                return NotFound();
            }

            // On applique la méthode DeleteByUserId de la classe UserRepo.
            _repository.DeleteUserById(userItem.Id);

            // On sauvegarde les changements.
            _repository.SaveChanges();

            return NoContent();
        }

    }
}