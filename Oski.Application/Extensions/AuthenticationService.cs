﻿using Oski.Application.Interfaces;
using Oski.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Oski.Domain.Entities;
using System.Security.Cryptography;
 

namespace Oski.Application.Extensions
{
    public class AuthenticationService:IAuthenticationService
    {
        private readonly IGenericRepository<User> _userRepository; 
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;


        // TODO: use IMediator and cqrs pattern
        public AuthenticationService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _userRepository = _unitOfWork.Repository<User>();
        
        }


        public string Authenticate(string email,string password)
        {
            var user = _userRepository.GetAllAsync().Result.Where(x=>x.Email == email).FirstOrDefault();


            if(user == null)
                return null;

            if(!VerifyPassword(password,user.Password))
                return null;  

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool Register(string name,string email,string password)
        {

            var existingUser = _unitOfWork.Repository<User>().GetAllAsync().Result.Find(x=>x.Email == email);
            if(existingUser != null)
                return false; 

            var hashedPassword = HashPassword(password); 

            var user = new User { FullName = name,Email = email, Password = hashedPassword };
            
            _userRepository.AddAsync(user);
            _unitOfWork.Save(user.Id,default);

            return true;  
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password,string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password,hashedPassword);
        }


    }
}
