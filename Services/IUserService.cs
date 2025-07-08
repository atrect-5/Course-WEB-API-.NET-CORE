﻿using Dtos;
using Dtos.User;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IUserService
    {
        /// <summary>
        /// Adds a new user to the system based on the provided user creation model.
        /// </summary>
        /// <param name="model">The data transfer object containing the details of the user to be created. Cannot be null.</param>
        /// <returns>A <see cref="UserDto"/> representing the newly created user.</returns>
        UserDto Add(CreateUserDto model);

        /// <summary>
        /// Retrieves a Collection of all users.
        /// </summary>
        /// <remarks>This method does not filter or paginate the results. It retrieves all users in a
        /// single operation.</remarks>
        /// <returns>A Collection of <see cref="UserDto"/> objects representing all users.  Returns an empty Collection if no users are
        /// available.</returns>
        IEnumerable<UserDto> GetAllUsers();

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to retrieve. Must be a positive integer.</param>
        /// <returns>A <see cref="UserDto"/> object representing the user with the specified identifier,  or <see
        /// langword="null"/> if no user with the given identifier exists.</returns>
        UserDto? GetUserById(int id);
        
        /// <summary>
        /// Updates an existing user with the specified ID using the provided data.
        /// </summary>
        /// <param name="id">The unique identifier of the user to update. Must be a positive integer.</param>
        /// <param name="model">The data transfer object with the fields to update. All fields are optional.</param>
        /// <returns>A <see cref="UserDto"/> representing the updated user, or <see langword="null"/> if no user with the
        /// specified ID exists.</returns>
        UserDto? Update(int id, UpdateUserDto model);

        /// <summary>
        /// Deletes the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete. Must be a positive integer.</param>
        /// <returns><see langword="true"/> if the entity was successfully deleted; otherwise, <see langword="false"/>.</returns>
        bool Delete(int id);
    }
}
