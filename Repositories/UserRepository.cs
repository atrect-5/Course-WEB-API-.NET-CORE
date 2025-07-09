using Data;
using Dtos;
using Dtos.User;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class UserRepository(ProjectDBContext context) : IUserService
    {
        private readonly ProjectDBContext _dbContext = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<UserDto> AddAsync(CreateUserDto createUserDto)
        {
            ArgumentNullException.ThrowIfNull(createUserDto);
            // Validar que el email no exista ya en la base de datos.
            if (await _dbContext.Users.AnyAsync(u => u.Email == createUserDto.Email))
                throw new ArgumentException("El correo electrónico ya está en uso.", nameof(createUserDto));
            
            var user = new User
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password)
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return new UserDto { Id = user.Id, Name = user.Name, Email = user.Email };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            // Devolvemos 'false' para que el controlador pueda manejar cuando no se encontro un usuario (ej. con un 404 Not Found).
            if (user is null)
                return false;

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync() =>
            await _dbContext.Users
                .Select(u => new UserDto { Id = u.Id, Name = u.Name, Email = u.Email })
                .ToListAsync();

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user is null)
                return null;

            return new UserDto { Id = user.Id, Email = user.Email, Name = user.Name };
        }


        public async Task<UserDto?> UpdateAsync(int id, UpdateUserDto updateUserDto)
        {
            ArgumentNullException.ThrowIfNull(updateUserDto);
            var userInDb = await _dbContext.Users.FindAsync(id);
            // Devolvemos 'null' para que el controlador pueda manejar cuando no se encontro un usuario (ej. con un 404 Not Found).
            if (userInDb is null)
                return null;
            
            // Actualiza si es que se mando la informacion
            if (!string.IsNullOrWhiteSpace(updateUserDto.Name))
                userInDb.Name = updateUserDto.Name;
            if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
                userInDb.Password = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
                
            await _dbContext.SaveChangesAsync();
            return new UserDto { Id = userInDb.Id, Name = userInDb.Name, Email = userInDb.Email};
        }
    }
}
