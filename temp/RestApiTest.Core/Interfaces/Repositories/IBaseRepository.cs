﻿using RestApiTest.Core.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestApiTest.Core.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> AddAsync(T objectToAdd, Action additionalPreSteps = null);
        Task DeleteAsync(long id, Action additionalPreSteps = null);
        Task<T> UpdateAsync(T objectToUpdate, Action additionalPreSteps = null);
        Task<T> GetAsync(long id);
        Task<T> ApplyPatchAsync(T objectToModify, List<PatchDTO> propertiesToUpdate);
        //TODO: [Optional] dodać też dla all - repo może mieć, ukryć w kontrolerze
    }
}
