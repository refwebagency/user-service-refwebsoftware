using System.Collections.Generic;
using UserService.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace UserService.Data
{
    public class UserRepo : IUserRepo
    {
        // L'attribut booléen readlony rend l'élément non mutable, l'utilisateur ne peut pas le modifier.
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context)
        {
            _context = context;
        }

        public void CreateUser(User user)
        {
            // On vérifie que les données lors de la création d'un user ne soient pas nulles.
            if(user != null)
            {
                // La méthode Add() ajoute un objet.
                _context.user.Add(user);
            }
            else 
            {
                // Sinon on retourne un message d'erreur.
                throw new ArgumentNullException(nameof(user));
            }
        }
        public IEnumerable<User> GetAllUsers()
        {
            // La méthode ToList() crée une liste des données.
            return _context.user.ToList();
        }

        public User GetUserById(int id)
        {
            // La méthode Find() recherche l'élément correspondant au paramètre spécifié.
            // Et on le retourne.
            return _context.user.Find(id);
        }

        public IEnumerable<User> GetUserByExpIdAndSpecId(int Xp, int SpecId)
        {
            return _context.user.Where(u => u.Experience == Xp && u.SpecializationId == SpecId).ToList();
        }
        
        public void UpdateUserById(int id)
        {
            // On stocke le user recherché avec la méthode Find() dans la variable userItem.
            var userItem = _context.user.Find(id);

            // La méthode Entry() permet un accés aux données de userItem et permet la modification de ces données.
            // Puis on dit au context que l'entité a été modifiée.
            _context.Entry(userItem).State = EntityState.Modified;
        }

        public void DeleteUserById(int id)
        {
            // La méthode Find() recherche l'élément correspondant au paramètre spécifié.
            // Et on le retourne.
            var userItem = _context.user.Find(id);

            // On vérifie que l'élément ne soit pas nul.
            if (userItem != null)
            {
                // On supprime avec la méthode Remove().
                _context.user.Remove(userItem);       
            }

        }

        //Verification du mail si éxistant
        public bool VerifyUserByEmail(string email)
        {
            //recuperation d'un user par rapport à son email
            var userEmail = _context.user.FirstOrDefault(u => u.Email == email);

            //renvoie true si utilisateur existe
            if(userEmail != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // La méthode SaveChanges permet de sauvegarer l'état des entités.
        public bool SaveChanges()
        {
            return (_context.SaveChanges() >=0);
        }

    }
}