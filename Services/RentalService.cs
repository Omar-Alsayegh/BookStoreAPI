using BookStoreApi.Entities;
using BookStoreApi.Extra;
using BookStoreApi.Models;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Models.DTOs.Response;
using BookStoreApi.Repositories;
using Microsoft.AspNetCore.Identity;

namespace BookStoreApi.Services
{
    public class RentalService : IRentalService
    {
        public DateTime FixTime(DateTime time,int? days)
        {
            var day = (double)days;
           var newtime=time.AddDays(day);
            
            return newtime;
        }

    }
}
